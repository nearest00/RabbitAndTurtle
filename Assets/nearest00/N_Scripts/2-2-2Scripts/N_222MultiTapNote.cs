using UnityEngine;

public class N_222MultiTapNote : N_222NoteBase
{
    public override void OnPerfect()
    {
        Debug.Log($"[Note] MultiTap Perfect! - ID: {roundID}, Keys: {inputKey} + {inputKey2}");
        IsFinished = true;
        gameObject.SetActive(false);
    }
    public override void OnGood()
    {
        Debug.Log($"[Note] MultiTap Good - ID: {roundID}");
        OnPerfect();
    }
    public override void OnMiss()
    {
        Debug.Log($"[Note] MultiTap Miss... - ID: {roundID}");
        IsFinished = true;
        gameObject.SetActive(false);
    }
}