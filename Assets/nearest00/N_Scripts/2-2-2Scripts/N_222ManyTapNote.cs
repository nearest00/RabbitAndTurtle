using UnityEngine;

public class N_222ManyTapNote : N_222NoteBase
{
    [SerializeField] private int requiredTapCount = 10;
    private int currentTapCount = 0;
    private bool isFailed = false;

    public void OnManyTapInput()
    {
        if (IsFinished) return;

        // 연타를 한 번이라도 누르면 AutoMissCheck에서 보호하기 위해 true 설정
        isJudged = true;

        currentTapCount++;
        Debug.Log($"<color=white>[ManyTap]</color> {currentTapCount}/{requiredTapCount}");

        if (currentTapCount >= requiredTapCount)
        {
            OnPerfect();
        }
    }

    public override void OnPerfect()
    {
        if (IsFinished) return;
        Debug.Log($"<color=lime>[ManyTap Perfect]</color>");
        IsFinished = true;
        gameObject.SetActive(false);
        N_222LifeSlider.Instance.AddValue(10f);
    }
    public override void OnGreat() => OnPerfect();
    public override void OnGood() => OnPerfect();
    public override void OnBad() => OnPerfect();
    // JudgeManager의 AutoMissCheck에서 호출함
    public void CheckFinalResult()
    {
        if (!IsFinished && currentTapCount < requiredTapCount)
        {
            OnMiss();
        }
    }

    public override void OnMiss()
    {
        if (isFailed) return;
        isFailed = true;
        Debug.Log($"<color=red>[ManyTap Miss]</color> 횟수 부족: {currentTapCount}");
        IsFinished = true;
        gameObject.SetActive(false);
        N_222LifeSlider.Instance.AddValue(-50f);
    }
}