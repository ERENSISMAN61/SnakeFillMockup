using UnityEngine;
using TMPro;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    public Transform wallModelTransform;
    public MeshRenderer wallMeshRenderer;
    public TextMeshPro healthText;

    public int health;
    public int usableHealth; // Cylinder'ların etkileşebileceği sağlık değeri
    public ColorType colorType;

    private Vector3 initialWallScale;
    private AllEnemiesController allEnemiesController;


    void Start()
    {
        healthText.text = health.ToString();
        usableHealth = health;
        wallMeshRenderer.material.color = ColorTypeProvider.GetColor(colorType);

        initialWallScale = wallModelTransform.localScale;
        allEnemiesController = FindAnyObjectByType<AllEnemiesController>();
    }
    public void EnemyDestroy()
    {
        // Dictionary'den kaldır
        if (allEnemiesController != null)
        {
            allEnemiesController.RemoveEnemyFromAttackable(this);
        }

        EnemyWalls enemyWalls = transform.parent.GetComponent<EnemyWalls>();
        enemyWalls.RemoveEnemy(this);

    }
    public void GetDamage()
    {

        health--;
        healthText.text = health.ToString();
        if (health <= 0)
        {
            wallModelTransform.DOKill(); // Stop any ongoing animations
            transform.DOKill();
            healthText.DOKill();
            healthText.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack);
            transform.DOMoveY(-5f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                EnemyDestroy();
                // Destroy(gameObject);
            });

        }
        else
        {
            // Optional: Add hit feedback like scaling effect
            wallModelTransform.DOKill(); // Stop any ongoing animations
            // wallModelTransform.localScale = initialWallScale ; // Enlarge slightly
            wallModelTransform.DOScale(initialWallScale * 0.95f, 0.05f).SetEase(Ease.InSine).OnComplete(() =>
            {
                wallModelTransform.DOScale(initialWallScale, 0.05f).SetEase(Ease.OutSine);
            });
        }
    }
}
