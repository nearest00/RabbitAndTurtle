using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class N_222JudgeManager : MonoBehaviour
{
    [SerializeField] private RectTransform judgeLine;
    [SerializeField] private float visualOffset = -35f;
    [SerializeField] private float perfectRange = 25f;
    [SerializeField] private float goodRange = 50f;
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
            if (n == null || n.IsFinished || n.isJudged) continue;

            float noteX = n.RectTransform.anchoredPosition.x - visualOffset;
            if (lineX > noteX + missBoundary)
            {
                if (IsLongNote(n.noteType))
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
        for (int i = 0; i < activeNotes.Count; i++)
        {
            var n = activeNotes[i];
            if (n != null && !n.IsFinished && n.noteType == N_222NoteBase.NoteType.ManyTap)
            {
                if (Input.GetKeyDown(n.inputKey))
                {
                    float dist = Mathf.Abs((n.RectTransform.anchoredPosition.x - judgeLine.anchoredPosition.x) - visualOffset);
                    if (dist <= goodRange * 2.0f) n.GetComponent<N_222ManyTapNote>()?.OnManyTapInput();
                }
            }
        }

        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (key == KeyCode.Space) continue;
            if (Input.GetKeyDown(key)) HandleKeyDown(key);
            if (Input.GetKeyUp(key)) HandleKeyUp(key);
        }

        if (holdingRoundID != -1)
        {
            for (int i = activeNotes.Count - 1; i >= 0; i--)
            {
                var n = activeNotes[i];
                if (n.roundID == holdingRoundID && n.noteType == N_222NoteBase.NoteType.LongHold && !n.isJudged)
                {
                    float dist = Mathf.Abs((n.RectTransform.anchoredPosition.x - judgeLine.anchoredPosition.x) - visualOffset);
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
            if (note == null || note.IsFinished || note.isJudged) continue;
            if (note.noteType == N_222NoteBase.NoteType.ManyTap) continue;

            float dist = Mathf.Abs((note.RectTransform.anchoredPosition.x - judgeLine.anchoredPosition.x) - visualOffset);
            if (dist > goodRange) continue;

            if (note.noteType == N_222NoteBase.NoteType.MultiTap)
            {
                if (note.inputKey == key || note.inputKey2 == key)
                {
                    if (Input.GetKey(note.inputKey) && Input.GetKey(note.inputKey2))
                    {
                        note.isJudged = true;
                        Judge(note, dist);
                    }
                }
            }
            else if (note.inputKey == key)
            {
                if (note.noteType == N_222NoteBase.NoteType.LongStart)
                {
                    holdingRoundID = note.roundID;
                }
                note.isJudged = true;
                Judge(note, dist);
            }
        }
    }

    private void HandleKeyUp(KeyCode key)
    {
        if (holdingRoundID == -1) return;

        int targetID = holdingRoundID;
        bool isCorrectKey = false;
        N_222NoteBase endNote = null;

        // 현재 홀딩 중인 그룹의 키인지, 꼬리가 존재하는지 확인
        foreach (var n in activeNotes)
        {
            if (n.roundID == targetID && n.inputKey == key)
            {
                isCorrectKey = true;
                if (n.noteType == N_222NoteBase.NoteType.LongEnd) endNote = n;
            }
        }

        if (isCorrectKey)
        {
            if (endNote != null)
            {
                float dist = Mathf.Abs((endNote.RectTransform.anchoredPosition.x - judgeLine.anchoredPosition.x) - visualOffset);
                // 꼬리 판정 범위 내에서 뗐을 때
                if (dist <= goodRange)
                {
                    endNote.isJudged = true;
                    Judge(endNote, dist);
                    ClearLongGroup(targetID);
                }
                else
                {
                    // 꼬리가 아직 멀었는데 뗐을 때 (도중 이탈 미스)
                    FailLongGroup(targetID, "Early Release Miss");
                }
            }
            else
            {
                // 꼬리 자체가 아직 생성 전이거나 리스트에 없을 때 뗐을 경우
                FailLongGroup(targetID, "Incomplete Hold Miss");
            }
            holdingRoundID = -1;
        }
    }

    // 롱노트 실패 처리 및 로그 출력
    private void FailLongGroup(int rID, string reason)
    {
        Debug.Log($"<color=red>[Long Group Failed]</color> ID: {rID} | Reason: {reason}");
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            if (activeNotes[i].roundID == rID && IsLongNote(activeNotes[i].noteType))
            {
                if (!activeNotes[i].isJudged)
                {
                    activeNotes[i].OnMiss(); // 미판정 조각들 미스 처리
                }
                activeNotes[i].IsFinished = true;
                activeNotes[i].gameObject.SetActive(false);
            }
        }
    }

    private void ClearLongGroup(int rID)
    {
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            if (activeNotes[i].roundID == rID && IsLongNote(activeNotes[i].noteType))
            {
                activeNotes[i].IsFinished = true;
                activeNotes[i].gameObject.SetActive(false);
            }
        }
    }

    private bool IsLongNote(N_222NoteBase.NoteType type) =>
        type == N_222NoteBase.NoteType.LongStart || type == N_222NoteBase.NoteType.LongHold || type == N_222NoteBase.NoteType.LongEnd;

    private void Judge(N_222NoteBase note, float dist)
    {
        if (dist <= perfectRange) note.OnPerfect(); else note.OnGood();
    }
}