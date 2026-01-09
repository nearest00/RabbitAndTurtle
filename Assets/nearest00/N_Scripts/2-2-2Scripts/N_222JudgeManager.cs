using System.Collections.Generic;
using UnityEngine;

public class N_222JudgeManager : MonoBehaviour
{
    [SerializeField] private RectTransform judgeLine;

    [Header("Judge Slot")]
    [SerializeField] private RectTransform[] noteSlots; // 0~4

    [Header("Judge Range")]
    [SerializeField] private float perfectRange = 20f;
    [SerializeField] private float goodRange = 40f;

    private readonly List<N_222NoteBase> activeNotes = new();

    public int CurrentJudgeSlot { get; private set; }

    /* =========================
     * Register
     * ========================= */

    public void RegisterNote(N_222NoteBase note)
    {
        if (!activeNotes.Contains(note))
            activeNotes.Add(note);
    }

    public void UnregisterNote(N_222NoteBase note)
    {
        activeNotes.Remove(note);
    }


    void Update()
    {
        UpdateJudgeSlot();
        HandleInput();
        AutoMissCheck();
    }

    /* =========================
     * Judge Line
     * ========================= */

    void UpdateJudgeSlot()
    {
        float judgeX = judgeLine.anchoredPosition.x;

        for (int i = 0; i < noteSlots.Length; i++)
        {
            if (judgeX < noteSlots[i].anchoredPosition.x)
            {
                CurrentJudgeSlot = Mathf.Max(0, i - 1);
                return;
            }
        }

        CurrentJudgeSlot = noteSlots.Length - 1;
    }
    /* =========================
 * Judge Line Reset
 * ========================= */

    public void ResetJudgeLine(Vector2 startPos)
    {
        if (judgeLine == null)
        {
            Debug.LogWarning("JudgeLine is not assigned.");
            return;
        }

        judgeLine.anchoredPosition = startPos;
    }

    /* =========================
     * Input
     * ========================= */

    void HandleInput()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                    HandleKeyDown(key);
            }
        }

        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(key))
                HandleKeyHold(key);

            if (Input.GetKeyUp(key))
                HandleKeyUp(key);
        }
    }

    void HandleKeyDown(KeyCode key)
    {
        foreach (var note in activeNotes)
        {
            if (note.IsFinished) continue;
            if (note.inputKey != key) continue;

            if (note.noteType == N_222NoteBase.NoteType.Tap)
            {
                JudgeTap(note);
                return;
            }

            if (note.noteType == N_222NoteBase.NoteType.LongStart &&
                note.slotIndex == CurrentJudgeSlot)
            {
                note.OnInputDown();
                return;
            }
        }
    }

    void HandleKeyHold(KeyCode key)
    {
        foreach (var note in activeNotes)
        {
            if (note.IsFinished) continue;
            if (note.inputKey != key) continue;

            if (note.noteType == N_222NoteBase.NoteType.LongStart)
            {
                note.OnInputHold();
                return;
            }
        }
    }

    void HandleKeyUp(KeyCode key)
    {
        foreach (var note in activeNotes)
        {
            if (note.IsFinished) continue;
            if (note.inputKey != key) continue;

            if (note.noteType == N_222NoteBase.NoteType.LongEnd &&
                note.slotIndex == CurrentJudgeSlot)
            {
                note.OnInputUp();
                return;
            }
        }
    }

    /* =========================
     * Tap Judge
     * ========================= */

    void JudgeTap(N_222NoteBase note)
    {
        float distance = Mathf.Abs(
            judgeLine.anchoredPosition.x -
            noteSlots[note.slotIndex].anchoredPosition.x
        );

        if (distance <= perfectRange)
        {
            note.OnPerfect();
            UnregisterNote(note);
        }
        else if (distance <= goodRange)
        {
            note.OnGood();
            UnregisterNote(note);
        }
    }

    /* =========================
     * Auto Miss
     * ========================= */

    void AutoMissCheck()
    {
        foreach (var note in activeNotes)
        {
            if (note.IsFinished)
                continue;

            if (CurrentJudgeSlot > note.slotIndex)
            {
                note.OnMiss();
            }
        }
    }

    /* =========================
     * Long Group Finish
     * ========================= */

    public void FinishLongGroup(int groupId)
    {
        foreach (var note in activeNotes)
        {
            if (note is N_222LongNote longNote &&
                longNote.longGroupId == groupId)
            {
                longNote.gameObject.SetActive(false);
            }
        }
    }
}
