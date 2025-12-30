using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;
public class Cylinder : MonoBehaviour
{
    public int capacity;
    public int UsedCapacity;

    public ColorType colorType;
    public MeshRenderer meshRenderer;

    public CylinderSpawner cylinderSpawner;
    public TextMeshPro capacityText;
    public GameObject bulletPrefab;
    public float bulletSpacing = 0.3f; // Bullet'lar arası mesafe
    public float bulletSpeed = 1f; // Bullet hareket hızı

    private Transform trashTransform;
    private bool isOnAttackSlot = false;
    private bool isBusyWithEnemy = false;
    private List<GameObject> activeBullets = new List<GameObject>();
    void Update()
    {
        if (isBusyWithEnemy) return;
        if (!isOnAttackSlot) return;
        if (UsedCapacity >= capacity)
        {
            Debug.Log("Cylinder is full, cannot export more items.");
            return;
        }

        var attackableEnemies = GameManager.Instance.allEnemiesController.attackableFrontEnemies;

        if (attackableEnemies.ContainsKey(colorType))
        {
            Enemy targetEnemy = attackableEnemies[colorType];
            Debug.Log($"Found enemy with color {colorType}");
            if (targetEnemy == null)
            {
                Debug.Log("Target enemy is null.");
                return;
            }
            if (targetEnemy.health <= 0)
            {
                Debug.Log("Target enemy is already destroyed.");
                return;
            }


            AttackEnemy(targetEnemy);
        }
    }
    public void ExportItem(AttackSlotsController attackSlotsController, int slotIndex)
    {
        transform.SetParent(trashTransform);
        // Logic to export this cylinder to the specified attack slot
        AttackSlot slot = attackSlotsController.attackSlots[slotIndex];
        if (!slot.isFulled)
        {
            slot.isFulled = true;
            ExportMove(slot.transform);
        }
    }


    public void ExportMove(Transform targetTransform)
    {
        transform.DOMove(targetTransform.position, 0.5f).OnComplete(() =>
        {
            isOnAttackSlot = true;

        });
    }

    public void SetInitialText()
    {
        capacityText.text = capacity.ToString();
    }
    public void SetTrashTransform(Transform trashTransform)
    {
        // Store the trash transform for later use
        this.trashTransform = trashTransform;
    }

    private void AttackEnemy(Enemy targetEnemy)
    {
        isBusyWithEnemy = true;

        // Kaç mermi göndereceğimizi hesapla
        int remainingCapacity = capacity - UsedCapacity;
        int bulletsToSend = Mathf.Min(remainingCapacity, targetEnemy.health);

        if (bulletsToSend <= 0)
        {
            isBusyWithEnemy = false;
            return;
        }

        // Yılan şeklinde mermileri spawn et
        Sequence snakeSequence = DOTween.Sequence();

        for (int i = 0; i < bulletsToSend; i++)
        {
            int bulletIndex = i;
            snakeSequence.AppendCallback(() =>
            {
                // Mermiyi spawn et
                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                activeBullets.Add(bullet);

                // Hedef pozisyon - yılan gibi sırayla dizilim
                Vector3 targetPosition = targetEnemy.transform.position;
                UsedCapacity++;
                capacityText.text = (capacity - UsedCapacity).ToString();
                // Mermiyi hedefe hareket ettir
                bullet.transform.DOMove(targetPosition, bulletSpeed).SetEase(Ease.Linear).OnComplete(() =>
                {
                    // Mermi hedefe ulaştığında
                    if (targetEnemy != null && targetEnemy.health > 0)
                    {
                        targetEnemy.GetDamage();

                    }

                    activeBullets.Remove(bullet);
                    Destroy(bullet);

                    // Tüm mermiler tamamlandı mı?
                    if (activeBullets.Count == 0)
                    {
                        isBusyWithEnemy = false;
                    }
                });
            });

            // Her mermi arasında delay
            snakeSequence.AppendInterval(0.1f);
        }
    }
}
