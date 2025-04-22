using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    [SerializeField] private Slider HpSlider;
    [SerializeField] private Transform CenterPos;
    public float CurrentHealth { get; set; }

    private void Awake()
    {
        if (HpSlider == null)
        {
            HpSlider = GetComponentInChildren<Slider>();
        }
        HpSlider.maxValue = MaxHealth;
        HpSlider.value = HpSlider.maxValue;
        CurrentHealth = MaxHealth;
        
        if (CenterPos == null)
        {
            Transform child = transform.Find("CenterPoint");
            CenterPos = child.transform;
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
        HpSlider.value = Mathf.Clamp((float)CurrentHealth, 0f, MaxHealth);
    }
    #endregion

    // 위치 고정
    private void FixedUpdate()
    {
        this.transform.position = new Vector2(CenterPos.position.x, this.transform.position.y);
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
