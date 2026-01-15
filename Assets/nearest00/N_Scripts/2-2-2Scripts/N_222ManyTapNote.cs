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
    }

    public override void OnGood() => OnPerfect();

    public override void OnMiss()
    {
        Debug.Log($"<color=red>[ManyTap Miss]</color> ID: {roundID}");
        isJudged = true;
        IsFinished = true;
        gameObject.SetActive(false);
    }
}