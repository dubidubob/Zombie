using UnityEngine;

/// <summary>
/// ���� �׽�Ʈ�� ��Ʈ�ѷ�:
/// A Ű = ����, D Ű = ������, Space Ű = ����
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
        // 1. ���� �̵� ó��
        float horizontalInput = 0f;
        if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
        if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        // 2. ���� ó�� (�ٴڿ� ���� ����)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    // ������ �ٴ� ����: �浹 ���� �� �ٴ� ����
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
    }

    // �ٴڿ��� �������� ������ �� ����
    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}
