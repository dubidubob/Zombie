using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestZombieMove : MonoBehaviour
{
    [SerializeField] private float jumpForce = 1f;  // 점프 시 힘
    [SerializeField] private float moveSpeed = 1f;  // 달릴 시 힘
    private Rigidbody2D rb;
    private bool canRun = true;
    
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tower"))
        {
            canRun = false;
        }

        else if (collision.gameObject.CompareTag("Monster"))
        {
            // 왼쪽(앞쪽)일 시 점프
            ContactPoint2D contact = collision.contacts[0];
            Vector2 normal = contact.normal;

            if (Vector2.Dot(normal, Vector2.right) > 0.9f)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tower"))
        {
            Invoke(nameof(EnableRun), 1f);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            canRun = false;
        }
    }

    private void EnableRun()
    {
        canRun = true;
    }
}
