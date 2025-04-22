using UnityEngine;

public class Pawn : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; }
    public Rigidbody2D RB { get; set; }
    public float MyDamage { get; set; }
    

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        MyDamage = 10;
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    #region Health / Die

    public void Damage(float damageAmount)=> OnDamage(damageAmount);
    public void Die() => OnDie();

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
}