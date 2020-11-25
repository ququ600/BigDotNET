using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float shootSpeed;
    [SerializeField] private float damage = 1.0f;
    [SerializeField] private float lifetime;
    public LayerMask collisionMask;//Enemy Layer
    // Start is called before the first frame update
    private void Start()
    {
        Destroy(gameObject, lifetime);//发射后N秒内销毁
        // Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        // if (initialCollisions.Length > 0)
        // {
        //     HitEnemy(initialCollisions[0], transform.position);
        // }
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Translate(Vector3.up * shootSpeed * Time.deltaTime);
        CheckCollision();
    }

    private void CheckCollision()
    {
        Ray ray = new Ray(transform.position, transform.up);
        RaycastHit hitInfo;
        Debug.DrawRay(transform.position, transform.up, Color.red);
        if (Physics.Raycast(ray, out hitInfo, shootSpeed * Time.deltaTime, collisionMask))
        {
            Debug.Log(Time.deltaTime);
            HitEnemy(hitInfo, hitInfo.point);
            //HitEnemy(hitInfo.collider, hitInfo.point);
        }
    }

    private void HitEnemy(RaycastHit _hitInfo, Vector3 _hitPoint)
    {
        IDamageable damageableObject = _hitInfo.collider.GetComponent<IDamageable>();
        if (damageableObject != null)
            damageableObject.TakeHit(damage, _hitPoint, transform.forward);
        Destroy(gameObject);//击中敌人后，子弹销毁
    }

    private void HitEnemy(Collider _collider, Vector3 _hitPoint)
    {
        IDamageable damageableObject = _collider.GetComponent<IDamageable>();
        if (damageableObject != null)
            damageableObject.TakeHit(damage, _hitPoint, transform.forward);
        Destroy(gameObject);
    }
    private void HitEnemy(RaycastHit _hitInfo)
    {
        IDamageable damageableObject = _hitInfo.collider.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakenDamage(damage);
        }
        Destroy(gameObject);
    }
}
