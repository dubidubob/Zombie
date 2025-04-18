using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class TestZombieGroup : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("앞쪽 물체 감지를 위한 거리")]
    public float rayDistance = 1.5f;
    [Tooltip("감지할 레이어 (예: Monster)")]
    public LayerMask targetLayer;

    private Transform frontTarget;
    private Vector3 lastTargetPosition;
    private Rigidbody2D rb;
    private bool isFollowing = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // 1) 아직 따라갈 대상이 없으면 레이캐스트로 감지
        if (!isFollowing)
        {
            Bounds bounds = GetComponent<Collider2D>().bounds;
            Vector2 rayOrigin = new Vector2(transform.position.x, bounds.max.y);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.left, rayDistance, targetLayer);
            Debug.DrawRay(transform.position, Vector2.left * rayDistance, Color.cyan);

            if (hit.collider != null && hit.collider.CompareTag("Monster"))
            {
                SetTarget(hit.collider.transform);
            }
        }
        // 2) 이미 따라가는 중이면 위치 동기화 또는 중단 여부 체크
        else
        {
            if (frontTarget == null || !frontTarget.gameObject.activeInHierarchy)
            {
                StopFollowing();
            }
            else
            {
                Vector3 currentPos = frontTarget.position;
                Vector3 delta = currentPos - lastTargetPosition;
                rb.MovePosition(rb.position + new Vector2(delta.x, delta.y));
                lastTargetPosition = currentPos;
            }
        }
    }

    /// <summary>
    /// 앞쪽 물체를 따라가기 시작한다.
    /// </summary>
    public void SetTarget(Transform target)
    {
        Debug.Log($"{this.name}, {target.name}에 붙었더요");
        frontTarget = target;
        lastTargetPosition = frontTarget.position;
        isFollowing = true;
    }

    /// <summary>
    /// 따라가기를 중단하고 상태 초기화.
    /// </summary>
    public void StopFollowing()
    {
        Debug.Log($"{this.name}, 떨어졌더요");
        isFollowing = false;
        frontTarget = null;
    }
}
