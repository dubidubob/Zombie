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
        CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }
    public void Die()
    {
        //TODO : Object Pooling
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