using System.Collections;
using TMPro;
using UnityEngine;

public class DamagePopupSpawner : MonoBehaviour
{
    public static DamagePopupSpawner Instance;
    public GameObject DamageUIPrefab;
    [SerializeField] float randomPosRange =0.1f;
    private void Start()
    {
        Instance = this;   
    }

    public void CreatePopup(Vector2 originPos, string text)
    {
        Vector2 pos = new Vector2(originPos.x + Random.Range(0f, randomPosRange), originPos.y + Random.Range(0f, randomPosRange));
        GameObject popup = ObjectPoolManager.SpawnObject(
            DamageUIPrefab,
            pos,
            Quaternion.identity
            );
        var temp = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        temp.text = text;

        SetAcitveFalse(popup, 1f);
    }

    private IEnumerator SetAcitveFalse(GameObject go, float delay)
    {
        ObjectPoolManager.ReturnObjectPool(go);
        yield return new WaitForSeconds(delay);
    }
}
