using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform FirePlace;
    [SerializeField] private Transform FindCenter;
    [SerializeField] private float detectionWidth;
    [SerializeField] private float detectionHeight;
    [SerializeField] private LayerMask zombieLayer;

    [SerializeField] private float FireCoolTime = 1f;
    [SerializeField] private int BulletNumber = 5;
    [SerializeField] private float shootingForce=1f;
    [SerializeField] private float bulletAngle;

    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletDamage;
    
    private void Start()
    {
        StartCoroutine(Shooting());
    }

    private IEnumerator Shooting()
    {
        // 무한 반복
        while (true)
        {
            yield return new WaitForSeconds(FireCoolTime);
            ShootRoutine();
        }
    }

    private void ShootRoutine()
    {
        Transform target = FindNearestZombie();
        if(target == null) return;

        Vector2 dir = Aim(target);
        Shoot(dir);
    }

    // 영역 가장 가까이에 있는 좀비를 찾기
    private Transform FindNearestZombie()
    {
        // 충돌 영역 내에 있는 모든 좀비를 찾음        
        Collider2D[] zombieColliders = Physics2D.OverlapBoxAll(new Vector2(FindCenter.position.x, FindCenter.position.y),
                                                                new Vector2(detectionWidth, detectionHeight),
                                                                0f,
                                                                zombieLayer);
        // 아무 좀비도 발견되지 않았으면 null 반환
        if (zombieColliders.Length == 0)
        {
            Debug.Log("슈팅할 게 없는뎁쇼?");
            return null;
        }
            

        Transform nearestZombie = null;
        float minXPosition = float.MaxValue;

        // 모든 충돌체를 검사하여 x 위치가 가장 작은(가장 왼쪽) 좀비 찾기
        foreach (Collider2D zombieCollider in zombieColliders)
        {
            if (zombieCollider.CompareTag("Zombie"))
            {
                float zombieXPos = zombieCollider.transform.position.x;

                if (zombieXPos < minXPosition)
                {
                    minXPosition = zombieXPos;
                    nearestZombie = zombieCollider.transform;
                }
            }
        }
        //Debug.Log($"{nearestZombie} 쏘쇼");
        // 가장 왼쪽에 있는 좀비의 transform 반환
        return nearestZombie;
    }

    private Vector2 Aim(Transform originTarget)
    {
        Vector2 target = new Vector2(originTarget.position.x + 0.5f, originTarget.position.y);
        // 타겟 방향 벡터
        Vector2 dir = target - (Vector2)FirePlace.position;
        dir.Normalize();

        // 각도
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Z축 회전 only, 빼도 됨
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        return dir;
    }


    // 쏘기 : 모든 bullet의 위치가 조금은 랜덤
    private void Shoot(Vector2 dir)
    {
        dir.Normalize();
        
        float half = bulletAngle * 0.5f;

        for (int cnt = 0; cnt < BulletNumber; cnt++)
        {
            float randomAngle = Random.Range(-half, half);

            Vector2 ShootingDir = Quaternion.Euler(0, 0, randomAngle) * dir;
            GameObject go = ObjectPoolManager.SpawnObject(
                bulletPrefab,
                FirePlace.position, 
                Quaternion.identity);
            go.GetComponent<Bullet>().BulletDamage = bulletDamage; // 데미지 설정
            Vector2 impulse = ShootingDir * shootingForce;
            go.GetComponent<Rigidbody2D>().AddForce(impulse, ForceMode2D.Impulse);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (FindCenter == null) return;

        // Gizmo 색상 설정
        Gizmos.color = Color.cyan;

        // 박스 센터와 크기
        Vector3 center = FindCenter.position;
        Vector3 size = new Vector3(detectionWidth, detectionHeight, 0f);

        // 2D OverlapBox와 같은 사각형 그리기
        Gizmos.DrawWireCube(center, size);
    }
#endif
}
