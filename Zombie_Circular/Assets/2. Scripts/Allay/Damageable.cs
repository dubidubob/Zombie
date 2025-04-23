using UnityEngine;
using UnityEngine.UI;

public class Damageable : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Transform centerPos;
    [SerializeField] private float xOffSet;
    public float CurrentHealth { get; set; }

    private void Awake()
    {
        if (hpSlider == null)
        {
            hpSlider = GetComponentInChildren<Slider>();
        }
        hpSlider.maxValue = MaxHealth;
        hpSlider.value = hpSlider.maxValue;
        CurrentHealth = MaxHealth;
        
        if (centerPos == null)
        {
            Transform child = transform.Find("CenterPoint");
            centerPos = child.transform;
        }
    }

    #region HP/Damage
    public void Damage(float damageAmount)
    {
        CurrentHealth -= damageAmount;

        UpdateHpPanel();

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }
    public void Die()
    {

        Destroy(gameObject);
    }

    private void UpdateHpPanel()
    {
        hpSlider.value = Mathf.Clamp((float)CurrentHealth, 0f, MaxHealth);
    }
    #endregion

    // 위치 고정
    private void FixedUpdate()
    {
        this.transform.position = new Vector2(centerPos.position.x+ xOffSet, this.transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HurtHero"))
        {
            var pawn = collision.gameObject.GetComponentInParent<Zombie>();
            Damage(pawn.MyDamage);
        }
    }
}
