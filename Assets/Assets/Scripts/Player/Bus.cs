using UnityEngine;
using DG.Tweening;

public class Bus : MonoBehaviour
{
    private Vector3 targetPosition;
    private float moveSpeed = 0.5f; // Constant movement speed

    public GameObject roadEndLine;
    public Color endLineColor;
    public Color WarningColor;
    public MeshRenderer roadEndLineRenderer;

    private bool canWarning = false;

    void Start()
    {
        // Initialize target position 4 units ahead
        targetPosition = transform.position + new Vector3(0f, 0f, 4f);
        roadEndLine.transform.position = targetPosition + new Vector3(0f, 0f, 5.86f);
        roadEndLineRenderer.sharedMaterial.color = endLineColor;

    }

    void OnEnable()
    {
        GameManager.Instance.OnOneWallCleaned += HandleOneWallCleaned;
    }
    void OnDisable()
    {
        GameManager.Instance.OnOneWallCleaned -= HandleOneWallCleaned;
    }
    void OnDestroy()
    {
        GameManager.Instance.OnOneWallCleaned -= HandleOneWallCleaned;
    }

    void Update()
    {
        // Continuously move towards target position at constant speed
        if (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) <= 1.01f)
            {
                Debug.LogWarning("Warning zone reached");
                canWarning = true;
                DOWarningAnimation();
            }

        }
        else
        {
            canWarning = false;
            DOWarningAnimation(); // This will stop the animation
        }
    }
    private Sequence warningSequence;

    private void DOWarningAnimation()
    {
        if (!canWarning)
        {
            // Stop animation if already running
            if (warningSequence != null && warningSequence.IsActive())
            {

                warningSequence.Kill();
                roadEndLineRenderer.sharedMaterial.color = endLineColor;
                roadEndLine.transform.localScale = Vector3.one;
            }
            return;
        }

        // Start animation only if not already running
        if (warningSequence == null || !warningSequence.IsActive())
        {
            warningSequence = DOTween.Sequence();

            // Color animation
            warningSequence.Append(roadEndLineRenderer.sharedMaterial.DOColor(WarningColor, "_Color", 0.3f));
            warningSequence.Join(roadEndLine.transform.DOScale(Vector3.one * 1.1f, 0.3f));
            warningSequence.Append(roadEndLineRenderer.sharedMaterial.DOColor(endLineColor, "_Color", 0.3f));
            warningSequence.Join(roadEndLine.transform.DOScale(Vector3.one, 0.3f));




            // Loop forever
            warningSequence.SetLoops(-1, LoopType.Restart);
        }
    }
    private void HandleOneWallCleaned()
    {
        Debug.Log("A wall has been cleaned! Bus received the event.");
        // Add 2 units to the target position
        targetPosition += new Vector3(0f, 0f, 2f);
        roadEndLine.transform.DOMove(targetPosition + new Vector3(0f, 0f, 5.86f), 0.5f);
    }


    //hareketsiz versiyon, bus z pos 0 yap, camera z pos -0.9 yap
    //     private void HandleOneWallCleaned()
    // {
    //     Debug.Log("A wall has been cleaned! Bus received the event.");
    //     MoveBus();
    // }


    // private void MoveBus()
    // {
    //     float moveDistance = 2f; // Move distance for each wall cleaned
    //     float moveDuration = 1f; // Duration of the move animation

    //     Vector3 targetPosition = transform.position + new Vector3(0f, 0f, moveDistance);
    //     transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InOutSine);
    // }
}
