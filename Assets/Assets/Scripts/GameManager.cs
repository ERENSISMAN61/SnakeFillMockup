using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public AllEnemiesController allEnemiesController;
    public List<GameObject> levels;

    [SerializeField]
    private int currentLevelIndex = 0;

    [SerializeField] private GameObject currentLevel;

    public GameObject loadingPanel;
    public GameObject failText;
    public GameObject successText;

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
        Instantiate(currentLevel, Vector3.zero, Quaternion.identity);
        loadingPanel.SetActive(false);
        allEnemiesController = FindAnyObjectByType<AllEnemiesController>();
        FindAllEC();
    }

    private void FindAllEC()
    {
        StartCoroutine(FindAllECCoroutine());
    }
    private IEnumerator FindAllECCoroutine()
    {
        yield return null;
        yield return null;
        allEnemiesController = FindAnyObjectByType<AllEnemiesController>();
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
    public void RestartLevel()
    {
        Instantiate(currentLevel, Vector3.zero, Quaternion.identity);
        loadingPanel.SetActive(false);
        FindAllEC();
    }
    public void LoadNextLevel()
    {
        successText.SetActive(false);

        currentLevelIndex++;
        if (currentLevelIndex >= levels.Count)
        {
            Debug.Log("All levels completed!");
            currentLevelIndex = 0; // Or handle end of game scenario
        }

        // Optionally, you can add a loading screen or transition effect here

        // Load the next level

        currentLevel = levels[currentLevelIndex];
        Instantiate(currentLevel, Vector3.zero, Quaternion.identity);
        loadingPanel.SetActive(false);
    }

    public void NextLevel()
    {
        LoadNextLevel();
        FindAllEC();
    }
}
