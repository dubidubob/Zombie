using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 데미지 UI 스포너 / 싱글톤
/// </summary>
public class DamagePopupSpawner : MonoBehaviour
{
    public static DamagePopupSpawner Instance;
    public GameObject DamageUIPrefab;

    [SerializeField] float randomPosRange =0.1f;
    [SerializeField] float textDuration = 0.8f;
    private void Start()
    {
        Instance = this;   
    }

    public void CreatePopup(Vector2 originPos, string damageText)
    {
        Vector2 pos = new Vector2(originPos.x + Random.Range(0f, randomPosRange), originPos.y + Random.Range(0f, randomPosRange));
        GameObject popup = ObjectPoolManager.SpawnObject(
            DamageUIPrefab,
            pos,
            Quaternion.identity
            );
        var temp = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        temp.text = damageText;

        Despawn(popup, textDuration);
    }

    private IEnumerator Despawn(GameObject go, float delay)
    {
        ObjectPoolManager.ReturnObjectPool(go);
        yield return new WaitForSeconds(delay);
    }
}
