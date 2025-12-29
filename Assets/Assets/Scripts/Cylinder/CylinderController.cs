using UnityEngine;

public class CylinderController : MonoBehaviour
{
    [SerializeField] private GameObject cylinderPrefab;
    [SerializeField] private float offsetZ = 0.5f;

    public GameObject CylinderPrefab => cylinderPrefab;
    public float OffsetZ => offsetZ;
}
