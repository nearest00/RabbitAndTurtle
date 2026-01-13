using UnityEngine;

public class N_222RoundManager : MonoBehaviour
{
    [System.Serializable]
    public class NoteData
    {
        public KeyCode key;
        public N_222NoteBase.NoteType noteType;
    }

    [System.Serializable]
    public class RoundPattern
    {
        public NoteData[] notes = new NoteData[5];
    }

    [SerializeField] private RoundPattern[] easyRounds;
    [SerializeField] private N_222NoteManager noteManager;
    [SerializeField] private N_222JudgeManager judgeManager;
    [SerializeField] private Vector2 judgeLineStartPos;

    private int currentRound;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartNextRound();
        }
    }

    void StartNextRound()
    {
        if (currentRound >= easyRounds.Length)
            return;

        // JudgeManager에 ResetJudgeLine 메서드가 있어야 함
        judgeManager.ResetJudgeLine(judgeLineStartPos);

        // NoteManager에 SpawnRound 메서드가 있어야 함
        noteManager.SpawnRound(easyRounds[currentRound]);

        currentRound++;
    }
}