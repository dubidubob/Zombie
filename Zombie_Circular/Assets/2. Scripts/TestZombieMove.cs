using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestZombieMove : MonoBehaviour
{
    [SerializeField] private float jumpForce = 1f;  // 점프 시 힘
    [SerializeField] private float pushRightForce = 1f;  // 점프 시 오른쪽 힘
    [SerializeField] private float moveSpeed = 1f;  // 달릴 시 힘
    private Rigidbody2D rb;
    private bool canRun = true;

    private bool isBeingPushed = false;
    private float pushThreshold = 1.0f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 달리기
        if (canRun && !isBeingPushed)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
    }

    private void FixedUpdate()
    {
        float externalForceX = rb.velocity.x - moveSpeed;

        if (externalForceX > pushThreshold)
        {
            // 오른쪽으로 밀리는 힘이 감지됨
            isBeingPushed = true;
            canRun = false;
        }
        else
        {
            isBeingPushed = false;
            canRun = true;
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
                Vector2 impulse = Vector2.up * jumpForce
                        + Vector2.right * pushRightForce;
                rb.AddForce(impulse, ForceMode2D.Impulse);
            }
            canRun = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DisableRun();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Invoke(nameof(EnableRun), 1f);
    }

    private void EnableRun()
    {
        isBeingPushed = false;
        canRun = true;
    }

    private void DisableRun()
    {
        isBeingPushed = true;
        canRun = false;
    }
}
