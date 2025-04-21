using UnityEngine;

public class TestSensor : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayerMask;
    private Collider2D m_myCollider;

    // 1) 센싱할 최대 콜라이더 수에 맞춰 버퍼 크기 결정
    private Collider2D[] buffer = new Collider2D[8];

    // 2) 캡슐 사이즈를 매 프레임 계산하거나, Awake에서 한 번 가져오기
    private Vector2 size;
    private void Awake()
    {
        m_myCollider = GetComponent<Collider2D>();
        // 캡슐 크기를 콜라이더 크기와 동일하게 설정
        size = m_myCollider.bounds.size;
    }

    private void FixedUpdate()
    {
        Vector2 center = m_myCollider.bounds.center;
        int count = Physics2D.OverlapCapsuleNonAlloc(
            center,
            size,
            CapsuleDirection2D.Vertical,
            0f,
            buffer,
            enemyLayerMask
        );

        bool hasUp = false, hasDown = false, hasLeft = false, hasRight = false;

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = buffer[i];
            if (hit == m_myCollider) continue;

            Vector2 dir = ((Vector2)hit.bounds.center - center).normalized;
            if (Vector2.Dot(dir, Vector2.up) > 0.7f) hasUp = true;
            if (Vector2.Dot(dir, Vector2.down) > 0.7f) hasDown = true;
            if (Vector2.Dot(dir, Vector2.left) > 0.7f) hasLeft = true;
            if (Vector2.Dot(dir, Vector2.right) > 0.7f) hasRight = true;
        }

        // 이제 hasUp, hasDown, hasLeft, hasRight를 이용해 로직 처리
        Debug.Log($"hasUp : {hasUp} down : {hasDown}, left : {hasLeft}, right : {hasRight}");
    }
}
