using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

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

    private int health = 3;
    public Image healthIcon_1;
    public TextMeshProUGUI healthText_1;


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
        DOTween.KillAll();
        failText.SetActive(true);
        loadingPanel.SetActive(true);

        healthIcon_1.gameObject.SetActive(false);

        ResetHeartIcons();
        Debug.Log("Level Failed! Restarting level...");

    }
    private void ResetHeartIcons()
    {
        healthIcon_1.transform.localScale = Vector3.one;

        healthIcon_1.color = Color.white;

    }
    public void LevelCompleted()
    {



        successText.SetActive(true);
        loadingPanel.SetActive(true);

        healthIcon_1.gameObject.SetActive(false);

        ResetHeartIcons();
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


        healthIcon_1.gameObject.SetActive(true);
        healthText_1.text = "3";

        health = 3;

        currentLevelInstance = Instantiate(currentLevel, Vector3.zero, Quaternion.identity);


    }
    public void LoadNextLevel()
    {
        DestroyLastLevel();
        successText.SetActive(false);
        failText.SetActive(false);

        healthIcon_1.gameObject.SetActive(true);
        healthText_1.text = "3";


        health = 3;

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

    public void DecreaseHealth()
    {
        health--;
        if (health < 0) health = 0;

        if (health == 2)
        {
            DecreaseHealthAnimation(healthIcon_1);
        }
        else if (health == 1)
        {
            DecreaseHealthAnimation(healthIcon_1);
        }
        else if (health == 0)
        {
            DecreaseHealthAnimation(healthIcon_1, true);

        }
    }

    private void DecreaseHealthAnimation(Image healthIcon, bool isFail = false)
    {
        Sequence healthSequence = DOTween.Sequence();

        // Scale up with slowing ease and turn black
        healthSequence.Append(healthIcon.transform.DOScale(Vector3.one * 1.3f, 0.1f));
        healthSequence.Join(healthIcon.DOColor(Color.black, 0.4f).OnComplete(() =>
        {
            healthText_1.text = health.ToString();
        }));

        // Scale down to 0
        healthSequence.Append(healthIcon.transform.DOScale(Vector3.one, 0.5f));
        healthSequence.Join(healthIcon.DOColor(Color.white, 0.15f));
        // Disable at the end
        healthSequence.OnComplete(() =>
        {
            // healthIcon.gameObject.SetActive(false);

            if (isFail)
            {
                LevelFailed();
            }
        });
    }




}
