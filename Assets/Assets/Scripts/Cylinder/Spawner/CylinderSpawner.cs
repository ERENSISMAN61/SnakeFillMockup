using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using DG.Tweening;

public class CylinderSpawner : MonoBehaviour
{
    [SerializeField] private CylinderController cylinderController;


    [TableList]
    [SerializeField] private List<CylinderData> cylindersToSpawn = new List<CylinderData>();

    public List<GameObject> cylindersOnRoad = new List<GameObject>();

    [SerializeField] private bool isMovingAllSlots = false;


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

            cylindersOnRoad.Add(spawnedObject);

            Cylinder cylinderComponent = spawnedObject.GetComponent<Cylinder>();
            if (cylinderComponent != null)
            {
                cylinderComponent.capacity = cylindersToSpawn[i].capacity;

                cylinderComponent.colorType = cylindersToSpawn[i].colorType;
                cylinderComponent.SetInitialText();
                cylinderComponent.SetTrashTransform(cylinderController.trashTransform);
                // Apply color based on ColorType
                Color color = ColorTypeProvider.GetColor(cylindersToSpawn[i].colorType);
                MeshRenderer meshRenderer = cylinderComponent.meshRenderer;
                if (meshRenderer != null)
                {
                    // Create material instance to avoid shared material modification
                    Material materialInstance = new Material(meshRenderer.material);
                    materialInstance.color = color;
                    meshRenderer.material = materialInstance;
                }

                cylinderComponent.cylinderSpawner = this;
            }
            else
            {
                Debug.LogWarning($"Spawned object does not have a Cylinder component!");
            }
        }
    }

    public void RemoveFirstItem()
    {
        if (cylindersOnRoad.Count > 0)
        {
            cylindersOnRoad.RemoveAt(0);
        }
    }
    public void ExportCylinder(Rail rail, GravityObject gravityObject)
    {
        if (isMovingAllSlots) return;
        isMovingAllSlots = true;

        // if (attackSlotsController.attackSlots[slotIndex].isFulled) return;
        if (cylindersOnRoad.Count <= 0) return;

        Cylinder exportItem = null;
        exportItem = cylindersOnRoad[0].GetComponent<Cylinder>();

        if (exportItem == null) return;

        cylindersOnRoad.RemoveAt(0);

        // attackSlotsController.attackSlots[slotIndex].ImportCylinder(exportItem);



        exportItem.ExportItem(rail, gravityObject);

        MoveAllSlotsForward();
    }
    public GameObject GetFirstExportableObject()
    {
        if (isMovingAllSlots) return null;
        //   isMovingAllSlots = true;

        // if (attackSlotsController.attackSlots[slotIndex].isFulled) return;
        if (cylindersOnRoad.Count <= 0) return null;

        Cylinder exportItem = null;
        exportItem = cylindersOnRoad[0].GetComponent<Cylinder>();

        if (exportItem == null) return null;

        return exportItem.gameObject;
    }

    private void MoveAllSlotsForward()
    {
        // for (int i = 0; i < cylindersOnRoad.Count; i++)
        // {
        //     // Vector3 targetPosition = transform.localPosition + new Vector3(0, 0, -cylinderController.OffsetZ * i);
        //     cylindersOnRoad[i].transform.DOLocalMove(transform.localPosition + new Vector3(0, 0, 0), 0.55f).OnComplete(() =>
        //     {
        //         isMovingAllSlots = false;
        //     });
        // }

        transform.DOLocalMoveZ(transform.localPosition.z + cylinderController.OffsetZ, 0.55f).OnComplete(() =>
        {
            isMovingAllSlots = false;
        });
    }



















}


[System.Serializable]
public class CylinderData
{
    public int capacity;
    public ColorType colorType;
}
