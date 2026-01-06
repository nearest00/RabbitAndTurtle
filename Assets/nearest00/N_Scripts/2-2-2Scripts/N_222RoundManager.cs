using UnityEngine;
using System.Collections.Generic;

public class N_222RoundManager : MonoBehaviour
{
    [Header("Difficulty Patterns")]
    [SerializeField]
    List<N_222DifficultyRoundPattern> difficultyPatterns;

    [Header("Managers")]
    [SerializeField] N_222NoteManager noteManager;
    [SerializeField] N_222JudgeLine judgeLine;

    [Header("JudgeLine")]
    [SerializeField] float judgeLineStartX = -500f;

    N_222Difficulty currentDifficulty = N_222Difficulty.Easy;

    List<N_222RoundPattern> currentRounds;
    int currentRoundIndex = -1;

    void Start()
    {
        SelectDifficulty(currentDifficulty);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartNextRound();
        }
    }

    void SelectDifficulty(N_222Difficulty difficulty)
    {
        foreach (var set in difficultyPatterns)
        {
            if (set.difficulty == difficulty)
            {
                currentRounds = set.rounds;
                currentRoundIndex = -1;
                Debug.Log($"[N_222] Difficulty = {difficulty}");
                return;
            }
        }

        Debug.LogError($"[N_222] Difficulty {difficulty} not found");
    }

    void StartNextRound()
    {
        if (currentRounds == null || currentRounds.Count == 0)
            return;

        currentRoundIndex++;

        if (currentRoundIndex >= currentRounds.Count)
        {
            Debug.Log("[N_222] All rounds finished");
            return;
        }

        Debug.Log($"[N_222] Round {currentRoundIndex + 1}");

        judgeLine.ResetPosition(judgeLineStartX);
        noteManager.SpawnNotes(currentRounds[currentRoundIndex]);
    }
}
