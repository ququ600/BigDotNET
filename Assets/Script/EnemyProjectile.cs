using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float shootSpeed;
    [SerializeField] private float damage = 1.0f;
    [SerializeField] private float lifetime;
    public Transform self;
    private Transform target;
    public LayerMask collisionMask;//Enemy Layer
    // Start is called before the first frame update
    private void Start()
    {
        Destroy(gameObject, lifetime);//发射后N秒内销毁
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);//防止武器在敌人身体内部不造成伤害
        if (initialCollisions.Length > 0)
        {
            HitPlayer(initialCollisions[0], transform.position);
        }
    }

    // Update is called once per frame
    private void Update()
    {

        transform.Translate(Vector3.forward * shootSpeed * Time.deltaTime);
        CheckCollision();
    }

    private void CheckCollision()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        if (Physics.Raycast(ray, out hitInfo, shootSpeed * Time.deltaTime, collisionMask))
        {
            Debug.Log(Time.deltaTime);
            HitPlayer(hitInfo, hitInfo.point);
            //HitEnemy(hitInfo.collider, hitInfo.point);
        }
    }

    private void HitPlayer(RaycastHit _hitInfo, Vector3 _hitPoint)
    {
        IDamageable damageableObject = _hitInfo.collider.GetComponent<IDamageable>();
        if (damageableObject != null)
            damageableObject.TakeHit(damage, _hitPoint, transform.forward);
        Destroy(gameObject);//击中敌人后，子弹销毁
        Debug.Log("Destroy");
    }

    private void HitPlayer(Collider _collider, Vector3 _hitPoint)
    {
        IDamageable damageableObject = _collider.GetComponent<IDamageable>();
        if (damageableObject != null)
            damageableObject.TakeHit(damage, _hitPoint, transform.forward);
        Destroy(gameObject);
        Debug.Log("Destroy");
    }

}
