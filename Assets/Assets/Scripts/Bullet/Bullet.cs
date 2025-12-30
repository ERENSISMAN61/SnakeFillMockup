using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 targetPosition;
    public Enemy targetEnemy; // Hedef düşman referansı
    public float moveSpeed = 5f;
    public float waveAmplitude = 0.5f; // Dalga genişliği (sağa sola ne kadar gidecek)
    public float waveFrequency = 2f; // Dalga hızı (ne kadar hızlı sallanacak)

    private Vector3 startPosition;
    private float traveledDistance = 0f;
    private Vector3 direction;
    private Vector3 perpendicular;
    private bool hasHit = false;

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

        // İleri doğru hareket
        float step = moveSpeed * Time.deltaTime;
        traveledDistance += step;

        // Yılan dalgası hareketi (sine wave)
        float waveOffset = Mathf.Sin(traveledDistance * waveFrequency) * waveAmplitude;

        // Yeni pozisyon: İleri + Dalga hareketi
        Vector3 forwardMovement = direction * step;
        Vector3 waveMovement = perpendicular * (waveOffset - Mathf.Sin((traveledDistance - step) * waveFrequency) * waveAmplitude);

        transform.position += forwardMovement + waveMovement;

        // Hedefe yaklaştık mı kontrol et
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            transform.position = targetPosition;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        // EnemyCollider'a çarptı mı?
        EnemyCollider enemyCollider = other.GetComponent<EnemyCollider>();
        if (enemyCollider != null)
        {
            // Bu bizim hedef enemy'miz mi kontrol et
            if (targetEnemy != null && enemyCollider.enemy == targetEnemy)
            {
                hasHit = true;
                targetEnemy.GetDamage();
                Destroy(gameObject);
            }
        }
        // Başka bir bullet'a çarptı mı?
        else if (other.GetComponent<Bullet>() != null)
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }
}
