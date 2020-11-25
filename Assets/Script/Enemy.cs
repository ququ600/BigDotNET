using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State { Idle, Chasing, Attacking };
    private NavMeshAgent navMeshAgent;
    private Transform target;

    #region Attack
    [SerializeField] private float attackDistanceThreshold = 2.5f;
    private float timeBtwAttack = 1.0f;

    private float nextAttackTime;
    public State currentState;
    public float damage = 1.0f;
    #endregion

    [SerializeField] private float updateRate;

    [SerializeField] private bool hasTarget;

    public ParticleSystem deathEffect;

    Material skinMaterial;
    Color originalColor;

    public static event Action onDeathStatic;

    private void Awake()
    {
        Debug.Log("awake");
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)//因为之后敌人生成时，很可能Player已经阵亡了，首先要判断限制
        {
            hasTarget = true;
            Debug.Log("hastarget");
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    protected override void Start()
    {
        base.Start();

        if (hasTarget)
        {
            currentState = State.Chasing;
            target.GetComponent<LivingEntity>().onDeath += TargetDeath;

            StartCoroutine(UpdatePath());
        }
    }


    private void Update()
    {


        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrtDstToTarget = (target.position - transform.position).sqrMagnitude;//两者距离的平方
                if (sqrtDstToTarget < Mathf.Pow(attackDistanceThreshold, 2))
                {
                    nextAttackTime = Time.time + timeBtwAttack;

                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 销毁当前游戏物体
        var name = collision.collider.name;
        Debug.Log(name);

        if (name == "player")
        {

            HitPlayer(damage, collision);
        }
    }
    private void HitPlayer(float damage, Collision collision)
    {
        IDamageable damageableObject = collision.collider.GetComponent<IDamageable>();
        if (damageableObject != null)
            damageableObject.TakenDamage(damage);
        //Destroy(gameObject);//击中敌人后，子弹销毁
        Debug.Log("Destroy");
    }
    IEnumerator UpdatePath()
    {
        Debug.Log("pathing");
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

    public override void TakeHit(float _damageAmount, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (_damageAmount >= health)
        {
            if (onDeathStatic != null)
            {
                onDeathStatic();
            }
            GameObject spawnEffect = Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection));
            Destroy(spawnEffect, deathEffect.startLifetime);//1.0f是PS的Start Lifetime数值
            Debug.Log("Deathhhhhhhhhh");
        }
        base.TakeHit(_damageAmount, hitPoint, hitDirection);
    }



}
