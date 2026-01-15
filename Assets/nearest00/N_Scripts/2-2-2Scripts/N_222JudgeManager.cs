using System.Collections.Generic;
using UnityEngine;

public class N_222JudgeManager : MonoBehaviour
{
    [SerializeField] private RectTransform judgeLine;
    [SerializeField] private float perfectRange = 15f;
    [SerializeField] private float greatRange = 35f;
    [SerializeField] private float goodRange = 60f;
    [SerializeField] private float badRange = 90f;
    [SerializeField] private float missBoundary = 180f;

    private List<N_222NoteBase> activeNotes = new List<N_222NoteBase>();
    private int holdingRoundID = -1;

    public void RegisterNote(N_222NoteBase note) { if (note != null) activeNotes.Add(note); }

    public void ResetJudgeLine(Vector2 startPos)
    {
        if (judgeLine != null) judgeLine.anchoredPosition = startPos;
        foreach (var n in activeNotes) if (n != null) n.gameObject.SetActive(false);
        activeNotes.Clear();
        holdingRoundID = -1;
    }

    void Update()
    {
        HandleInput();
        AutoMissCheck();
        activeNotes.RemoveAll(n => n == null || n.IsFinished);
    }

    private void AutoMissCheck()
    {
        float lineX = judgeLine.anchoredPosition.x;
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            var n = activeNotes[i];
            if (n == null || n.IsFinished) continue;

            float noteX = n.RectTransform.anchoredPosition.x;

            // 판정선이 노트를 완전히 지나쳤을 때
            if (lineX > noteX + missBoundary)
            {
                if (n.noteType == N_222NoteBase.NoteType.ManyTap)
                {
                    // 연타노트는 지나가는 순간 최종 결과 확인 (횟수 채웠는지)
                    n.GetComponent<N_222ManyTapNote>()?.CheckFinalResult();
                }
                else if (n.isJudged) continue;
                else if (IsLongNote(n.noteType))
                {
                    FailLongGroup(n.roundID, "Pass Miss");
                }
                else
                {
                    n.OnMiss();
                }
            }
        }
    }

    private void HandleInput()
    {
        // 1. 연타 노트 입력 별도 처리
        for (int i = 0; i < activeNotes.Count; i++)
        {
            var n = activeNotes[i];
            if (n != null && !n.IsFinished && n.noteType == N_222NoteBase.NoteType.ManyTap)
            {
                if (Input.GetKeyDown(n.inputKey))
                {
                    float dist = Mathf.Abs(n.RectTransform.anchoredPosition.x - judgeLine.anchoredPosition.x);
                    // 연타는 판정선 근처(Good x 2)에서만 입력 받음
                    if (dist <= badRange * 2.0f)
                        n.GetComponent<N_222ManyTapNote>()?.OnManyTapInput();
                }
            }
        }

        // 2. 일반 및 롱노트 입력 처리
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (key == KeyCode.Space) continue;
            if (Input.GetKeyDown(key)) HandleKeyDown(key);
            if (Input.GetKeyUp(key)) HandleKeyUp(key);
        }

        // 3. 롱노트 유지(Hold) 판정
        if (holdingRoundID != -1)
        {
            foreach (var n in activeNotes)
            {
                if (n.roundID == holdingRoundID && n.noteType == N_222NoteBase.NoteType.LongHold && !n.isJudged)
                {
                    float dist = Mathf.Abs(n.RectTransform.anchoredPosition.x - judgeLine.anchoredPosition.x);
                    if (dist <= perfectRange) n.OnPerfect();
                }
            }
        }
    }

    private void HandleKeyDown(KeyCode key)
    {
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            var note = activeNotes[i];
            if (note == null || note.IsFinished || note.isJudged || note.noteType == N_222NoteBase.NoteType.ManyTap) continue;

            float dist = Mathf.Abs(note.RectTransform.anchoredPosition.x - judgeLine.anchoredPosition.x);
            if (dist > goodRange) continue;

            if (note.noteType == N_222NoteBase.NoteType.MultiTap)
            {
                if ((note.inputKey == key || note.inputKey2 == key) && Input.GetKey(note.inputKey) && Input.GetKey(note.inputKey2))
                {
                    note.isJudged = true;
                    Judge(note, dist);
                }
            }
            else if (note.inputKey == key)
            {
                if (note.noteType == N_222NoteBase.NoteType.LongStart) holdingRoundID = note.roundID;
                note.isJudged = true;
                Judge(note, dist);
            }
        }
    }

    private void HandleKeyUp(KeyCode key)
    {
        if (holdingRoundID == -1) return;

        N_222NoteBase endNote = null;
        bool isCorrectKey = false;

        foreach (var n in activeNotes)
        {
            if (n.roundID == holdingRoundID && n.inputKey == key)
            {
                isCorrectKey = true;
                if (n.noteType == N_222NoteBase.NoteType.LongEnd) endNote = n;
            }
        }

        if (isCorrectKey)
        {
            if (endNote != null)
            {
                float dist = Mathf.Abs(endNote.RectTransform.anchoredPosition.x - judgeLine.anchoredPosition.x);
                if (dist <= goodRange) { endNote.isJudged = true; Judge(endNote, dist); ClearLongGroup(holdingRoundID); }
                else FailLongGroup(holdingRoundID, "Early Release");
            }
            else FailLongGroup(holdingRoundID, "Incomplete");

            holdingRoundID = -1;
        }
    }

    private void Judge(N_222NoteBase note, float dist)
    {
        if (dist <= perfectRange) note.OnPerfect();
        else if (dist <= greatRange) note.OnGreat();
        else if (dist <= goodRange) note.OnGood();
        else if (dist <= badRange) note.OnBad();
        else note.OnMiss();
    }

    private void FailLongGroup(int rID, string reason)
    {
        Debug.Log($"<color=red>[Long Fail]</color> ID: {rID} ({reason})");
        foreach (var n in activeNotes)
        {
            if (n.roundID == rID && IsLongNote(n.noteType))
            {
                if (!n.isJudged) n.OnMiss();
                n.IsFinished = true;
                n.gameObject.SetActive(false);
            }
        }
    }

    private void ClearLongGroup(int rID)
    {
        foreach (var n in activeNotes)
        {
            if (n.roundID == rID && IsLongNote(n.noteType))
            {
                n.IsFinished = true;
                n.gameObject.SetActive(false);
            }
        }
    }

    private bool IsLongNote(N_222NoteBase.NoteType type) =>
        type == N_222NoteBase.NoteType.LongStart || type == N_222NoteBase.NoteType.LongHold || type == N_222NoteBase.NoteType.LongEnd;
}