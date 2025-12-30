using UnityEngine;

public class EnemyCollider : MonoBehaviour
{
    public Enemy enemy; // Public yapıldı - Bullet kontrolü için
    void Start()
    {
        enemy = transform.parent.GetComponent<Enemy>();
    }
    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Snake"))
    //     {
    //         enemy.GetDamage();
    //     }
    // }
}
