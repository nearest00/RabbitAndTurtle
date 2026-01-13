using System.Collections.Generic;
using UnityEngine;

public class N_222JudgeManager : MonoBehaviour
{
    [SerializeField] private RectTransform judgeLine;
    [Header("Judge Ranges")]
    [SerializeField] private float visualOffset = -35f;
    [SerializeField] private float perfectRange = 15f;
    [SerializeField] private float goodRange = 35f;
    [SerializeField] private float missBoundary = 120f;

    private List<N_222NoteBase> activeNotes = new List<N_222NoteBase>();
    private int currentHoldingRoundID = -1;

    public void RegisterNote(N_222NoteBase note) { if (note != null) activeNotes.Add(note); }

    public void ResetJudgeLine(Vector2 startPos)
    {
        if (judgeLine != null) judgeLine.anchoredPosition = startPos;
        foreach (var n in activeNotes) if (n != null) n.gameObject.SetActive(false);
        activeNotes.Clear();
        currentHoldingRoundID = -1;
        Debug.Log("<color=yellow>Judge Manager Cleared for New Round</color>");
    }

    void Update()
    {
        if (judgeLine == null) return;
        HandleInput();
        AutoMissCheck();
        // 판정 완료된 노지 실시간 정리
        activeNotes.RemoveAll(n => n == null || n.IsFinished || !n.gameObject.activeInHierarchy);
    }

    private void HandleInput()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key)) HandleKeyDown(key);
            if (Input.GetKeyUp(key)) HandleKeyUp(key);
        }
    }

    private void HandleKeyDown(KeyCode key)
    {
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            var note = activeNotes[i];
            if (note.IsFinished || note.inputKey != key) continue;

            float dist = (note.RectTransform.anchoredPosition.x - judgeLine.anchoredPosition.x) - visualOffset;
            float absDist = Mathf.Abs(dist);

            if (note is N_222ManyTapNote many)
            {
                if (absDist <= (note.RectTransform.rect.width * 0.5f + goodRange)) many.OnManyTapInput();
                continue;
            }

            if (absDist <= goodRange)
            {
                if (note.noteType == N_222NoteBase.NoteType.Tap)
                {
                    if (absDist <= perfectRange) note.OnPerfect(); else note.OnGood();
                }
                else if (note.noteType == N_222NoteBase.NoteType.LongStart)
                {
                    note.OnInputDown();
                    currentHoldingRoundID = note.roundID;
                }
            }
        }
    }

    private void HandleKeyUp(KeyCode key)
    {
        if (currentHoldingRoundID == -1) return;
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            var note = activeNotes[i];
            if (note.IsFinished || note.inputKey != key || note.roundID != currentHoldingRoundID) continue;

            if (note.noteType == N_222NoteBase.NoteType.LongEnd)
            {
                float dist = Mathf.Abs((note.RectTransform.anchoredPosition.x - judgeLine.anchoredPosition.x) - visualOffset);
                if (dist <= goodRange) note.OnInputUp();
                else FailLongGroup(key, currentHoldingRoundID);
            }
            else FailLongGroup(key, currentHoldingRoundID);
        }
    }

    private void FailLongGroup(KeyCode key, int rID)
    {
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            var n = activeNotes[i];
            if (n.inputKey == key && n.roundID == rID && n is N_222LongNote) n.OnMiss();
        }
        if (currentHoldingRoundID == rID) currentHoldingRoundID = -1;
    }

    private void AutoMissCheck()
    {
        float lineX = judgeLine.anchoredPosition.x;
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            var n = activeNotes[i];
            if (n.IsFinished) continue;
            if (n.roundID == currentHoldingRoundID && n.noteType != N_222NoteBase.NoteType.LongEnd) continue;

            if (lineX > (n.RectTransform.anchoredPosition.x - visualOffset) + missBoundary)
            {
                if (n is N_222LongNote) FailLongGroup(n.inputKey, n.roundID); else n.OnMiss();
            }
        }
    }
}