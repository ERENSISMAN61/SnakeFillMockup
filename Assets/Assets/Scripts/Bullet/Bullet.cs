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
    public ColorType colorType;

    void Start()
    {
        startPosition = transform.position;
        direction = (targetPosition - startPosition).normalized;

        // Hareket yönüne dik olan vektörü bul (sağa-sola sallanmak için)
        perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
    }

    void FixedUpdate()
    {
        if (hasHit) return;

        if (rb != null)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (hasHit) return;

        // // EnemyCollider'a çarptı mı?
        // GravityObject enemyCollider = other.GetComponent<GravityObject>();
        if (other.collider.tag == "Snake" || other.collider.tag == "Enemy")
        {
            // Bu bizim hedef enemy'miz mi kontrol et
            // if (targetEnemy != null && enemyCollider == targetEnemy)
            // {
            if (other.collider.tag == "Enemy")
            {
                hasHit = true;
                rb.isKinematic = true;
                targetEnemy.AddBullet(gameObject, colorType);
            }



            // rb.isKinematic = true;

            // transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InSine).OnComplete(() =>
            // {
            //     Destroy(gameObject);
            // });
            // }
        }

    }
}
