using System.Collections;
using UnityEngine;

/// <summary>
/// Tag : Zombie를 지정 범위 내에서 찾아 총을 쏜다.
/// Bullet Spawner
/// TODO : Weapon base 제작
/// </summary>
public class Weapon : MonoBehaviour
{
    [Header("Detection Area")]
    [SerializeField] private Transform detectionCenter;
    [SerializeField] private float detectionWidth;
    [SerializeField] private float detectionHeight;
    [SerializeField] private LayerMask zombieLayers;

    [Header("Weapon Owning Data")]
    [SerializeField] private Transform shootingPlace;
    [SerializeField] private float shootingForce = 1f;
    [SerializeField] private float shootingCoolTime = 1f;
    [SerializeField] private float shootingtAngleRange = 15;
    [SerializeField] private int bulletNumber = 5;

    [Header("Bullet Owning Data")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletDamage;

    private float m_targetAngle;

    void Start()
    {
        StartCoroutine(Shooting());
    }

    private void Update()
    {
        RotateWeapon(m_targetAngle);
    }

    private IEnumerator Shooting()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootingCoolTime);
            ShootRoutine();
        }
    }

    private void ShootRoutine()
    {
        Transform target = FindNearestZombie();
        if(target == null) return;

        Vector2 shootingDir = Aim(target);
        Shoot(shootingDir);
    }

    private Transform FindNearestZombie()
    {
        
        Collider2D[] zombiesInDetectionArea = Physics2D.OverlapBoxAll(new Vector2(detectionCenter.position.x, detectionCenter.position.y),
                                                                new Vector2(detectionWidth, detectionHeight),
                                                                0f,
                                                                zombieLayers);
        if (zombiesInDetectionArea.Length == 0)
        {
            //Debug.Log("Nothing To Spawn");
            return null;
        }

        Transform nearestZombie = null;
        float minXPosition = float.MaxValue;

        // x위치가 가장 작은 좀비 찾기
        foreach (Collider2D zombieCollider in zombiesInDetectionArea)
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
        
        return nearestZombie;
    }

    private Vector2 Aim(Transform originTarget)
    {
        // 포물선 고려, 미세조정
        Vector2 target = new Vector2(originTarget.position.x + 0.5f, originTarget.position.y);
        Vector2 shootingDir = target - (Vector2)shootingPlace.position;
        shootingDir.Normalize();

        m_targetAngle = Mathf.Atan2(shootingDir.y, shootingDir.x) * Mathf.Rad2Deg;

        return shootingDir;
    }

    private void RotateWeapon(float angle)
    {
        Quaternion currentRot = transform.rotation;
        Quaternion desiredRot = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Lerp(
            currentRot,
            desiredRot,
            Time.deltaTime * shootingCoolTime * 3f
        );
    }

    // 쏘기 : 모든 bullet의 발사 각도가 조금은 랜덤
    private void Shoot(Vector2 dir)
    {
        float halfAngleRange = shootingtAngleRange * 0.5f;

        for (int cnt = 0; cnt < bulletNumber; cnt++)
        {
            float randomAngle = Random.Range(-halfAngleRange, halfAngleRange);

            Vector2 ShootingDir = Quaternion.Euler(0, 0, randomAngle) * dir;
            GameObject go = ObjectPoolManager.SpawnObject(
                bulletPrefab,
                shootingPlace.position, 
                Quaternion.identity);
            go.GetComponent<Bullet>().BulletDamage = bulletDamage; // Bullet 데미지 설정

            Vector2 impulse = ShootingDir * shootingForce;
            go.GetComponent<Rigidbody2D>().AddForce(impulse, ForceMode2D.Impulse);
        }
    }
}
