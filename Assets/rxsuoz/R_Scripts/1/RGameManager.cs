using UnityEngine;
using TMPro;

public class RGameManager : MonoBehaviour
{
    public static RGameManager Instance;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI judgeText;

    [Header("Game Data")]
    public int score;
    public int totalNotes;
    public float inputOffsetMs = 0f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        audioSource.Play();
    }

    void Update()
    {
        scoreText.text = $"Score : {score}";

        if (!audioSource.isPlaying)
        {
            judgeText.text = IsClear() ? "CLEAR" : "FAIL";
        }
    }

    public double GetSongTime()
    {
        return audioSource.time;
    }

    public void AddScore(int value)
    {
        score += value;
    }

    public bool IsClear()
    {
        int maxScore = totalNotes * 10;
        return score >= maxScore * 0.6f;
    }

    public void ShowJudge(string text)
    {
        judgeText.text = text;
        CancelInvoke(nameof(ClearJudge));
        Invoke(nameof(ClearJudge), 0.5f);
    }

    void ClearJudge()
    {
        judgeText.text = "";
    }
}
