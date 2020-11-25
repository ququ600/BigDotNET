using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class StrongerEnemy : LivingEntity
{
    public enum State { Idle, Chasing, Attacking };
    private NavMeshAgent navMeshAgent;
    private Transform target;
    public GameObject projectilePrefab;
    public Transform firePoint;
    [SerializeField] private float attackDistanceThreshold = 10f;
    private float timeBtwAttack = 0.5f;
    public LayerMask PlayerMask;
    private float nextAttackTime;
    public State currentState;
    public float damage = 1.0f;
    public float fireRate;

    [SerializeField] private float updateRate;

    [SerializeField] private bool hasTarget;

    public ParticleSystem deathEffect;

    Material skinMaterial;
    Color originalColor;
    public static event Action onDeathStatic;//为了记分而创建的事件
    private float timer;

    private void Awake()
    {
        Debug.Log("awake");
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)//因为之后敌人生成时，很可能Player已经阵亡了，首先要判断限制
        {
            hasTarget = true;
            Debug.Log("hastarget");
            target = GameObject.FindGameObjectWithTag("Player").transform;//target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
    }

    protected override void Start()
    {
        base.Start();

        if (hasTarget)//因为之后敌人生成时，很可能Player已经阵亡了，首先要判断限制
        {
            currentState = State.Chasing;
            target.GetComponent<LivingEntity>().onDeath += TargetDeath;//target.GetComponent<PlayerController>().onDeath += TargetDeath;

            StartCoroutine(UpdatePath());
        }
    }
    private void Update()
    {
        //navMeshAgent.SetDestination(target.position);//OPTIONAL 锲而不舍类


        StartCoroutine(TryAttack());

    }
    IEnumerator TryAttack()
    {


        Vector3 originalPos = transform.position;
        Vector3 attackPos = target.position;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        if (Physics.Raycast(ray, out hitInfo, 10, PlayerMask))
        {
            Debug.Log("attacking!");
            currentState = State.Attacking;
            navMeshAgent.enabled = false;//保证在【正在攻击的状态】下不会出现继续寻路的情况
            timer += Time.deltaTime;
            if (timer > fireRate)
            {
                timer = 0;
                GameObject spawnProjectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0, 180, 0));


            }

        }

        yield return null;
        currentState = State.Chasing;
        navMeshAgent.enabled = true;
    }
    IEnumerator UpdatePath()//OPTIONAL 追人类
    {

        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                if (isDead == false)
                {
                    Vector3 preTargetPos = new Vector3(target.position.x, 0, target.position.z);
                    navMeshAgent.SetDestination(preTargetPos);
                }
            }
            yield return new WaitForSeconds(updateRate);
        }
    }
    private void TargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }
}
