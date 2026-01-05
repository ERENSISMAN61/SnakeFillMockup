using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FailController : MonoBehaviour
{
    public AllEnemiesController AllEnemiesController;
    public AttackSlotsController AttackSlotsController;
    public CylinderController CylinderController;
    public List<CylinderSpawner> CylinderSpawners = new List<CylinderSpawner>();

    private float checkInterval = 1f;
    private float nextCheckTime = 0f;

    private int failCheckCounter = 0;
    private int requiredFailChecks = 3;
    private float failCheckInterval = 0.5f; // Fail kontrolleri arası süre
    private float nextFailCheckTime = 0f;
    private bool alreadyFailed = false;
    void Start()
    {
        AllEnemiesController = FindAnyObjectByType<AllEnemiesController>();
        AttackSlotsController = FindAnyObjectByType<AttackSlotsController>();
        CylinderController = FindAnyObjectByType<CylinderController>();
        CylinderSpawners = new List<CylinderSpawner>(FindObjectsByType<CylinderSpawner>(FindObjectsSortMode.None));
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0)) return; // Tıklama anında fail kontrolü yapma
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            CheckFailConditions();
        }
    }

    private void CheckFailConditions()
    {
        Debug.Log("[FailControl] === Fail Kontrolü Başladı ===");

        if (AllEnemiesController == null || AttackSlotsController == null || CylinderSpawners == null || CylinderSpawners.Count == 0)
        {
            Debug.Log("[FailControl] Gerekli referanslar eksik, kontrol iptal edildi.");
            return;
        }

        // attackableFrontEnemies boşsa kontrol yapmaya gerek yok
        if (AllEnemiesController.attackableFrontEnemies == null || AllEnemiesController.attackableFrontEnemies.Count == 0)
        {
            Debug.Log("[FailControl] Saldırılabilir düşman yok, fail kontrolü gerekli değil.");
            return;
        }

        // Herhangi bir cylinder düşmanla meşgulse fail kontrolleri yapma
        foreach (var slot in AttackSlotsController.attackSlots)
        {
            Cylinder cylinder = slot.occupiedCylinder;
            if (cylinder != null && cylinder.isBusyWithEnemy)
            {
                Debug.Log($"[FailControl] Cylinder {cylinder.name} düşmanla meşgul, fail kontrolü yapılmıyor.");
                return; // Herhangi bir cylinder meşgulse hiç fail verme
            }
        }

        // Tüm slotları kontrol et
        bool allSlotsFull = true;
        bool hasEmptySlot = false;
        List<ColorType> attackSlotColors = new List<ColorType>();

        Debug.Log($"[FailControl] Toplam slot sayısı: {AttackSlotsController.attackSlots.Count}");

        foreach (var slot in AttackSlotsController.attackSlots)
        {
            if (slot.isFulled)
            {
                // Slot'taki cylinder'ın rengini al
                Cylinder cylinder = slot.occupiedCylinder;
                if (cylinder != null)
                {
                    attackSlotColors.Add(cylinder.colorType);
                    Debug.Log($"[FailControl] Slot dolu - Renk: {cylinder.colorType}, Kapasite: {cylinder.UsedCapacity}/{cylinder.capacity}");
                }
            }
            else
            {
                allSlotsFull = false;
                hasEmptySlot = true;
                Debug.Log("[FailControl] Boş slot bulundu.");
            }
        }

        // Düşman renkleri
        List<ColorType> enemyColors = AllEnemiesController.attackableFrontEnemies.Keys.ToList();
        Debug.Log($"[FailControl] Düşman renkleri: {string.Join(", ", enemyColors)}");
        Debug.Log($"[FailControl] Slot renkleri: {string.Join(", ", attackSlotColors)}");

        // Tüm spawner'larda cylinder kalmış mı kontrol et
        bool anyCylindersLeft = false;
        int totalCylindersLeft = 0;
        foreach (var spawner in CylinderSpawners)
        {
            if (spawner != null && spawner.cylindersOnRoad.Count > 0)
            {
                anyCylindersLeft = true;
                totalCylindersLeft += spawner.cylindersOnRoad.Count;
            }
        }
        Debug.Log($"[FailControl] Spawner'larda kalan toplam cylinder: {totalCylindersLeft}");

        // SENARYO 1: Tüm slotlar doluysa VE hiçbir slot rengi düşman renkleriyle eşleşmiyorsa
        // VE tüm cylinderlar kapasiteleri doluysa
        Debug.Log($"[FailControl] --- SENARYO 1 Kontrolü ---");
        Debug.Log($"[FailControl] Tüm slotlar dolu mu: {allSlotsFull}");

        if (allSlotsFull)
        {
            bool hasMatchingColor = false;
            bool allCylindersFull = true;

            foreach (var slot in AttackSlotsController.attackSlots)
            {
                Cylinder cylinder = slot.occupiedCylinder;
                if (cylinder != null)
                {
                    // Renk kontrolü
                    if (enemyColors.Contains(cylinder.colorType))
                    {
                        hasMatchingColor = true;
                        Debug.Log($"[FailControl] Eşleşen renk bulundu: {cylinder.colorType}");
                    }

                    // Capacity kontrolü - eğer henüz tam dolmamışsa
                    if (cylinder.UsedCapacity < cylinder.capacity)
                    {
                        allCylindersFull = false;
                        Debug.Log($"[FailControl] Cylinder henüz dolu değil: {cylinder.colorType} ({cylinder.UsedCapacity}/{cylinder.capacity})");
                    }
                }
            }

            Debug.Log($"[FailControl] Eşleşen renk var mı: {hasMatchingColor}, Tüm cylinderlar dolu mu: {allCylindersFull}");

            // Sadece renkler eşleşmiyorsa VE tüm cylinderlar doluysa fail durumu var
            if (!hasMatchingColor || allCylindersFull)
            {
                Debug.LogWarning("[FailControl] SENARYO 1: FAIL DURUMU TESPİT EDİLDİ!");
                CheckAndTriggerFail("Tüm slotlar dolu, tüm cylinderlar tam dolu ve düşmanlarla eşleşen renk yok!");
                return;
            }
            else
            {
                Debug.Log("[FailControl] SENARYO 1: Fail durumu yok.");
            }
        }

        // SENARYO 2: Slot boşsa ama hiçbir spawner'da cylinder kalmamışsa ve mevcut renkler uyuşmuyorsa
        Debug.Log($"[FailControl] --- SENARYO 2 Kontrolü ---");
        Debug.Log($"[FailControl] Boş slot var mı: {hasEmptySlot}, Spawner'da cylinder kaldı mı: {anyCylindersLeft}");

        if (hasEmptySlot && !anyCylindersLeft)
        {
            // Mevcut slotlardaki renklerin düşmanlarla eşleşip eşleşmediğini kontrol et
            bool hasMatchingColor = false;
            foreach (var slotColor in attackSlotColors)
            {
                if (enemyColors.Contains(slotColor))
                {
                    hasMatchingColor = true;
                    Debug.Log($"[FailControl] Eşleşen renk bulundu: {slotColor}");
                    break;
                }
            }

            Debug.Log($"[FailControl] Eşleşen renk var mı: {hasMatchingColor}");

            if (!hasMatchingColor)
            {
                Debug.LogWarning("[FailControl] SENARYO 2: FAIL DURUMU TESPİT EDİLDİ!");
                CheckAndTriggerFail("Hiçbir spawner'da cylinder kalmadı ve mevcut renkler düşmanlarla eşleşmiyor!");
                return;
            }
            else
            {
                Debug.Log("[FailControl] SENARYO 2: Fail durumu yok.");
            }
        }
        else
        {
            Debug.Log("[FailControl] SENARYO 2: Kontrol şartları sağlanmadı.");
        }

        // Hiçbir fail durumu yoksa sayacı sıfırla
        Debug.Log("[FailControl] === Fail durumu tespit edilmedi, sayaç sıfırlanıyor ===");
        ResetFailCounter();
    }

    private void CheckAndTriggerFail(string reason)
    {
        // İlk fail durumu veya önceki fail check'ten beri zaman geçtiyse
        if (failCheckCounter == 0 || Time.time >= nextFailCheckTime)
        {
            failCheckCounter++;
            nextFailCheckTime = Time.time + failCheckInterval;

            Debug.Log($"Fail Check {failCheckCounter}/{requiredFailChecks}: {reason}");

            // 3 kez üst üste fail durumu tespit edildiyse
            if (failCheckCounter >= requiredFailChecks)
            {
                TriggerFail(reason);
            }
        }
    }

    private void ResetFailCounter()
    {
        if (failCheckCounter > 0)
        {
            Debug.Log("Fail durumu düzeldi, sayaç sıfırlandı.");
            failCheckCounter = 0;
        }
    }

    private void TriggerFail(string reason)
    {
        if (alreadyFailed)
        {
            Debug.Log("[FailControl] Fail zaten tetiklenmişti, tekrar tetiklenmiyor.");
            return;
        }
        alreadyFailed = true;
        Debug.Log($"[FailControl] ✖✖✖ FAIL TETİKLENDİ ✖✖✖\nSebep: {reason}");
        failCheckCounter = 0; // Sayacı sıfırla
        GameManager.Instance.CheckCompleteLevel(false);
    }
}
