using UnityEngine;

public class N_222TapNote : N_222NoteBase
{
    protected override void Awake()
    {
        base.Awake();
        noteType = NoteType.Tap;
    }

    public override void OnPerfect()
    {
        Debug.Log("Tap Perfect");
        isFinished = true;
        gameObject.SetActive(false);
    }

    public override void OnGood()
    {
        Debug.Log("Tap Good");
        isFinished = true;
        gameObject.SetActive(false);
    }

    public override void OnMiss()
    {
        if (isFinished) return;
        Debug.Log("Tap Miss");
        isFinished = true;
        gameObject.SetActive(false);
    }
}
