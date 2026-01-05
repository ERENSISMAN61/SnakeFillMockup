using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections;
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
    public float bulletSpeed = 5f; // Bullet hareket hızı
    public float waveAmplitude = 0.5f; // Yılan dalgasının genişliği
    public float waveFrequency = 2f; // Yılan dalgasının hızı

    private Transform trashTransform;
    [SerializeField] private bool isOnAttackSlot = false;
    public bool isBusyWithEnemy = false;
    private List<GameObject> activeBullets = new List<GameObject>();

    private AllEnemiesController allEnemiesController;
    private AttackSlot occupiedAttackSlot;

    void Start()
    {
        allEnemiesController = FindAnyObjectByType<AllEnemiesController>();
    }
    void Update()
    {
        if (isBusyWithEnemy) { Debug.Log("Busy with enemy, cannot attack now."); return; }
        if (!isOnAttackSlot)
        {
            // Debug.Log("Not on attack slot, cannot attack.");
            return;
        }
        if (UsedCapacity >= capacity)
        {
            Debug.Log("Cylinder is full, cannot export more items.");
            return;
        }

        var attackableEnemies = allEnemiesController.attackableFrontEnemies;

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
        else
        {
            Debug.Log($"No attackable enemy found for color {colorType}");
        }
    }
    public void ExportItem(AttackSlotsController attackSlotsController, int slotIndex)
    {
        // transform.SetParent(trashTransform);
        // Logic to export this cylinder to the specified attack slot
        occupiedAttackSlot = attackSlotsController.attackSlots[slotIndex];
        if (!occupiedAttackSlot.isFulled)
        {
            occupiedAttackSlot.isFulled = true;
            occupiedAttackSlot.occupiedCylinder = this;
            ExportMove(occupiedAttackSlot.transform);
        }
    }


    public void ExportMove(Transform targetTransform)
    {
        transform.SetParent(targetTransform);
        transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() =>
        {
            Debug.Log("Cylinder reached attack slot.");
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
        // this.trashTransform = trashTransform;
    }

    private void AttackEnemy(Enemy targetEnemy)
    {
        isBusyWithEnemy = true;

        // Kaç mermi göndereceğimizi hesapla
        int remainingCapacity = capacity - UsedCapacity;
        int bulletsToSend = Mathf.Min(remainingCapacity, targetEnemy.usableHealth);

        targetEnemy.usableHealth -= bulletsToSend;

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
                // Her atışta scale shake animasyonu
                transform.DOShakeScale(0.08f, 0.2f, 1, 0, false);

                // Mermiyi spawn et
                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                activeBullets.Add(bullet);

                // Bullet scriptini ayarla
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.targetPosition = targetEnemy.transform.position;
                    bulletScript.targetEnemy = targetEnemy; // Hedef enemy'yi ata
                    bulletScript.moveSpeed = bulletSpeed;
                    bulletScript.waveAmplitude = waveAmplitude;
                    bulletScript.waveFrequency = waveFrequency;
                }

                UsedCapacity++;
                capacityText.text = (capacity - UsedCapacity).ToString();

                if (capacity <= UsedCapacity)
                {
                    Debug.Log("Z Cylinder is empty after attack, destroying cylinder.");
                    DestroyCylinder();
                }
                // Hedefe ulaşma kontrolü için coroutine başlat
                StartCoroutine(CheckBulletReachedTarget(bullet, targetEnemy));
            });

            // Her mermi arasında delay
            snakeSequence.AppendInterval(0.08f);
        }




    }
    private void DestroyCylinder()
    {
        transform.SetParent(null);
        transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            isBusyWithEnemy = false;
            isOnAttackSlot = false;
            occupiedAttackSlot.isFulled = false;
            occupiedAttackSlot.occupiedCylinder = null;
            Destroy(gameObject);
        });
    }
    private IEnumerator CheckBulletReachedTarget(GameObject bullet, Enemy targetEnemy)
    {
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        while (bullet != null && bulletScript != null)
        {
            if (Vector3.Distance(bullet.transform.position, bulletScript.targetPosition) < 0.1f)
            {
                // Mermi hedefe ulaştı
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

                yield break;
            }

            yield return null;
        }

        // Bullet destroy edildiyse
        if (bullet == null)
        {
            activeBullets.Remove(bullet);
            if (activeBullets.Count == 0)
            {
                isBusyWithEnemy = false;
            }
        }
    }
}
