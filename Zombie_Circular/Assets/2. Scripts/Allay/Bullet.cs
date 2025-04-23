using UnityEngine;

/// <summary>
/// 총알 - 닿았을 때 데미지 전달
/// BulletDamage 주입 필수
/// </summary>
public class Bullet : MonoBehaviour
{
    public float BulletDamage {get; set;}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Zombie")|| collision.gameObject.CompareTag("Ground"))
            ObjectPoolManager.ReturnObjectPool(gameObject);
    }
}