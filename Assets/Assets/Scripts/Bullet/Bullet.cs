using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    public Vector3 targetPosition;
    public GravityObject targetEnemy; // Hedef düşman referansı
    public float moveSpeed = 5f;
    public float waveAmplitude = 0.5f; // Dalga genişliği (sağa sola ne kadar gidecek)
    public float waveFrequency = 2f; // Dalga hızı (ne kadar hızlı sallanacak)
    public MeshRenderer meshRenderer;
    private Vector3 startPosition;
    private float traveledDistance = 0f;
    private Vector3 direction;
    private Vector3 perpendicular;
    public bool hasHit = false;
    public Rigidbody rb;
    public Collider col;


    void Start()
    {
        startPosition = transform.position;
        direction = (targetPosition - startPosition).normalized;

        // Hareket yönüne dik olan vektörü bul (sağa-sola sallanmak için)
        perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
    }

    void Update()
    {
        if (hasHit) return;

        // // İleri doğru hareket
        // float step = moveSpeed * Time.deltaTime;
        // traveledDistance += step;

        // // Yılan dalgası hareketi (sine wave)
        // float waveOffset = Mathf.Sin(traveledDistance * waveFrequency) * waveAmplitude;

        // // Yeni pozisyon: İleri + Dalga hareketi
        // Vector3 forwardMovement = direction * step;
        // Vector3 waveMovement = perpendicular * (waveOffset - Mathf.Sin((traveledDistance - step) * waveFrequency) * waveAmplitude);

        // transform.position += forwardMovement + waveMovement;

        // // Hedefe yaklaştık mı kontrol et
        // if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        // {
        //     transform.position = targetPosition;
        // }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        // EnemyCollider'a çarptı mı?
        GravityObject enemyCollider = other.GetComponent<GravityObject>();
        if (enemyCollider != null)
        {
            // Bu bizim hedef enemy'miz mi kontrol et
            if (targetEnemy != null && enemyCollider == targetEnemy)
            {
                hasHit = true;


                rb.isKinematic = true;
                // transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InSine).OnComplete(() =>
                // {
                //     Destroy(gameObject);
                // });
            }
        }

    }
}
