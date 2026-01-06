using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RTimingManager : MonoBehaviour
{
    public List<RNote> notes = new List<RNote>();
    public RectTransform judgmentLine;
    public float hitRange = 50f;

    public void CheckTiming(NoteDirection inputDir)
    {
        if (notes.Count == 0)
        {
            Debug.Log("Miss");
            return;
        }

        RNote note = notes[0];

        //     
        if (note.direction != inputDir)
        {
            Debug.Log("Wrong Key");
            return;
        }

        RectTransform noteRect = note.GetComponent<RectTransform>();
        float distance = Mathf.Abs(
            noteRect.anchoredPosition.y -
            judgmentLine.anchoredPosition.y
        );

        if (distance <= hitRange)
        {
            Debug.Log("Hit");
            note.Hit();
            notes.RemoveAt(0);
        }
        else
        {
            Debug.Log("Miss");
        }
    }
}
