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
        targetPosition = transform.position + new Vector3(0f, 0f, 6f);
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

            if (Vector3.Distance(transform.position, targetPosition) <= 0.8f)
            {
                Debug.LogWarning("Warning zone reached");
                canWarning = true;
                DOWarningAnimation();
            }

        }
        else
        {
            Debug.LogError("Target position reached");
            canWarning = false;
            DOWarningAnimation(true); // This will stop the animation
        }
    }
    private Sequence warningSequence;

    private void DOWarningAnimation(bool getDamage = false)
    {
        if (!canWarning && getDamage)
        {
            // Stop animation if already running
            if (warningSequence != null && warningSequence.IsActive())
            {
                // Save current state
                Color currentColor = roadEndLineRenderer.sharedMaterial.color;
                Vector3 currentScale = roadEndLine.transform.localScale;
                Quaternion currentRotation = roadEndLine.transform.rotation;

                warningSequence.Kill();

                // Damage animation
                Sequence damageSequence = DOTween.Sequence();

                // First rotation: 10 degrees
                damageSequence.Append(roadEndLine.transform.DORotate(new Vector3(25f, 0f, 0f), 0.15f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuad));

                // Scale up and color change with second rotation: -30 degrees
                damageSequence.Append(roadEndLine.transform.DOScale(currentScale * 1.1f, 0.3f));
                damageSequence.Join(roadEndLineRenderer.sharedMaterial.DOColor(WarningColor, "_Color", 0.3f));
                damageSequence.Join(roadEndLine.transform.DORotate(new Vector3(-40f, 0f, 0f), 0.3f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuart));

                // Return to normal state
                damageSequence.Append(roadEndLine.transform.DOScale(Vector3.one, 0.25f));
                damageSequence.Join(roadEndLineRenderer.sharedMaterial.DOColor(endLineColor, "_Color", 0.25f));
                damageSequence.Join(roadEndLine.transform.DORotate(currentRotation.eulerAngles, 0.25f).SetEase(Ease.InQuart)).OnComplete(() =>
                {
                    // Ensure final state is exactly as before
                    // roadEndLineRenderer.sharedMaterial.color = currentColor;
                    // roadEndLine.transform.localScale = currentScale;
                    // roadEndLine.transform.rotation = currentRotation;
                    GameManager.Instance.DecreaseHealth();

                });
            }
            return;
        }
        else if (!canWarning)
        {
            // Stop animation if already running
            if (warningSequence != null && warningSequence.IsActive())
            {
                warningSequence.Kill();
                // Reset to original state
                roadEndLineRenderer.sharedMaterial.DOColor(endLineColor, "_Color", 0.2f);
                roadEndLine.transform.DOScale(Vector3.one, 0.2f);
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
        roadEndLine.transform.DOMove(targetPosition + new Vector3(0f, 0f, 5.86f), 0.5f).OnComplete(() =>
        {
            canWarning = false;
            DOWarningAnimation(); // This will stop the animation
        });


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
