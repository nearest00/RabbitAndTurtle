using UnityEngine;

public class N_222RoundManager : MonoBehaviour
{
    // ======================
    // Difficulty
    // ======================
    public enum N_222Difficulty
    {
        Easy,
        Normal,
        Difficult
    }

    [Header("Difficulty")]
    [SerializeField] private N_222Difficulty currentDifficulty = N_222Difficulty.Easy;

    // ======================
    // Round Data
    // ======================
    [System.Serializable]
    public class NoteSpawnData
    {
        public KeyCode key;               // 입력 키
        public Vector2 anchoredPosition;  // UI 기준 위치
    }

    [System.Serializable]
    public class RoundPattern
    {
        public NoteSpawnData[] notes;     // 한 라운드에 등장할 노트들
    }

    [Header("Round Patterns")]
    [SerializeField] private RoundPattern[] easyRounds;
    [SerializeField] private RoundPattern[] normalRounds;
    [SerializeField] private RoundPattern[] difficultRounds;

    // ======================
    // Managers
    // ======================
    [Header("Managers")]
    [SerializeField] private N_222NoteManager noteManager;
    [SerializeField] private N_222JudgeManager judgeManager;

    [Header("Judge Line")]
    [SerializeField] private Vector2 judgeLineStartPos;

    private int currentRound = 0;

    // ======================
    // Unity Loop
    // ======================
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartNextRound();
        }
    }

    // ======================
    // Round Control
    // ======================
    void StartNextRound()
    {
        RoundPattern[] rounds = GetCurrentDifficultyRounds();

        if (rounds == null || currentRound >= rounds.Length)
            return;

        // 이전 라운드 정리
        noteManager.ClearAllNotes();
        judgeManager.ResetJudgeLine(judgeLineStartPos);

        // 새 라운드 시작
        noteManager.SpawnRound(rounds[currentRound]);

        currentRound++;
    }

    RoundPattern[] GetCurrentDifficultyRounds()
    {
        switch (currentDifficulty)
        {
            case N_222Difficulty.Easy:
                return easyRounds;
            case N_222Difficulty.Normal:
                return normalRounds;
            case N_222Difficulty.Difficult:
                return difficultRounds;
            default:
                return null;
        }
    }

    // ======================
    // External Control (나중 확장용)
    // ======================
    public void SetDifficulty(N_222Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        currentRound = 0;
    }
}
