using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChecker : MonoBehaviour
{
    private Rigidbody2D RB;

    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        //ray cast 
    }

    // 우선순위 

    // 앞에 tower 있다면 attack

    // 앞에 zombie 있다면
        // 뒤에 zombie 있는 체크
        // 있다면 idle
        // 없다면 jump

    // 앞에 비어있다면 run

    // 그러나, 위에서 내려오는 zombie가 있다면 idle 
}