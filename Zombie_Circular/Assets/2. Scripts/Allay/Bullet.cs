using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float BulletDamage {get; set;}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Zombie")|| collision.gameObject.CompareTag("Ground"))
            ObjectPoolManager.ReturnObjectPool(gameObject);
    }
}