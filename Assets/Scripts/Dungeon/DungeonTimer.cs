using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonTimer : MonoBehaviour
{
    public static DungeonTimer Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI timerText; // Assign in Inspector

    private float elapsedTime = 0f;
    private bool isRunning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Dungeon")
        {
            StartTimer();
        }
        else
        {
            StopTimer();
        }
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
        UpdateTimerUI();
    }

    public void StopTimer()
    {
        isRunning = false;
        Debug.Log($"Dungeon run time: {FormatTime(elapsedTime)}");
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = FormatTime(elapsedTime);
        }
    }

    public string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return $"{minutes:00}:{seconds:00}"; // MM:SS format
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
