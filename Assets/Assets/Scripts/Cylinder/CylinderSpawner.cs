using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class CylinderSpawner : MonoBehaviour
{
    [SerializeField] private CylinderController cylinderController;
    
    [TableList]
    [SerializeField] private List<CylinderData> cylindersToSpawn = new List<CylinderData>();

    void Start()
    {
        SpawnCylinders();
    }

    private void SpawnCylinders()
    {
        if (cylinderController == null || cylinderController.CylinderPrefab == null)
        {
            Debug.LogError("CylinderController or CylinderPrefab is not assigned!");
            return;
        }

        Vector3 spawnPosition = transform.position;
        float offset = cylinderController.OffsetZ;

        for (int i = 0; i < cylindersToSpawn.Count; i++)
        {
            Vector3 position = spawnPosition + new Vector3(0, 0, -offset * i);
            GameObject spawnedObject = Instantiate(cylinderController.CylinderPrefab, position, Quaternion.identity, transform);
            
            Cylinder cylinderComponent = spawnedObject.GetComponent<Cylinder>();
            if (cylinderComponent != null)
            {
                cylinderComponent.capacity = cylindersToSpawn[i].capacity;
                cylinderComponent.UsedCapacity = cylindersToSpawn[i].usedCapacity;
                cylinderComponent.colorType = cylindersToSpawn[i].colorType;
            }
            else
            {
                Debug.LogWarning($"Spawned object does not have a Cylinder component!");
            }
        }
    }
}

[System.Serializable]
public class CylinderData
{
    public int capacity;
    public int usedCapacity;
    public ColorType colorType;
}
