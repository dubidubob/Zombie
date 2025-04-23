using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Tag "Zombie"�� ���� �������� �ް�, ��ġ �����ϴ� Allay(Box, Hero) ���� Ŭ����
/// TODO : ��ġ ���� / ������ ���ú� å�� �и�
/// </summary>
public class Damageable : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Transform trucktCenterPos;
    [SerializeField] private float xOffSet;
    public float CurrentHealth { get; set; }

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (hpSlider == null)
        {
            hpSlider = GetComponentInChildren<Slider>();
        }
        hpSlider.maxValue = MaxHealth;
        hpSlider.value = hpSlider.maxValue;
        CurrentHealth = MaxHealth;

        if (trucktCenterPos == null)
        {
            Transform child = transform.Find("CenterPoint");
            trucktCenterPos = child.transform;
        }
    }

    #region HP/Damage
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Zombie"))
        {
            var pawn = collision.gameObject.GetComponentInParent<Zombie>();
            if (pawn != null)
            {
                Damage(pawn.MyDamage);
            }
        }
    }

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
        gameObject.SetActive(false);
    }

    private void UpdateHpPanel()
    {
        hpSlider.value = Mathf.Clamp((float)CurrentHealth, 0f, MaxHealth);
    }

    #endregion

    // ��ġ ����
    private void FixedUpdate()
    {
        this.transform.position = new Vector2(trucktCenterPos.position.x+ xOffSet, this.transform.position.y);
    }
}
