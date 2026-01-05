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
        if (AllEnemiesController == null || AttackSlotsController == null || CylinderSpawners == null || CylinderSpawners.Count == 0)
            return;

        // attackableFrontEnemies boşsa kontrol yapmaya gerek yok
        if (AllEnemiesController.attackableFrontEnemies == null || AllEnemiesController.attackableFrontEnemies.Count == 0)
            return;

        // Herhangi bir cylinder düşmanla meşgulse fail kontrolleri yapma
        foreach (var slot in AttackSlotsController.attackSlots)
        {
            Cylinder cylinder = slot.occupiedCylinder;
            if (cylinder != null && cylinder.isBusyWithEnemy)
            {
                return; // Herhangi bir cylinder meşgulse hiç fail verme
            }
        }

        // Tüm slotları kontrol et
        bool allSlotsFull = true;
        bool hasEmptySlot = false;
        List<ColorType> attackSlotColors = new List<ColorType>();

        foreach (var slot in AttackSlotsController.attackSlots)
        {
            if (slot.isFulled)
            {
                // Slot'taki cylinder'ın rengini al
                Cylinder cylinder = slot.GetComponentInChildren<Cylinder>();
                if (cylinder != null)
                {
                    attackSlotColors.Add(cylinder.colorType);
                }
            }
            else
            {
                allSlotsFull = false;
                hasEmptySlot = true;
            }
        }

        // Düşman renkleri
        List<ColorType> enemyColors = AllEnemiesController.attackableFrontEnemies.Keys.ToList();

        // Tüm spawner'larda cylinder kalmış mı kontrol et
        bool anyCylindersLeft = false;
        foreach (var spawner in CylinderSpawners)
        {
            if (spawner != null && spawner.cylindersOnRoad.Count > 0)
            {
                anyCylindersLeft = true;
                break;
            }
        }

        // SENARYO 1: Tüm slotlar doluysa VE hiçbir slot rengi düşman renkleriyle eşleşmiyorsa
        // VE tüm cylinderlar kapasiteleri doluysa
        if (allSlotsFull)
        {
            bool hasMatchingColor = false;
            bool allCylindersFull = true;

            foreach (var slot in AttackSlotsController.attackSlots)
            {
                Cylinder cylinder = slot.GetComponentInChildren<Cylinder>();
                if (cylinder != null)
                {
                    // Renk kontrolü
                    if (enemyColors.Contains(cylinder.colorType))
                    {
                        hasMatchingColor = true;
                    }

                    // Capacity kontrolü - eğer henüz tam dolmamışsa
                    if (cylinder.UsedCapacity < cylinder.capacity)
                    {
                        allCylindersFull = false;
                    }
                }
            }

            // Sadece renkler eşleşmiyorsa VE tüm cylinderlar doluysa fail durumu var
            if (!hasMatchingColor && allCylindersFull)
            {
                CheckAndTriggerFail("Tüm slotlar dolu, tüm cylinderlar tam dolu ve düşmanlarla eşleşen renk yok!");
                return;
            }
        }

        // SENARYO 2: Slot boşsa ama hiçbir spawner'da cylinder kalmamışsa ve mevcut renkler uyuşmuyorsa
        if (hasEmptySlot && !anyCylindersLeft)
        {
            // Mevcut slotlardaki renklerin düşmanlarla eşleşip eşleşmediğini kontrol et
            bool hasMatchingColor = false;
            foreach (var slotColor in attackSlotColors)
            {
                if (enemyColors.Contains(slotColor))
                {
                    hasMatchingColor = true;
                    break;
                }
            }

            if (!hasMatchingColor)
            {
                CheckAndTriggerFail("Hiçbir spawner'da cylinder kalmadı ve mevcut renkler düşmanlarla eşleşmiyor!");
                return;
            }
        }

        // Hiçbir fail durumu yoksa sayacı sıfırla
        ResetFailCounter();
    }

    private void CheckAndTriggerFail(string reason)
    {
        // İlk fail durumu veya önceki fail check'ten beri zaman geçtiyse
        if (failCheckCounter == 0 || Time.time >= nextFailCheckTime)
        {
            failCheckCounter++;
            nextFailCheckTime = Time.time + failCheckInterval;

            Debug.LogWarning($"Fail Check {failCheckCounter}/{requiredFailChecks}: {reason}");

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
        if (alreadyFailed) return;
        alreadyFailed = true;
        // Debug.LogError("FAIL TRIGGERED: " + reason);
        failCheckCounter = 0; // Sayacı sıfırla
        GameManager.Instance.LevelFailed();
    }
}
