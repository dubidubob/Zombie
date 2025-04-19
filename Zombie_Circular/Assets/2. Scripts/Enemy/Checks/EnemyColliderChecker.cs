using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyColliderChecker : MonoBehaviour
{
    [SerializeField] private float frontRayDist = 1f;
    [SerializeField] private float backRayDist = 1f;
    [SerializeField] private float upRayDist = 1f;

    private Enemy m_Enemy;
    private Collider2D m_Collider2D;
    private float m_RayOffsetX;
    private float m_RayOffsetY;
    
    private void Awake()
    {
        Physics2D.queriesStartInColliders = false;

        m_Enemy = GetComponent<Enemy>();
        m_Collider2D = GetComponent<Collider2D>();

        Bounds bounds = m_Collider2D.bounds;
        m_RayOffsetX = (bounds.max.x - bounds.min.x) / 2;
        m_RayOffsetY = (bounds.max.y - bounds.min.y) / 2;
    }

    private void FixedUpdate()
    {
        CastFrontRay();
        CastBackRay();
        CastUpRay();
    }

    private void CastFrontRay()
    {
        Vector2 rayFront = new Vector2(transform.position.x - m_RayOffsetX, transform.position.y - m_RayOffsetY);
        RaycastHit2D hitFront = Physics2D.Raycast(rayFront, Vector2.left, frontRayDist);
        Debug.DrawRay(rayFront, Vector2.left * hitFront.distance, Color.red);

        if (hitFront.collider == null)
        {
            if (m_Enemy.IsTowerFront) m_Enemy.SetTowerFront(false);
            if (m_Enemy.IsZombieFront) m_Enemy.SetZombieFront(false);
        }
        else
        {
            if (hitFront.collider.gameObject.CompareTag("Tower"))
            {
                if (!m_Enemy.IsTowerFront) m_Enemy.SetTowerFront(true);
            }
            else if (hitFront.collider.gameObject.CompareTag("Monster"))
            {
                if (!m_Enemy.IsZombieFront) m_Enemy.SetZombieFront(true);
            }
        }
    }

    private void CastBackRay()
    {
        Vector2 rayBack = new Vector2(transform.position.x + m_RayOffsetX, transform.position.y + m_RayOffsetY);
        RaycastHit2D hitBack = Physics2D.Raycast(rayBack, Vector2.right, backRayDist);
        Debug.DrawRay(rayBack, Vector2.right * hitBack.distance, Color.blue);

        if (hitBack.collider == null)
        {
            if (m_Enemy.IsZombieBack) m_Enemy.SetZombieBack(false);
        }
        else if (hitBack.collider.gameObject.CompareTag("Monster"))
        {
            if (!m_Enemy.IsZombieBack) m_Enemy.SetZombieBack(true);
        }
    }

    private void CastUpRay()
    {
        Vector2 rayUp = new Vector2(transform.position.x - m_RayOffsetX, transform.position.y + m_RayOffsetY);
        RaycastHit2D hitUp = Physics2D.Raycast(rayUp, Vector2.left, upRayDist);
        Debug.DrawRay(rayUp, Vector2.left * hitUp.distance, Color.yellow);

        if (hitUp.collider == null)
        {
            if (m_Enemy.IsZombieUp) m_Enemy.SetZombieUp(false);
        }
        else if (hitUp.collider.gameObject.CompareTag("Monster"))
        {
            if (!m_Enemy.IsZombieUp) m_Enemy.SetZombieUp(true);
        }
    }
}