using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTowerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;  // 달릴 시 힘\
    private bool canRun = true;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 달리기
        if (canRun)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Monster"))
        {
            canRun = false;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Monster"))
        {
            Debug.Log($"{collider.gameObject.name} 나갔대요!");
            canRun = true;
            rb.constraints -= RigidbodyConstraints2D.FreezePositionX;
        }
    }
}
