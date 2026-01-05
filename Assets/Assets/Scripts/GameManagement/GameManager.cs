using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Game Events
    public event System.Action OnOneWallCleaned;

    public List<GameObject> levels;

    [SerializeField]
    private int currentLevelIndex = 0;

    [SerializeField] private GameObject currentLevel;

    [SerializeField] private GameObject currentLevelInstance;


    public GameObject loadingPanel;
    public GameObject failText;
    public GameObject successText;

    private bool levelAlreadyFailed = false;
    private bool levelAlreadyCompleted = false;

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
    void Start()
    {
        currentLevel = levels[currentLevelIndex];
        currentLevelInstance = Instantiate(currentLevel, Vector3.zero, Quaternion.identity);
        loadingPanel.SetActive(false);


    }


    public void LevelFailed()
    {

        failText.SetActive(true);
        loadingPanel.SetActive(true);
        Debug.Log("Level Failed! Restarting level...");

    }
    public void LevelCompleted()
    {



        successText.SetActive(true);
        loadingPanel.SetActive(true);
        Debug.Log("Level Completed! Loading next level...");

    }

    public void CheckCompleteLevel(bool condition)
    {

        if (condition)
        {
            if (levelAlreadyCompleted || levelAlreadyFailed) return;
            levelAlreadyCompleted = true;

            LevelCompleted();
        }
        else
        {
            if (levelAlreadyFailed || levelAlreadyCompleted) return;
            levelAlreadyFailed = true;

            LevelFailed();
        }
    }

    public void RestartLevel()
    {
        DestroyLastLevel();
        currentLevelInstance = Instantiate(currentLevel, Vector3.zero, Quaternion.identity);


    }
    public void LoadNextLevel()
    {
        DestroyLastLevel();
        successText.SetActive(false);
        failText.SetActive(false);
        currentLevelIndex++;
        if (currentLevelIndex >= levels.Count)
        {
            Debug.Log("All levels completed!");
            currentLevelIndex = 0; // Or handle end of game scenario
        }

        // Optionally, you can add a loading screen or transition effect here

        // Load the next level

        currentLevel = levels[currentLevelIndex];
        currentLevelInstance = Instantiate(currentLevel, Vector3.zero, Quaternion.identity);
    }

    public void NextLevel()
    {
        LoadNextLevel();

    }

    public void TriggerOneWallCleaned()
    {
        OnOneWallCleaned?.Invoke();
    }

    private void DestroyLastLevel()
    {
        StartCoroutine(DestroyLastLevelCoroutine());
    }
    private IEnumerator DestroyLastLevelCoroutine()
    {
        DestroyImmediate(currentLevelInstance);
        currentLevelInstance = null;
        yield return null;
        yield return null;
        yield return null;

        levelAlreadyFailed = false;
        levelAlreadyCompleted = false;

        loadingPanel.SetActive(false);
        failText.SetActive(false);
        successText.SetActive(false);
    }
}
