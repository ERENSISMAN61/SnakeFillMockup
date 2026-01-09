using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

public class GravityObject : MonoBehaviour
{
    public Transform targetTransform;

    [System.Serializable]
    public class MergeSettings
    {
        public ColorType colorType;
        public int thresholdCount = 20; // Bu sayıya ulaşınca merge olacak
        public int outputCount = 5; // Merge sonrası kaç obje üretilecek
        public ColorType nextColorType; // Merge edince hangi renge dönüşecek
    }

    [System.Serializable]
    public class TargetColorSettings
    {
        public ColorType targetColor;
        public int targetCount;
        public TextMeshProUGUI countText;
        public Image colorImage;
    }

    [SerializeField] private List<MergeSettings> mergeSettingsList = new List<MergeSettings>();
    [SerializeField] private List<TargetColorSettings> targetColorsList = new List<TargetColorSettings>();

    [ShowInInspector]
    private Dictionary<ColorType, List<GameObject>> bulletLists = new Dictionary<ColorType, List<GameObject>>();

    void Start()
    {
        // Her renk için boş liste oluştur
        foreach (ColorType colorType in System.Enum.GetValues(typeof(ColorType)))
        {
            bulletLists[colorType] = new List<GameObject>();
        }

        // Target UI'larını ayarla
        foreach (TargetColorSettings target in targetColorsList)
        {
            if (target.countText != null)
            {
                target.countText.text = target.targetCount.ToString();
            }

            if (target.colorImage != null)
            {
                target.colorImage.color = ColorTypeProvider.GetColor(target.targetColor);
            }
        }
    }

    public void AddBullet(GameObject bullet, ColorType colorType)
    {
        if (!bulletLists.ContainsKey(colorType))
        {
            bulletLists[colorType] = new List<GameObject>();
        }

        bulletLists[colorType].Add(bullet);

        // Target kontrolü - hedef renk ve sayıya ulaşıldı mı?
        CheckTargetCompletion(colorType);

        // Bu renk için merge ayarı var mı kontrol et
        MergeSettings settings = mergeSettingsList.FirstOrDefault(s => s.colorType == colorType);
        if (settings != null)
        {
            // Threshold'a ulaştı mı?
            if (bulletLists[colorType].Count >= settings.thresholdCount)
            {
                MergeBullets(colorType, settings);
            }
        }
    }

    private void CheckTargetCompletion(ColorType colorType)
    {

        // Bu renk için target var mı kontrol et
        TargetColorSettings target = targetColorsList.FirstOrDefault(t => t.targetColor == colorType);
        if (target != null)
        {
            int currentCount = bulletLists[colorType].Count;

            // UI'ı güncelle
            if (target.countText != null)
            {
                target.countText.text = currentCount.ToString();
            }

            // Hedefe ulaşıldı mı?
            if (currentCount >= target.targetCount)
            {
                // Kazandı!
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.CheckCompleteLevel(true);
                }
            }

        }
    }

    private void MergeBullets(ColorType sourceColor, MergeSettings settings)
    {
        List<GameObject> sourceBullets = bulletLists[sourceColor];

        // İlk threshold kadar objeyi al
        List<GameObject> bulletsToProcess = sourceBullets.Take(settings.thresholdCount).ToList();

        // TargetTransform'a mesafeye göre sırala ve en yakın N adet objeyi al (merge edilecekler)
        List<GameObject> bulletsToMerge = bulletsToProcess
            .OrderBy(b => Vector3.Distance(b.transform.position, targetTransform.position))
            .Take(settings.outputCount)
            .ToList();

        // Kalanları al (silinecekler)
        List<GameObject> bulletsToDestroy = bulletsToProcess.Except(bulletsToMerge).ToList();

        // Merge edilecek objeleri bir sonraki renk listesine ekle
        Color nextColor = ColorTypeProvider.GetColor(settings.nextColorType);
        foreach (GameObject bullet in bulletsToMerge)
        {
            if (bullet != null)
            {
                // Rengini değiştir


                // Bullet script'indeki color type'ı da güncelle (varsa)
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    // Eğer Bullet script'inde colorType field'ı varsa güncelle
                    bulletScript.colorType = settings.nextColorType;

                }

                // Yeni renk listesine ekle
                bulletLists[settings.nextColorType].Add(bullet);

                StartCoroutine(DelayedTargetCheck(settings.nextColorType));

                // Original scale'i sakla
                Vector3 originalScale = bullet.transform.localScale;

                // Scale'i 0 yap, sonra rengi değiştir ve pop scale ile geri getir
                bullet.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    bulletScript.meshRenderer.material.color = Color.Lerp(nextColor, Color.white, 0.3f);
                    bullet.transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutBack);
                });
            }
        }



        // Kaynak listeden hepsini kaldır (threshold kadar)
        sourceBullets.RemoveRange(0, settings.thresholdCount);

        // Silinecek objeleri yok et
        foreach (GameObject bullet in bulletsToDestroy)
        {
            if (bullet != null)
            {
                bullet.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    Destroy(bullet);

                });

            }
        }


        GameManager.Instance.TriggerColorsMerged();

    }

    private System.Collections.IEnumerator DelayedTargetCheck(ColorType nextColorType)
    {
        // Bir frame bekle
        yield return new WaitForSeconds(0.1f);

        // Tüm renkler için target kontrolü yap

        CheckTargetCompletion(nextColorType);

    }
}
