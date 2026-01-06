using UnityEngine;
using DG.Tweening;

public class Bus : MonoBehaviour
{
    private Vector3 targetPosition;
    private float moveSpeed = 0.5f; // Constant movement speed

    void Start()
    {
        // Initialize target position 4 units ahead
        targetPosition = transform.position + new Vector3(0f, 0f, 4f);
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
        }
    }

    private void HandleOneWallCleaned()
    {
        Debug.Log("A wall has been cleaned! Bus received the event.");
        // Add 2 units to the target position
        targetPosition += new Vector3(0f, 0f, 2f);
    }
}
