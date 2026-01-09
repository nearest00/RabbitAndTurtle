using System.Collections.Generic;
using UnityEngine;

public class N_222NoteManager : MonoBehaviour
{
    [SerializeField] private RectTransform noteParent;
    [SerializeField] private RectTransform[] noteSlots;
    [SerializeField] private N_222JudgeManager judgeManager;

    [Header("Prefabs")]
    [SerializeField] private N_222TapNote tapNotePrefab;
    [SerializeField] private N_222LongNote longNotePrefab;

    private readonly List<N_222NoteBase> activeNotes = new();

    public void SpawnRound(N_222RoundManager.RoundPattern pattern)
    {
        ClearAllNotes();

        for (int i = 0; i < 5; i++)
        {
            var data = pattern.notes[i];

            N_222NoteBase note;

            if (data.noteType == N_222NoteBase.NoteType.Tap)
            {
                var tap = Instantiate(tapNotePrefab, noteParent);
                tap.noteType = N_222NoteBase.NoteType.Tap;
                note = tap;
            }
            else
            {
                var longNote = Instantiate(longNotePrefab, noteParent);
                longNote.SetLongType(data.noteType);
                note = longNote;
            }

            note.Initialize(data.key, i, judgeManager);
            note.RectTransform.anchoredPosition = noteSlots[i].anchoredPosition;

            activeNotes.Add(note);
            judgeManager.RegisterNote(note);
        }
    }

    public void ClearAllNotes()
    {
        foreach (var note in activeNotes)
        {
            if (note != null)
                note.gameObject.SetActive(false);
        }
        activeNotes.Clear();
    }
}
