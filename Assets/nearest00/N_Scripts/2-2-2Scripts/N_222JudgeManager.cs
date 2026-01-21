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
        // 1. 연타 노트 처리 (기존 유지)
        for (int i = 0; i < activeNotes.Count; i++)
        {
            var n = activeNotes[i];
            if (n != null && !n.IsFinished && n.noteType == N_222NoteBase.NoteType.ManyTap)
            {
                if (Input.GetKeyDown(n.inputKey))
                {
                    float dist = Mathf.Abs(n.RectTransform.anchoredPosition.x - judgeLine.anchoredPosition.x);
                    if (dist <= badRange * 2.0f)
                        n.GetComponent<N_222ManyTapNote>()?.OnManyTapInput();
                }
            }
        }

        // 2. 판정 루프 (멀티탭 및 일반 노트)
        // 리스트를 돌며 '이미지 하나(노트 하나)'만 판정하면 되므로 구조가 단순해집니다.
        for (int i = 0; i < activeNotes.Count; i++)
        {
            var note = activeNotes[i];

            if (note == null || note.isJudged || note.IsFinished ||
                note.noteType == N_222NoteBase.NoteType.ManyTap ||
                note.noteType == N_222NoteBase.NoteType.LongHold) continue;

            float dist = Mathf.Abs(note.RectTransform.anchoredPosition.x - judgeLine.anchoredPosition.x);
            if (dist > goodRange) continue;

            bool isHit = false;
            if (note.noteType == N_222NoteBase.NoteType.MultiTap)
            {
                // 이 로그가 콘솔에 찍히는지 보세요. 안 찍히면 리스트에 없는 겁니다.
                Debug.Log($"멀티탭 검사 중: {note.inputKey} + {note.inputKey2} / 거리: {dist}");

                if (Input.GetKey(note.inputKey) && Input.GetKey(note.inputKey2))
                {
                    Debug.Log("두 키 모두 눌림 감지!"); // 이게 뜨는지 확인
                    isHit = true;
                }
            }
            // 테스트용: HandleInput의 일부
            if (note.noteType == N_222NoteBase.NoteType.MultiTap)
            {
                // 거리 무시하고 키 상태만 체크
                if (Input.GetKey(note.inputKey) && Input.GetKey(note.inputKey2))
                {
                    Debug.Log("멀티탭 성공!");
                    isHit = true;
                }
            }
            // [멀티탭 판정 핵심] 한 이미지에 화살표 두 개가 있는 경우
            if (note.noteType == N_222NoteBase.NoteType.MultiTap)
            {
                // 조건: 두 키가 모두 '눌려(GetKey)' 있어야 하며, 
                // 그중 최소 하나는 '방금(GetKeyDown)' 눌린 상태여야 함
                bool key1Down = Input.GetKeyDown(note.inputKey);
                bool key2Down = Input.GetKeyDown(note.inputKey2);
                bool key1Pressed = Input.GetKey(note.inputKey);
                bool key2Pressed = Input.GetKey(note.inputKey2);

                if ((key1Down || key2Down) && key1Pressed && key2Pressed)
                {
                    isHit = true;
                }
            }
            // [일반 노트 판정]
            else if (Input.GetKeyDown(note.inputKey))
            {
                isHit = true;
            }

            if (isHit)
            {
                if (note.noteType == N_222NoteBase.NoteType.LongStart) holdingRoundID = note.roundID;
                note.isJudged = true;
                Judge(note, dist);
                break; // 한 프레임에 가장 가까운 노트 하나만 판정하고 루프 탈출
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

    // 롱노트 유지 및 떼기 로직 분리
    private void HandleLongNoteCheck()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow)) HandleKeyUp(KeyCode.LeftArrow);
        if (Input.GetKeyUp(KeyCode.RightArrow)) HandleKeyUp(KeyCode.RightArrow);
        if (Input.GetKeyUp(KeyCode.UpArrow)) HandleKeyUp(KeyCode.UpArrow);
        if (Input.GetKeyUp(KeyCode.DownArrow)) HandleKeyUp(KeyCode.DownArrow);

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