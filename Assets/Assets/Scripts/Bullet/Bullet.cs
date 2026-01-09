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

    public bool isCheckingMovement = false;
    public bool isHitEnemy = false;
    public bool isColorMergeMovement = false;
    private bool firstHit = true;
    private Vector3 lastPositionCheck;
    public float stuckThreshold = 0.1f; // Bu mesafeden az hareket ettiyse stuck sayılır
    public float checkDelay = 0.03f; // Kaç saniye sonra kontrol edilecek



    void OnEnable()
    {
        GameManager.Instance.ColorsMerged += MoveAfterColorMerge;
    }
    void OnDisable()
    {
        GameManager.Instance.ColorsMerged -= MoveAfterColorMerge;
    }
    void OnDestroy()
    {
        GameManager.Instance.ColorsMerged -= MoveAfterColorMerge;
    }
    void Start()
    {
        startPosition = transform.position;
        direction = (targetPosition - startPosition).normalized;

        // Hareket yönüne dik olan vektörü bul (sağa-sola sallanmak için)
        perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

        // Rigidbody ayarları
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

        }
    }

    void FixedUpdate()
    {
        // if (hasHit) return;

        if (rb != null)
        {
            // Velocity bazlı hareket - daha smooth ve fizik motoruyla uyumlu
            Vector3 direction = (targetPosition - rb.position).normalized;
            Vector3 targetVelocity = direction * moveSpeed;

            // Mevcut velocity'yi hedef velocity'ye yumuşak geçiş
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 10f);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (hasHit) return;

        if (other.collider.tag == "Snake")
        {
            // Snake'e çarptı, pozisyon kontrolü başlat
            if (!isCheckingMovement)
            {
                StartCoroutine(CheckIfStuck());
            }
        }
        else if (other.collider.tag == "Enemy")
        {
            // Enemy'ye direkt çarptı
            hasHit = true;
            // rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            isHitEnemy = true;
            if (firstHit)
            {
                targetEnemy.AddBullet(gameObject, colorType);
                firstHit = false;
            }
            isColorMergeMovement = false;
        }
    }

    private System.Collections.IEnumerator CheckIfStuck()
    {
        isCheckingMovement = true;
        lastPositionCheck = transform.position;

        // Belirli bir süre bekle
        yield return new WaitForSeconds(checkDelay);

        // Pozisyon değişimi kontrolü
        float distanceMoved = Vector3.Distance(lastPositionCheck, transform.position);

        if (distanceMoved < stuckThreshold)
        {
            // Hareket etmedi veya çok az hareket etti, stuck durumunda
            hasHit = true;
            // rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            if (firstHit)
            {
                targetEnemy.AddBullet(gameObject, colorType);
                firstHit = false;
            }
            isColorMergeMovement = false;
        }
        else
        {
            // Hala hareket ediyor, tekrar kontrol et
            isCheckingMovement = false;
            StartCoroutine(CheckIfStuck());
        }
    }


    private void MoveAfterColorMerge()
    {
        if (isHitEnemy) return;

        rb.isKinematic = false;
        hasHit = false;
        isColorMergeMovement = true;
        // CheckIfStuck();

    }
}
