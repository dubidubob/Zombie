using UnityEngine;

public class TestSensor : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayerMask;
    private Collider2D m_myCollider;

    // 1) ������ �ִ� �ݶ��̴� ���� ���� ���� ũ�� ����
    private Collider2D[] buffer = new Collider2D[8];

    // 2) ĸ�� ����� �� ������ ����ϰų�, Awake���� �� �� ��������
    private Vector2 size;
    private void Awake()
    {
        m_myCollider = GetComponent<Collider2D>();
        // ĸ�� ũ�⸦ �ݶ��̴� ũ��� �����ϰ� ����
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

        // ���� hasUp, hasDown, hasLeft, hasRight�� �̿��� ���� ó��
        Debug.Log($"hasUp : {hasUp} down : {hasDown}, left : {hasLeft}, right : {hasRight}");
    }
}
