using UnityEngine;
/// <summary>
/// 움직임 가능한 Damageable
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Pawn : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; }
    public Rigidbody2D RB { get; set; }
    
    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    protected virtual void Init()
    {
        CurrentHealth = MaxHealth;
    }

    public void Damage(float damageAmount)=> OnDamage(damageAmount);
    public void Die() => OnDie();

    protected virtual void OnDamage(float damageAmount)
    {
        CurrentHealth -= damageAmount;
       
        if (CurrentHealth <= 0f)
            OnDie();
    }

    protected virtual void OnDie() { }
}