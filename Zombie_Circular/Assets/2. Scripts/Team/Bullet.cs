using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float BulletDamage {get; set;}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Monster")|| collision.gameObject.CompareTag("Ground"))
            Destroy(gameObject);
    }
}