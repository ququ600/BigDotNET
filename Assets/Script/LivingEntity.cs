using UnityEngine;
using System;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float maxHealth;
    public float health { get; protected set; }
    protected bool isDead;

    public event Action onDeath;

    protected virtual void Start()
    {
        health = maxHealth;
    }

    [ContextMenu("Self Destruct")]
    protected void Die()
    {
        isDead = true;
        Destroy(gameObject);
        Score.score += 100;
        if (onDeath != null)
            onDeath();
    }

    public virtual void TakeHit(float _damageAmount, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakenDamage(_damageAmount);
    }

    public virtual void TakenDamage(float _damageAmount)
    {
        health -= _damageAmount;

        if (health <= 0 && isDead == false)
            Die();
    }
}
