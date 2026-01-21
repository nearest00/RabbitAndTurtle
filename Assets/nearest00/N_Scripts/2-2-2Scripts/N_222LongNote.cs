using UnityEngine;
using UnityEngine.UI;

public class N_222LongNote : N_222NoteBase
{
    public override void OnPerfect()
    {
        Debug.Log($"<color=lime>[Perfect]</color> {noteType} (ID: {roundID})");

        N_222LifeSlider.Instance.AddValue(10f);
        N_222JudgeEffectManager.Instance.ShowJudge("perfect");
        HandleNoteStatus();
    }

    public override void OnGreat()
    {
        Debug.Log($"<color=yellow>[Great</color> {noteType} (ID: {roundID})");
        N_222LifeSlider.Instance.AddValue(7f);
        N_222JudgeEffectManager.Instance.ShowJudge("great");

        HandleNoteStatus();
    }
    public override void OnGood()
    {
        Debug.Log($"<color=yellow>[Good]</color> {noteType} (ID: {roundID})");
        N_222LifeSlider.Instance.AddValue(4f);
        N_222JudgeEffectManager.Instance.ShowJudge("good");
        HandleNoteStatus();
    }
    public override void OnBad()
    {
        Debug.Log($"<color=yellow>[Bad]</color> {noteType} (ID: {roundID})");
        N_222LifeSlider.Instance.AddValue(1f);
        N_222JudgeEffectManager.Instance.ShowJudge("bad");
        HandleNoteStatus();
    }

    public override void OnMiss()
    {
        Debug.Log($"<color=red>[Miss]</color> {noteType} (ID: {roundID})");
        isJudged = true;
        IsFinished = true;
        gameObject.SetActive(false);
        N_222LifeSlider.Instance.AddValue(-50f);
        N_222JudgeEffectManager.Instance.ShowJudge("miss");

    }
    private void HandleNoteStatus()
    {
        if (noteType == NoteType.LongStart || noteType == NoteType.LongHold)
        {
            isJudged = true;
            IsFinished = false; // 계속 유지
        }
        else if (noteType == NoteType.LongEnd)
        {
            isJudged = true;
            IsFinished = true;
            gameObject.SetActive(false); // 꼬리가 처리되면 비활성화
        }
    }
}