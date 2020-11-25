using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public AudioClip ShootSound;  //指定需要播放的音效
    private AudioSource source;   //必须定义AudioSource才能调用AudioClip
    public Transform firePoint;
    public GameObject projectilePrefab;
    [SerializeField] private float fireRate = 0.5f;

    public GameObject shellPrefab;
    public Transform shellTrans;

    private float timer;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
            Shot();
    }
    public void Shot()
    {
        timer += Time.deltaTime;
        if (timer > fireRate)
        {
            timer = 0;
            GameObject spawnProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            AudioSource.PlayClipAtPoint(ShootSound, firePoint.transform.position);
            Instantiate(shellPrefab, shellTrans.position, shellTrans.rotation);//弹壳

        }
    }

    public float GetHeight
    {
        get
        {
            return firePoint.position.y;
        }
    }
}
