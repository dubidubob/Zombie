using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopupAnimation : MonoBehaviour
{
    public AnimationCurve opacityCurve;
    public AnimationCurve scaleCurve;
    public AnimationCurve heightCurve;

    private TextMeshProUGUI tmp;
    private float time = 0;
    private Vector2 origin;

    private void Awake()
    {
        tmp = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        origin = transform.position;
    }
    void Update()
    {
        tmp.color = new Color(1, 1, 1, opacityCurve.Evaluate(time));
        transform.localScale = Vector2.one * scaleCurve.Evaluate(time);
        transform.localPosition = origin+new Vector2(0, 1+heightCurve.Evaluate(time));
        time+=Time.deltaTime;
    }
}
