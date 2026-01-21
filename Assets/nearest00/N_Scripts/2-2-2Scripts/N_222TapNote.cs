using UnityEngine;

public class N_222TapNote : N_222NoteBase
{
    public override void OnPerfect()
    {
        Debug.Log($"[Note] Tap Perfect - ID: {roundID}");
        IsFinished = true;
        gameObject.SetActive(false);
        N_222LifeSlider.Instance.AddValue(10f);
        N_222JudgeEffectManager.Instance.ShowJudge("Perfect");
    }
    public override void OnGreat()
    {
        Debug.Log($"[Note] Tap Great - ID: {roundID}");
        IsFinished = true;
        gameObject.SetActive(false);
        N_222LifeSlider.Instance.AddValue(7f);
        N_222JudgeEffectManager.Instance.ShowJudge("Great");

    }
    public override void OnGood()
    {
        Debug.Log($"[Note] Tap Good - ID: {roundID}");
        IsFinished = true;
        gameObject.SetActive(false);
        N_222LifeSlider.Instance.AddValue(4f);
        N_222JudgeEffectManager.Instance.ShowJudge("good");
    }
    public override void OnBad()
    {
        Debug.Log($"[Note] Tap Bad - ID: {roundID}");
        IsFinished = true;
        gameObject.SetActive(false);
        N_222LifeSlider.Instance.AddValue(1f);
        N_222JudgeEffectManager.Instance.ShowJudge("bad");
    }
    public override void OnMiss()
    {
        Debug.Log($"[Note] Tap Miss - ID: {roundID}");
        IsFinished = true;
        gameObject.SetActive(false);
        N_222LifeSlider.Instance.AddValue(-50f);
        N_222JudgeEffectManager.Instance.ShowJudge("miss");
    }
}