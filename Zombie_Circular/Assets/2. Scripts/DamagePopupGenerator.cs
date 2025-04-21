using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopupGenerator : MonoBehaviour
{
    public static DamagePopupGenerator Instance;
    public GameObject prefab;
    private void Start()
    {
        Instance = this;   
    }

    public void CreatePopup(Vector3 position, string text)
    { 
        var popup = Instantiate(prefab, position, Quaternion.identity);
        var temp = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        temp.text = text;

        // Destroy Timer
        Destroy(popup, 1f);
    }
}
