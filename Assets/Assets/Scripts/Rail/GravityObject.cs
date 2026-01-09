using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;

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

    [SerializeField] private List<MergeSettings> mergeSettingsList = new List<MergeSettings>();
    [ShowInInspector]
    private Dictionary<ColorType, List<GameObject>> bulletLists = new Dictionary<ColorType, List<GameObject>>();

    void Start()
    {
        // Her renk için boş liste oluştur
        foreach (ColorType colorType in System.Enum.GetValues(typeof(ColorType)))
        {
            bulletLists[colorType] = new List<GameObject>();
        }
    }

    public void AddBullet(GameObject bullet, ColorType colorType)
    {
        if (!bulletLists.ContainsKey(colorType))
        {
            bulletLists[colorType] = new List<GameObject>();
        }

        bulletLists[colorType].Add(bullet);

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
}
