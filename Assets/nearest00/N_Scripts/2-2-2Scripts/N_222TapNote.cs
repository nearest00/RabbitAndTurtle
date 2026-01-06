using UnityEngine;

public class N_222TapNote : N_222NoteBase
{
    public override void OnPerfect()
    {
        Debug.Log("ÆÛÆåÆ®");
        gameObject.SetActive(false);
    }

    public override void OnGood()
    {
        Debug.Log("±Â");
        gameObject.SetActive(false);
    }

    public override void OnMiss()
    {
        Debug.Log("¹Ì½º");
        gameObject.SetActive(false);
    }
}
