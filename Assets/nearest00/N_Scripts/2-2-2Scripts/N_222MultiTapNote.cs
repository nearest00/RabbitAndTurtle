using UnityEngine;

public class N_222MultiTapNote : N_222NoteBase
{
    public override void OnPerfect()
    {
        Debug.Log($"[Note] MultiTap Perfect! - ID: {roundID}, Keys: {inputKey} + {inputKey2}");
        IsFinished = true;
        gameObject.SetActive(false);
        N_222LifeSlider.Instance.AddValue(10f);
    }
    public override void OnGreat()
    {
        Debug.Log($"[Note] MultiTap Great - ID: {roundID}");
        IsFinished = true;
        gameObject.SetActive(false);
        N_222LifeSlider.Instance.AddValue(7f);
    }
    public override void OnGood()
    {
        Debug.Log($"[Note] MultiTap Good - ID: {roundID}");
        IsFinished = true;
        gameObject.SetActive(false);
        N_222LifeSlider.Instance.AddValue(4f);
    }
    public override void OnBad()
    {
        Debug.Log($"[Note] MultiTap Good - ID: {roundID}");
        IsFinished = true;
        gameObject.SetActive(false);
        N_222LifeSlider.Instance.AddValue(1f);
    }
    public override void OnMiss()
    {
        Debug.Log($"[Note] MultiTap Miss... - ID: {roundID}");
        IsFinished = true;
        gameObject.SetActive(false);
        N_222LifeSlider.Instance.AddValue(-50f);
    }
}