using UnityEngine;

public class EnemyCollider : MonoBehaviour
{
    private Enemy enemy;
    void Start()
    {
        enemy = transform.parent.GetComponent<Enemy>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Snake"))
        {
            enemy.GetDamage();
        }
    }
}
