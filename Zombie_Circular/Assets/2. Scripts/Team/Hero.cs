using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour, IDamageable
{
    #region 나중에 상속받을 거
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    [SerializeField] private Slider HpSlider;
    [SerializeField] private Transform CenterPos;
    public float CurrentHealth { get; set; }
    private void Awake()
    {
        if (HpSlider == null)
        {
            HpSlider = GetComponentInChildren<Slider>();
            HpSlider.value = 1;
        }
        if (CenterPos == null)
        {
            Transform child = transform.Find("CenterPoint");
            CenterPos = child.transform;
        }
        CurrentHealth = MaxHealth;
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
        //TODO
        Destroy(gameObject);
    }

    private void UpdateHpPanel()
    {
        HpSlider.value = Mathf.Clamp((float)CurrentHealth / MaxHealth, 0f, 1f);
    }
    #endregion

    // 위치 고정
    private void FixedUpdate()
    {
        this.transform.position = new Vector2(CenterPos.position.x, this.transform.position.y);
    }
    #endregion
}
