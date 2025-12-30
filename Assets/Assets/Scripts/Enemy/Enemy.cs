using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public Transform wallModelTransform;
    public MeshRenderer wallMeshRenderer;
    public TextMeshPro healthText;

    public int health;

    public ColorType colorType;


    void Start()
    {
        healthText.text = health.ToString();
        wallMeshRenderer.material.color = ColorTypeProvider.GetColor(colorType);
    }
    public void EnemyDestroy()
    {
        EnemyWalls enemyWalls = transform.parent.GetComponent<EnemyWalls>();
        enemyWalls.RemoveEnemy(this);
    }
    public void GetDamage()
    {
        health--;
        healthText.text = health.ToString();
        if (health <= 0)
        {
            EnemyDestroy();
            // Destroy(gameObject);
        }
    }
}
