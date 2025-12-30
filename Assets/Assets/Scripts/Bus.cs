using UnityEngine;
using DG.Tweening;

public class Bus : MonoBehaviour
{
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

    private void HandleOneWallCleaned()
    {
        Debug.Log("A wall has been cleaned! Bus received the event.");
        MoveBus();
    }


    private void MoveBus()
    {
        float moveDistance = 2f; // Move distance for each wall cleaned
        float moveDuration = 1f; // Duration of the move animation

        Vector3 targetPosition = transform.position + new Vector3(0f, 0f, moveDistance);
        transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InOutSine);
    }
}
