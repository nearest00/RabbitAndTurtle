using UnityEngine;

public class N_44JudgementManager : MonoBehaviour
{
    // 판정 범위 (초 단위)
    public const float PERFECT_WINDOW = 0.05f;
    public const float GREAT_WINDOW = 0.10f;
    public const float GOOD_WINDOW = 0.15f;
    public const float BAD_WINDOW = 0.20f;
    public const float MISS_WINDOW = 0.25f;
    N_44LifeSlider Lifeslider;

    public enum Judge { Perfect, Great, Good, Bad, Miss, None }

    public Judge GetJudgement(float diff)
    {
        float absDiff = Mathf.Abs(diff);

        if (absDiff <= PERFECT_WINDOW)
        {            
            N_44LifeSlider.Instance.AddValue(10);
            return Judge.Perfect;
        }
        if (absDiff <= GREAT_WINDOW)
        {
            N_44LifeSlider.Instance.AddValue(7);
            return Judge.Great;
        }
        if (absDiff <= GOOD_WINDOW)
        {
            N_44LifeSlider.Instance.AddValue(4);
            return Judge.Good;
        }
        if (absDiff <= BAD_WINDOW)
        {
            N_44LifeSlider.Instance.AddValue(1);
            return Judge.Bad;
        }
        else
        {
            N_44LifeSlider.Instance.AddValue(-50f);
            return Judge.Miss;
        }
    }
}