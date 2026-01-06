using UnityEngine;
using System.Collections.Generic;

public class N_222JudgeManager : MonoBehaviour
{
    [SerializeField] RectTransform judgeLine;
    [SerializeField] float perfectRange = 20f;
    [SerializeField] float goodRange = 40f;

    Dictionary<KeyCode, List<N_222Note>> noteMap = new();

    void Awake()
    {
        noteMap[KeyCode.LeftArrow] = new();
        noteMap[KeyCode.DownArrow] = new();
        noteMap[KeyCode.UpArrow] = new();
        noteMap[KeyCode.RightArrow] = new();
    }

    void Update()
    {
        HandleInput(KeyCode.LeftArrow);
        HandleInput(KeyCode.DownArrow);
        HandleInput(KeyCode.UpArrow);
        HandleInput(KeyCode.RightArrow);
    }

    public void RegisterNote(N_222Note note)
    {
        noteMap[note.InputKey].Add(note);
    }

    void HandleInput(KeyCode key)
    {
        if (!Input.GetKeyDown(key)) return;

        N_222Note target = null;
        float minDist = float.MaxValue;

        foreach (var note in noteMap[key])
        {
            float dist = Mathf.Abs(
                note.Rect.anchoredPosition.x -
                judgeLine.anchoredPosition.x
            );

            if (dist < minDist)
            {
                minDist = dist;
                target = note;
            }
        }

        if (target == null) return;

        if (minDist <= perfectRange) target.OnPerfect();
        else if (minDist <= goodRange) target.OnGood();
        else target.OnMiss();

        noteMap[key].Remove(target);
    }
}
