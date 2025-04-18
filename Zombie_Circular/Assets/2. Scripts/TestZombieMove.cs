using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestZombieMove : MonoBehaviour
{
    enum ZombieState
    { 
        Run,
        Stop,
        IsPushed,
        Jump
    }

    [SerializeField] private float jumpForce = 1f;  // 점프 시 힘
    [SerializeField] private float moveSpeed = 1f;  // 달릴 시 힘
    public float rayDistance = 1f;
    public LayerMask groundLayer;
    public LayerMask zombieLayer;
    private Rigidbody2D rb;
    private ZombieState state;
    private RaycastHit2D downHit;
    

    private void Awake()
    {
        Physics2D.queriesStartInColliders = false;
        rb = GetComponent<Rigidbody2D>();
        state = ZombieState.Run;
    }

    private void FixedUpdate()
    {
        var col = GetComponent<Collider2D>();
        Vector2 origin = (Vector2)col.bounds.center
                       + Vector2.left * (col.bounds.extents.x);

        RaycastHit2D frontHit = Physics2D.Raycast(
            origin, Vector2.left, rayDistance,
            groundLayer | zombieLayer);
        Debug.DrawLine(origin, origin + Vector2.left * rayDistance, Color.green, 0.1f);

        if (frontHit.collider != null)
        {
            // 앞에 장애물이 있으면 정지
            Debug.Log(frontHit.collider.tag);
            state = ZombieState.Stop;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            if (state == ZombieState.IsPushed)
                return;
            // 앞에 아무것도 없으면 전진
            Debug.Log("넌 이제 자유다");
            state = ZombieState.Run;
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y); // 왼쪽으로 이동
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tower"))
        {
            downHit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, groundLayer | zombieLayer);
            Debug.DrawLine(transform.position, transform.position + Vector3.down * rayDistance, Color.red, 60f);
            if (downHit.collider != null && downHit.collider.CompareTag("Ground"))
            {
                if(state != ZombieState.Stop)
                    state = ZombieState.Stop;
            }
            else if (downHit.collider != null && downHit.collider.CompareTag("Monster"))
            {
                Debug.Log($"{this.name} 밑에 몬스터~!");
                if (state!= ZombieState.Run)
                {
                    state = ZombieState.Run;
                }
            }
        }
        else if (collision.gameObject.CompareTag("Monster"))
        {
            // 왼쪽(앞쪽)일 시 점프
            ContactPoint2D contact = collision.contacts[0];
            Vector2 normal = contact.normal;

            if (Vector2.Dot(normal, Vector2.right) > 0.9f)
            {
                if (state == ZombieState.Jump)
                    return;
                state = ZombieState.Jump;
                Debug.Log($"{this.name} jump!");
                Vector2 impulse = Vector2.up * jumpForce;
                rb.AddForce(impulse, ForceMode2D.Impulse);
            }
        }
    }
}
