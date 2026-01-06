using UnityEngine;
using System.Collections.Generic;

public class N_222NoteManager : MonoBehaviour
{
    [SerializeField] N_222JudgeManager judgeManager;
    [SerializeField] N_222Note notePrefab;
    [SerializeField] Transform noteParent;

    List<N_222Note> activeNotes = new();

    public void ClearNotes()
    {
        foreach (var note in activeNotes)
        {
            if (note != null)
                Destroy(note.gameObject);
        }
        activeNotes.Clear();
    }

    public void SpawnNotes(N_222RoundPattern pattern)
    {
        ClearNotes();

        foreach (var data in pattern.notes)
        {
            N_222Note note = Instantiate(notePrefab, noteParent);
            note.Initialize(data.key, data.anchoredPosition);
            judgeManager.RegisterNote(note);
            activeNotes.Add(note);
        }
    }
}
