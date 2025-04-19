using UnityEngine;

public class TestZombiePush : MonoBehaviour
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
    [SerializeField] private float rayDistance = 0.5f;
    [SerializeField] private float pusingthresdhold = 0f;
    public LayerMask groundLayer;
    public LayerMask zombieLayer;
    private Rigidbody2D rb;
    private ZombieState state;
    private RaycastHit2D downHit;
    private Bounds bounds;
    private Collider2D myCollider2D;

    private Transform frontTransform = null;
    private Vector3 lastFrontTransformPos;
    private bool isInGroup = false;

    private void Awake()
    {
        Physics2D.queriesStartInColliders = false;
        rb = GetComponent<Rigidbody2D>();
        state = ZombieState.Run;
        myCollider2D = GetComponent<Collider2D>();
        bounds = myCollider2D.bounds;
        rayDistance += (bounds.max.x - bounds.min.x)/2;
    }

    private void Update()
    {
        //if (isInGroup && frontTransform != null)
        //{
        //    Vector3 delta = frontTransform.position - lastFrontTransformPos;
        //    transform.position += delta;
        //    lastFrontTransformPos = frontTransform.position;
        //}
    }


    private void FixedUpdate()
    {
        if (state == ZombieState.Run && !isInGroup)
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);

        #region 앞에 뭐가 있으면 멈춰!
        bounds = myCollider2D.bounds;
        Vector2 rayOrigin = new Vector2(transform.position.x, bounds.max.y);

        RaycastHit2D hitFront = Physics2D.Raycast(rayOrigin, Vector2.left, rayDistance);
        //Debug.DrawRay(rayOrigin, Vector2.left * hitFront.distance, Color.red);

        if (hitFront.collider != null)
        {
            //Debug.Log($"{this.name} {hitFront.collider.tag}에 부딪힘!");
            if (state != ZombieState.Stop)
            {
                state = ZombieState.Stop;
                //rb.velocity = new Vector2(0, rb.velocity.y);
                //if (hitFront.collider.gameObject.CompareTag("Monster"))
                //{
                //    frontTransform = hitFront.collider.transform;
                //    lastFrontTransformPos = frontTransform.position;
                //    isInGroup = true;
                //}
            }
        }
        else
        {
            //Debug.Log($"{this.name} 앞에 아무것도 없다");
            //Detach(frontTransform);
            if (state != ZombieState.Run)
            {
                Invoke("Run", 0.07f);
            }
        }
        #endregion
    }

    private void Detach(Transform otherTransform)
    {
        if (otherTransform == frontTransform)
        {
            frontTransform = null;
            isInGroup = false;
        }
    }

    private void Run()
    {
        state = ZombieState.Run;
    }
}
