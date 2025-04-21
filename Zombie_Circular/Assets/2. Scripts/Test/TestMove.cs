using UnityEngine;

/// <summary>
/// 간단 테스트용 컨트롤러:
/// A 키 = 왼쪽, D 키 = 오른쪽, Space 키 = 점프
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class TestMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;


    private Rigidbody2D rb;
    private bool isGrounded;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 1. 수평 이동 처리
        float horizontalInput = 0f;
        if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
        if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        // 2. 점프 처리 (바닥에 있을 때만)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    // 간단한 바닥 감지: 충돌 시작 시 바닥 판정
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
    }

    // 바닥에서 떨어지면 점프할 수 있음
    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}
