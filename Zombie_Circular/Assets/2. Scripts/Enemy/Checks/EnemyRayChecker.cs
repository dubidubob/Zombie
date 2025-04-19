using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyRayChecker : MonoBehaviour
{
    [SerializeField] private float frontRayDist = 0.1f;
    [SerializeField] private float backRayDist = 0.1f;
    [SerializeField] private float upRayDist = 0.1f;

    private Enemy m_Enemy;
    private Collider2D m_Collider2D;
    private float m_DelayOffset = 0f;
    private int rayLayerMask;

    private void Awake()
    {
        Physics2D.queriesStartInColliders = false;

        m_Enemy = GetComponent<Enemy>();
        m_Collider2D = GetComponent<Collider2D>();
        rayLayerMask = LayerMask.GetMask("Default", "Enemy");
    }

    private void FixedUpdate()
    {
        Bounds bounds = m_Collider2D.bounds;
        CastFrontRay(bounds);
        CastBackRay(bounds);
        CastUpRay(bounds);
    }
   

    private void CastFrontRay(Bounds bounds)
    {
        Vector2 rayFront = new Vector2(bounds.min.x-m_DelayOffset, (bounds.max.y+bounds.min.y)/2);
        RaycastHit2D hitFront = Physics2D.Raycast(rayFront, Vector2.left, frontRayDist, rayLayerMask);
        Debug.DrawRay(rayFront, Vector2.left * hitFront.distance, Color.black);

        if (hitFront.collider == null)
        {
            //Debug.Log("앞에 잇는데 왜?");
            if (m_Enemy.IsTowerFront) m_Enemy.SetTowerFront(false);
            if (m_Enemy.IsZombieFront) m_Enemy.SetZombieFront(false);
        }
        else
        {
            //Debug.Log($"{this.name}, {hitFront.collider.name} 이 놈이에여 ㅜㅜ");
            if (hitFront.collider.gameObject.CompareTag("Tower"))
            {
                if (!m_Enemy.IsTowerFront) 
                    m_Enemy.SetTowerFront(true);
            }
            else if (hitFront.collider.gameObject.CompareTag("Monster"))
            {
                m_Enemy.TargetTransform =hitFront.transform;
                if (!m_Enemy.IsZombieFront) m_Enemy.SetZombieFront(true);
            }
        }
    }

    private void CastBackRay(Bounds bounds)
    {
        Vector2 rayBack = new Vector2(bounds.max.x - m_DelayOffset, bounds.max.y);
        RaycastHit2D hitBack = Physics2D.Raycast(rayBack, Vector2.right, backRayDist, rayLayerMask);
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

    private void CastUpRay(Bounds bounds)
    {
        Vector2 rayUp = new Vector2(bounds.min.x, bounds.max.y);
        RaycastHit2D hitBack = Physics2D.Raycast(rayUp, Vector2.up, upRayDist, rayLayerMask);
        Debug.DrawRay(rayUp, Vector2.up * hitBack.distance, Color.blue);

        if (hitBack.collider == null)
        {
            if (m_Enemy.IsZombieUp) m_Enemy.SetZombieUp(false);
        }
        else if (hitBack.collider.gameObject.CompareTag("Monster"))
        {
            if (!m_Enemy.IsZombieUp) m_Enemy.SetZombieBack(true);
        }
    }
}