using UnityEngine;
using System.Collections.Generic;

public class N_222JudgeManager : MonoBehaviour
{
    [Header("Judge Line")]
    [SerializeField] RectTransform judgeLine;

    [Header("Judge Range")]
    [SerializeField] float perfectRange = 10f;
    [SerializeField] float goodRange = 25f;

    List<N_222NoteBase> activeNotes = new();

    void Update()
    {
        HandleInput();
        CheckMiss();
    }

    void CheckMiss()
    {
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            var note = activeNotes[i];
            if (!note.gameObject.activeSelf) continue;

            float noteX = note.GetComponent<RectTransform>().anchoredPosition.x;
            float judgeX = judgeLine.anchoredPosition.x;

            if (noteX < judgeX - goodRange)
            {
                note.OnMiss();
                activeNotes.RemoveAt(i);
            }
        }
    }


    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Judge(KeyCode.LeftArrow);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            Judge(KeyCode.DownArrow);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            Judge(KeyCode.UpArrow);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            Judge(KeyCode.RightArrow);
    }

    public void RegisterNote(N_222NoteBase note)
    {
        activeNotes.Add(note);
    }

    void Judge(KeyCode key)
    {
        N_222NoteBase target = null;
        float minDistance = float.MaxValue;

        foreach (var note in activeNotes)
        {
            if (!note.gameObject.activeSelf) continue;
            if (note.InputKey != key) continue;

            float dist = Mathf.Abs(
                note.GetComponent<RectTransform>().anchoredPosition.x -
                judgeLine.anchoredPosition.x
            );

            if (dist < minDistance)
            {
                minDistance = dist;
                target = note;
            }
        }

        if (target == null) return;

        if (minDistance <= perfectRange)
        {
            target.OnPerfect();
            activeNotes.Remove(target);
        }
        else if (minDistance <= goodRange)
        {
            target.OnGood();
            activeNotes.Remove(target);
        }
    }
    public void ResetJudgeLine(Vector2 startPos)
    {
        judgeLine.anchoredPosition = startPos;
        activeNotes.Clear();
    }

}
