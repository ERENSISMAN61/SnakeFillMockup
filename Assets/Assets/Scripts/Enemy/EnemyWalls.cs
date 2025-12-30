using UnityEngine;
using System.Collections.Generic;

public class EnemyWalls : MonoBehaviour
{
    [SerializeField] List<Enemy> enemies = new List<Enemy>();

    public bool isDestroyedWall = false;
    private void Start()
    {
        foreach (Transform child in transform)
        {
            Enemy enemy = child.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemies.Add(enemy);
            }
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);

        if (enemies.Count == 0)
        {
            isDestroyedWall = true;
            AllEnemiesController allEnemiesController = transform.parent.GetComponent<AllEnemiesController>();
            allEnemiesController.RemoveWallFromList(this);
        }
    }

    public List<Enemy> GetEnemies()
    {
        return enemies;
    }
}
