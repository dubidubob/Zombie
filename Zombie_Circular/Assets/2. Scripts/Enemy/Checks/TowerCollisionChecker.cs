using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class TowerCollisionChecker : MonoBehaviour
{
    [SerializeField] private float upRayDist = 0.1f;
    private Enemy m_Enemy;

    //TODO
    private int m_rayLayerMask;

    private void Awake()
    {
        Physics2D.queriesStartInColliders = false;

        m_Enemy = GetComponent<Enemy>();
        m_rayLayerMask = LayerMask.GetMask("Tower", "Enemy");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        #region Tower
        if (collision.gameObject.CompareTag("Tower"))
        {
            if (!m_Enemy.IsTowerFront)
                m_Enemy.SetTowerFront(true);
        }
        #endregion

        #region Enemy Front Back
        if (!collision.gameObject.CompareTag("Monster")) return;

        // 상대의 위치 기준으로 방향 계산
        Vector2 localPoint = transform.InverseTransformPoint(collision.transform.position);
        // localPoint.x < 0 → 상대가 내 왼쪽(앞쪽)에 있음
        if (localPoint.x < 0f)
        {
            if (!m_Enemy.IsZombieFront)
                m_Enemy.SetZombieFront(true);
        }
        else
        {
            if (!m_Enemy.IsZombieBack)
                m_Enemy.SetZombieBack(true);
        }
        #endregion
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        #region Tower
        if (collision.gameObject.CompareTag("Tower"))
        {
            if (m_Enemy.IsTowerFront)
                m_Enemy.SetTowerFront(false);
        }
        #endregion

        #region Enemy Front Back
        if (!collision.gameObject.CompareTag("Monster")) return;

        // Enter와 동일하게 방향 재계산
        Vector2 localPoint = transform.InverseTransformPoint(collision.transform.position);
        if (localPoint.x < 0f)
        {
            if (m_Enemy.IsZombieFront)
                m_Enemy.SetZombieFront(false);
        }
        else
        {
            if (m_Enemy.IsZombieBack)
                m_Enemy.SetZombieBack(false);
        }
        #endregion
    }
}
