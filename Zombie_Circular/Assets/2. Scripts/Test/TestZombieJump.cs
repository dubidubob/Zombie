using UnityEngine;

public class TestZombieJump : MonoBehaviour
{
    public float jumpForce = 1f;
    [SerializeField] private float rayDistance = 0.5f;
    private Rigidbody2D rb;
    private Collider2D myCollider2D;
    private Bounds bounds;
    private bool canJump = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider2D = GetComponent<Collider2D>();
        rayDistance += (bounds.max.x - bounds.min.x) / 2;
    }

    private void FixedUpdate()
    {
        bounds = myCollider2D.bounds;
        Vector2 rayOrigin = new Vector2(bounds.max.x, bounds.max.y);
        RaycastHit2D hitBack = Physics2D.Raycast(rayOrigin, Vector2.right, rayDistance);
        Debug.DrawRay(rayOrigin, Vector2.right * hitBack.distance, Color.red);

        if (canJump && hitBack.collider == null)
        {
            jump();
            canJump = false;
        }
        else if (hitBack.collider != null)
        { 
            canJump = false;
        }
        
    }

    private void jump()
    {
        Debug.Log($"{this.name} jump!");
        Vector2 impulse = Vector2.up * jumpForce;
        rb.AddForce(impulse, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        HandleCollision(collision);
    }

    private void HandleCollision(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            // 왼쪽(앞쪽)일 시 점프
            ContactPoint2D contact = collision.contacts[0];
            Vector2 normal = contact.normal;

            if (Vector2.Dot(normal, Vector2.right) > 0.9f)
            {
                canJump = true;
            }
        }
    }
}
