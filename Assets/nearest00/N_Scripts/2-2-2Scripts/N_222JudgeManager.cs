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

            if (lineX > noteX + missBoundary)
            {
                if (n.noteType == N_222NoteBase.NoteType.ManyTap)
                {
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
        if (activeNotes.Count > 0 && (Input.anyKeyDown))
        {
            var firstNote = activeNotes[0];
            Debug.Log($"<color=yellow>[Judge]</color> 현재 리스트 노드 수: {activeNotes.Count}, 첫 노드 키: {firstNote.inputKey}, 타입: {firstNote.noteType}");
        }
        // 1. 연타 노트 처리 (기존 유지)
        for (int i = 0; i < activeNotes.Count; i++)
        {
            var n = activeNotes[i];
            if (n != null && !n.IsFinished && n.noteType == N_222NoteBase.NoteType.ManyTap)
            {
                if (Input.GetKeyDown(n.inputKey))
                {
                    float dist = Mathf.Abs(n.RectTransform.anchoredPosition.x - judgeLine.anchoredPosition.x);
                    if (dist <= badRange * 2.5f) // 연타는 좀 더 넉넉하게
                        n.GetComponent<N_222ManyTapNote>()?.OnManyTapInput();
                }
            }
        }

        // 2. 판정 루프 (단타 및 멀티탭)
        for (int i = 0; i < activeNotes.Count; i++)
        {
            var note = activeNotes[i];

            // 판정 제외 대상 필터링
            if (note == null || note.isJudged || note.IsFinished ||
                note.noteType == N_222NoteBase.NoteType.ManyTap ||
                note.noteType == N_222NoteBase.NoteType.LongHold) continue;

            float dist = Mathf.Abs(note.RectTransform.anchoredPosition.x - judgeLine.anchoredPosition.x);

            // [중요] 사용자가 미리 누르는 것을 고려해 판정 최대 거리(missBoundary) 안쪽이면 입력 체크 시작
            if (dist > missBoundary) continue;

            bool isHit = false;

            // --- 멀티탭 판정 ---
            if (note.noteType == N_222NoteBase.NoteType.MultiTap)
            {
                bool key1 = Input.GetKey(note.inputKey);
                bool key2 = Input.GetKey(note.inputKey2);
                bool eitherDown = Input.GetKeyDown(note.inputKey) || Input.GetKeyDown(note.inputKey2);

                // 둘 다 눌려있고, 그 중 하나는 방금 누른 상태여야 함
                if (eitherDown && key1 && key2)
                {
                    isHit = true;
                }
            }
            // --- 단타 판정 ---
            else
            {
                if (Input.GetKeyDown(note.inputKey))
                {
                    isHit = true;
                }
            }

            if (isHit)
            {
                // 거리와 상관없이 일단 'hit' 했으므로 판정 실행
                if (note.noteType == N_222NoteBase.NoteType.LongStart) holdingRoundID = note.roundID;
                note.isJudged = true;
                Judge(note, dist);
                // Debug.Log($"판정 성공: {note.noteType}, 거리: {dist}");
                break;
            }
            if (Input.anyKeyDown && !note.isJudged)
            {
                Debug.Log($"<color=cyan>[Check]</color> {note.inputKey} 입력 대기 중. 거리: {dist}");
            }
        }

        // 3. 롱노트 유지 및 떼기 처리
        HandleLongNoteExtra();
    }

    private void HandleLongNoteExtra()
    {
        // KeyUp 체크
        if (Input.GetKeyUp(KeyCode.LeftArrow)) HandleKeyUp(KeyCode.LeftArrow);
        if (Input.GetKeyUp(KeyCode.RightArrow)) HandleKeyUp(KeyCode.RightArrow);
        if (Input.GetKeyUp(KeyCode.UpArrow)) HandleKeyUp(KeyCode.UpArrow);
        if (Input.GetKeyUp(KeyCode.DownArrow)) HandleKeyUp(KeyCode.DownArrow);

        // Hold 체크
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