using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public AllEnemiesController allEnemiesController;

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
}
