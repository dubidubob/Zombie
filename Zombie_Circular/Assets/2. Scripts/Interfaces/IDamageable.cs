/// <summary>
/// 다칠 수 있다면 해당 Interface 적용
/// </summary>
public interface IDamageable
{
    void Damage(float damageAmount);

    void Die();
    float MaxHealth { get; set; }
    float CurrentHealth { get; set; }
}
