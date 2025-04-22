using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class ZombieCollisionChecker : MonoBehaviour
{
    private Enemy m_Enemy;
    private int m_rayLayerMask;
    private Collider2D m_Collider2D;

    [SerializeField] private float frontRayDist = 0.1f;
    [SerializeField] private float backRayDist = 0.1f;

    private void Awake()
    {
        Physics2D.queriesStartInColliders = false;

        m_Enemy = GetComponent<Enemy>();
        m_Collider2D = GetComponent<Collider2D>();
        m_rayLayerMask = LayerMask.GetMask("Enemy");
    }

    private void Update()
    {
        Bounds bounds = m_Collider2D.bounds;
        CastFrontRay(bounds);
        CastBackRay(bounds);
    }

    private void CastFrontRay(Bounds bounds)
    {
        Vector2 rayFront = new Vector2(bounds.min.x, (bounds.max.y + bounds.min.y) / 2);
        RaycastHit2D hitFront = Physics2D.Raycast(rayFront, Vector2.left, frontRayDist, m_rayLayerMask);
        Debug.DrawRay(rayFront, Vector2.left * hitFront.distance, Color.black);

        if (hitFront.collider == null)
        {
            //Debug.Log("앞에 잇는데 왜?");
            if (m_Enemy.IsZombieFront) m_Enemy.SetZombieFront(false);
        }
        else
        {
            int layer = hitFront.collider.gameObject.layer;
            if (((1 << layer) & m_rayLayerMask) != 0) 
            {
                if (!m_Enemy.IsZombieFront)
                {
                    m_Enemy.TargetTransform = hitFront.transform;
                    Debug.Log($"{m_Enemy.TargetTransform.name}이 앞에 있어요~");
                    m_Enemy.SetZombieFront(true);
                }
            }
        }
    }

    private void CastBackRay(Bounds bounds)
    {
        Vector2 rayBack = new Vector2(bounds.max.x, bounds.max.y);
        RaycastHit2D hitBack = Physics2D.Raycast(rayBack, Vector2.right, backRayDist, m_rayLayerMask);
        Debug.DrawRay(rayBack, Vector2.right * hitBack.distance, Color.blue);

        if (hitBack.collider == null)
        {
            if (m_Enemy.IsZombieBack) m_Enemy.SetZombieBack(false);
        }
        else if (hitBack.collider.gameObject.CompareTag("Monster"))
        {
            Debug.Log($"{this.name}, {hitBack.transform.name}이 뒤에 있어요.");
            if (!m_Enemy.IsZombieBack) m_Enemy.SetZombieBack(true);
        }
    }
}
