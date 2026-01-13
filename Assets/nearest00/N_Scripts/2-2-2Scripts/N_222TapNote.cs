using UnityEngine;

public class N_222TapNote : N_222NoteBase
{
    public override void OnPerfect()
    {
        IsFinished = true;
        gameObject.SetActive(false);
        Debug.Log("<color=cyan>[Tap]</color> Perfect!");
    }

    public override void OnGood()
    {
        IsFinished = true;
        gameObject.SetActive(false);
        Debug.Log("<color=yellow>[Tap]</color> Good");
    }

    public override void OnMiss()
    {
        IsFinished = true;
        gameObject.SetActive(false);
        Debug.Log("<color=red>[Tap]</color> Miss...");
    }
}