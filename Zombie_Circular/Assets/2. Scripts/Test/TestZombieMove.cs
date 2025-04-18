using Unity.VisualScripting;
using UnityEngine;

public class TestZombieMove : MonoBehaviour
{
    [SerializeField] private float jumpForce = 1f;  // 점프 시 힘
    [SerializeField] private float moveSpeed = 1f;  // 달릴 시 힘
    [SerializeField] private float rayDistance = 0.5f;
    [SerializeField] private float pusingthresdhold = 0f;

    public LayerMask groundLayer;
    public LayerMask zombieLayer;
    
    private Rigidbody2D rb;
    private RaycastHit2D downHit;
    private Bounds bounds;
    private Collider2D myCollider2D;

    private Transform frontTransform = null;
    private Vector3 lastFrontTransformPos;
    private bool isInGroup = false;
    private bool canJump = false;

    private void Awake()
    {
        Physics2D.queriesStartInColliders = false;
        rb = GetComponent<Rigidbody2D>();
        myCollider2D = GetComponent<Collider2D>();
        bounds = myCollider2D.bounds;
        rayDistance += (bounds.max.x - bounds.min.x) / 2;
    }

    private void FixedUpdate()
    {
        if (!isInGroup)
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);

        if (isInGroup && frontTransform != null)
        {
            Vector3 delta = frontTransform.position - lastFrontTransformPos;
            transform.position += delta;
            lastFrontTransformPos = frontTransform.position;
        }

        bounds = myCollider2D.bounds;
        Transform trans = checkFront();
        if (trans!=null)
        {
            checkBack(trans);
        }
    }

    private void checkBack(Transform trans)
    {
        Vector2 rayOrigin = new Vector2(bounds.max.x, bounds.max.y);
        RaycastHit2D hitBack = Physics2D.Raycast(rayOrigin, Vector2.right, rayDistance);
        Debug.DrawRay(rayOrigin, Vector2.right * hitBack.distance, Color.red);

        if (canJump && hitBack.collider == null)
        {
            Detach(trans);
            jump();
            canJump = false;
        }
        else if (hitBack.collider != null)
        {
            Attach(trans);
            canJump = false;
        }
    }

    private Transform checkFront()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x, bounds.max.y);

        RaycastHit2D hitFront = Physics2D.Raycast(rayOrigin, Vector2.left, rayDistance);
        Debug.DrawRay(rayOrigin, Vector2.left * hitFront.distance, Color.red);

        if (hitFront.collider == null)
            return null;

        if (hitFront.collider.gameObject.CompareTag("Monster"))
        {
            return hitFront.collider.transform;
        }

        return null;
    }

    // 뒤가 비었거나, 앞이 비었을 때
    private void Detach(Transform otherTransform)
    {
        if (otherTransform == frontTransform)
        {
            frontTransform = null;
            isInGroup = false;
        }
    }

    // 앞과 뒤 모두 꽉 차있을 때, attach
    private void Attach(Transform otherTransform)
    {
        frontTransform = otherTransform;
        lastFrontTransformPos = frontTransform.position;
        isInGroup = true;
    }

    // 앞과 처음 충돌났을 때, 점프!
    void OnCollisionEnter2D(Collision2D collision)
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

    private void jump()
    {
        Debug.Log($"{this.name} jump!");
        Detach(frontTransform);
        Vector2 impulse = Vector2.up * jumpForce;
        rb.AddForce(impulse, ForceMode2D.Impulse);
    }
}
