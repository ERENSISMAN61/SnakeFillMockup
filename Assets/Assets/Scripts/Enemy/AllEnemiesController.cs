using System.Collections.Generic;
using UnityEngine;

public class AllEnemiesController : MonoBehaviour
{
    [SerializeField] private List<EnemyWalls> enemyWallsList = new List<EnemyWalls>();
    public float StartZPosition = 10f;
    public float OffsetZPosition = 2f;
    public Dictionary<ColorType, Enemy> attackableFrontEnemies = new Dictionary<ColorType, Enemy>();

    void Start()
    {
        float currentOffsetZ = StartZPosition;
        foreach (Transform child in transform)
        {
            EnemyWalls enemyWalls = child.GetComponent<EnemyWalls>();
            if (enemyWalls != null)
            {
                enemyWallsList.Add(enemyWalls);

                enemyWalls.transform.localPosition = new Vector3(0f, 0f, currentOffsetZ);
                currentOffsetZ += OffsetZPosition;
            }
        }
        SetOutlineActive();
        SetFirstAttackableFrontEnemies();
    }
    public void SetFirstAttackableFrontEnemies()
    {
        attackableFrontEnemies.Clear();
        if (enemyWallsList.Count == 0) return;
        var FrontEnemyWall = enemyWallsList[0];
        if (FrontEnemyWall == null) return;
        FrontEnemyWall.GetEnemies().ForEach(enemy =>
        {

            attackableFrontEnemies.Add(enemy.colorType, enemy);

        });

    }
    public void RemoveWallFromList(EnemyWalls enemyWalls)
    {
        enemyWallsList.Remove(enemyWalls);
        SetOutlineActive();
        if (enemyWallsList.Count == 0)
        {
            Debug.Log("All enemy walls destroyed!");
            // Additional logic when all walls are destroyed
        }
    }

    private void SetOutlineActive()
    {
        var FrontEnemyWall = enemyWallsList[0];
        if (FrontEnemyWall == null) return;
        foreach (var enemy in FrontEnemyWall.GetComponentsInChildren<Enemy>())
        {

            if (enemy.wallMeshRenderer.material.HasProperty("_OutlineColor"))
            {
                enemy.wallMeshRenderer.material.SetFloat("_OutlineWidth", 20f);
                // enemy.wallMeshRenderer.material.

                //  Debug.Log(gameObject.name + "2");
                // Set outline color to white
                // meshRenderer.material.SetColor("_OutlineColor", Color.white);

            }
        }
    }


}
