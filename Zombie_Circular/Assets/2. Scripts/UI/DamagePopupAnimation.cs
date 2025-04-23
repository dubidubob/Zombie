using TMPro;
using UnityEngine;
/// <summary>
/// UI Damage Animation
/// </summary>
public class DamagePopupAnimation : MonoBehaviour
{
    [Header("Curves")] 
    public AnimationCurve opacityCurve; // UI 투명도
    public AnimationCurve scaleCurve; // UI 사이즈
    public AnimationCurve heightCurve; // UI 높이 위치

    private TextMeshProUGUI m_text;
    private float m_time = 0;
    private float m_heightOffSet = 1f;
    private Vector2 m_originPos;

    void Awake()
    {
        m_text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        m_originPos = transform.position;
    }

    void Update()
    {
        AdjustUI();
        m_time +=Time.deltaTime;
    }

    private void AdjustUI()
    {
        m_text.color = new Color(1, 1, 1, opacityCurve.Evaluate(m_time));
        transform.localScale = Vector2.one * scaleCurve.Evaluate(m_time);
        transform.localPosition = m_originPos + new Vector2(0, heightCurve.Evaluate(m_time) + m_heightOffSet);
    }
}
