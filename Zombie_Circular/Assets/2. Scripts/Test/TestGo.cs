using UnityEngine;

public class TestGo : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float downForce = 5f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // 물리 연산은 FixedUpdate에서
    private void FixedUpdate()
    {
        // 1) 오른쪽 이동 속도 고정
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        // 2) 아래로 지속적인 힘 추가
        rb.AddForce(Vector2.down * downForce, ForceMode2D.Force);
    }
}
