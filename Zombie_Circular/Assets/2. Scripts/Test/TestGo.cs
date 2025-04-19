using UnityEngine;

public class TestGo : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float downForce = 5f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = 10;
    }

    // ���� ������ FixedUpdate����
    private void FixedUpdate()
    {
        // 1) ������ �̵� �ӵ� ����
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        // 2) �Ʒ��� �������� �� �߰�
        rb.AddForce(Vector2.down * downForce, ForceMode2D.Force);
    }
}
