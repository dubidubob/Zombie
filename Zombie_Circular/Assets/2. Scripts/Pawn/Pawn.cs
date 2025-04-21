using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; }
    #region Physics
    public Rigidbody2D RB { get; set; }
    #endregion

    private void Start()
    {
        CurrentHealth = MaxHealth;
        RB = GetComponent<Rigidbody2D>();
    }

    #region Health / Die

    public void Damage(float damageAmount)
    {
        OnDamage(damageAmount);
    }

    public void Die()
    {
        OnDie();
    }

    protected virtual void OnDamage(float damageAmount)
    {
        CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0f)
            OnDie();
    }

    protected virtual void OnDie()
    {
        // TODO: Object Pooling µî
        Destroy(gameObject);
    }

    #endregion

    #region Animation Triggers
    private void AnimationTriggerEvent(PawnStateType triggerType)
    {
        //TODO
    }

    
    public enum PawnStateType
    {
        Run,
        Attack,
        Stop
    }
    #endregion
}