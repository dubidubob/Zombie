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
        // ���� �ݺ�
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

    // ���� ���� �����̿� �ִ� ���� ã��
    private Transform FindNearestZombie()
    {
        // �浹 ���� ���� �ִ� ��� ���� ã��        
        Collider2D[] zombieColliders = Physics2D.OverlapBoxAll(new Vector2(FindCenter.position.x, FindCenter.position.y),
                                                                new Vector2(detectionWidth, detectionHeight),
                                                                0f,
                                                                zombieLayer);
        // �ƹ� ���� �߰ߵ��� �ʾ����� null ��ȯ
        if (zombieColliders.Length == 0)
        {
            Debug.Log("������ �� ���µ���?");
            return null;
        }
            

        Transform nearestZombie = null;
        float minXPosition = float.MaxValue;

        // ��� �浹ü�� �˻��Ͽ� x ��ġ�� ���� ����(���� ����) ���� ã��
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
        //Debug.Log($"{nearestZombie} ���");
        // ���� ���ʿ� �ִ� ������ transform ��ȯ
        return nearestZombie;
    }

    private Vector2 Aim(Transform originTarget)
    {
        Vector2 target = new Vector2(originTarget.position.x + 0.5f, originTarget.position.y);
        // Ÿ�� ���� ����
        Vector2 dir = target - (Vector2)FirePlace.position;
        dir.Normalize();

        // ����
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Z�� ȸ�� only, ���� ��
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        return dir;
    }


    // ��� : ��� bullet�� ��ġ�� ������ ����
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
            go.GetComponent<Bullet>().BulletDamage = bulletDamage; // ������ ����
            Vector2 impulse = ShootingDir * shootingForce;
            go.GetComponent<Rigidbody2D>().AddForce(impulse, ForceMode2D.Impulse);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (FindCenter == null) return;

        // Gizmo ���� ����
        Gizmos.color = Color.cyan;

        // �ڽ� ���Ϳ� ũ��
        Vector3 center = FindCenter.position;
        Vector3 size = new Vector3(detectionWidth, detectionHeight, 0f);

        // 2D OverlapBox�� ���� �簢�� �׸���
        Gizmos.DrawWireCube(center, size);
    }
#endif
}
