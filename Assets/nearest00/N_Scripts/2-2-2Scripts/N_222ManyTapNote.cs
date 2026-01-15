using UnityEngine;

public class N_222ManyTapNote : N_222NoteBase
{
    [SerializeField] private int requiredTapCount = 10;
    private int currentTapCount = 0;

    public void OnManyTapInput()
    {
        currentTapCount++;
        if (currentTapCount >= requiredTapCount) OnPerfect();
    }

    public override void OnPerfect()
    {
        Debug.Log($"<color=lime>[ManyTap Perfect]</color> ID: {roundID}");
        isJudged = true;
        IsFinished = true;
        gameObject.SetActive(false);
        N_222LifeSlider.Instance.AddValue(10f);

    }

    public override void OnGreat() => OnPerfect();
    public override void OnGood() => OnPerfect();
    public override void OnBad() => OnPerfect();


    public override void OnMiss()
    {
        Debug.Log($"<color=red>[ManyTap Miss]</color> ID: {roundID}");
        isJudged = true;
        IsFinished = true;
        gameObject.SetActive(false);
        N_222LifeSlider.Instance.AddValue(-50f);
    }
}