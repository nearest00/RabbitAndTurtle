using UnityEngine;
using UnityEngine.UI;

public class N_222LongNote : N_222NoteBase
{
    public override void OnPerfect()
    {
        Debug.Log($"<color=lime>[Perfect]</color> {noteType} (ID: {roundID})");
        if (noteType == NoteType.LongStart || noteType == NoteType.LongHold)
        {
            isJudged = true;
            IsFinished = false;
        }
        else if (noteType == NoteType.LongEnd)
        {
            isJudged = true;
            IsFinished = true;
            gameObject.SetActive(false);
        }
    }

    public override void OnGood()
    {
        Debug.Log($"<color=yellow>[Good]</color> {noteType} (ID: {roundID})");
        OnPerfect();
    }

    public override void OnMiss()
    {
        Debug.Log($"<color=red>[Miss]</color> {noteType} (ID: {roundID})");
        isJudged = true;
        IsFinished = true;
        gameObject.SetActive(false);
    }
}