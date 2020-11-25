using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : LivingEntity
{
    private Rigidbody rb;
    private Vector3 moveInput;
    [SerializeField] private float moveSpeed;
    public GameObject projectile;

    //public Crosshairs crosshairs;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        LookAtCursor();

        if (transform.position.y < -10)//如果玩家掉下去的话，就GameOver了
        {
            TakenDamage(health);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
    private void LookAtCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Plane plane = new Plane(Vector3.up, Vector3.zero);
        Plane plane = new Plane(Vector3.up, Vector3.up * FindObjectOfType<GunController>().GetHeight);

        float distToGround;
        if (plane.Raycast(ray, out distToGround))
        {
            Vector3 point = ray.GetPoint(distToGround);
            Vector3 rightPoint = new Vector3(point.x, transform.position.y, point.z);

            transform.LookAt(rightPoint);

        }
    }
    void OnCollisionStay(Collision collision)
    {
        // 销毁当前游戏物体
        var name = collision.collider.name;


        if (MapGenerator.triggerTile.Contains(collision.gameObject))
        {
            MapGenerator.triggerTile.Remove(collision.gameObject);
            Debug.Log("collision enter" + collision.collider.name);
            MapGenerator.dynamicCreateTile(collision.gameObject);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        // 销毁当前游戏物体
        var name = collision.collider.name;
        Debug.Log(name);

        if (name == "Capsule(Clone)")
        {


            Projectile pro = GetComponent<Projectile>();
            pro.collisionMask = 0 << 9;
        }
    }
}
