using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class TestZombieGroup : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("���� ��ü ������ ���� �Ÿ�")]
    public float rayDistance = 1.5f;
    [Tooltip("������ ���̾� (��: Monster)")]
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
        // 1) ���� ���� ����� ������ ����ĳ��Ʈ�� ����
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
        // 2) �̹� ���󰡴� ���̸� ��ġ ����ȭ �Ǵ� �ߴ� ���� üũ
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
    /// ���� ��ü�� ���󰡱� �����Ѵ�.
    /// </summary>
    public void SetTarget(Transform target)
    {
        Debug.Log($"{this.name}, {target.name}�� �پ�����");
        frontTarget = target;
        lastTargetPosition = frontTarget.position;
        isFollowing = true;
    }

    /// <summary>
    /// ���󰡱⸦ �ߴ��ϰ� ���� �ʱ�ȭ.
    /// </summary>
    public void StopFollowing()
    {
        Debug.Log($"{this.name}, ����������");
        isFollowing = false;
        frontTarget = null;
    }
}
