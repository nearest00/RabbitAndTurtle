using UnityEngine;
using UnityEngine.UI;

public class RLifeSlider : MonoBehaviour
{
    public static RLifeSlider Instance;

    public Slider targetSlider;  // 연결 필수: ScoreSlider
    public float internalValue = 0f;
    public float Max = 550f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    void Start()
    {
        if (targetSlider == null) targetSlider = GetComponent<Slider>();
        // 안전 초기화: slider가 있으면 max 반영
        if (targetSlider != null) targetSlider.maxValue = Max;
        UpdateSliderUI();
    }

    // 외부에서 난이도 문자열을 받고 Max를 설정
    // diff expects "easy", "normal", "hard" (case-insensitive)
    public void SetDifficulty(string diff)
    {
        if (string.IsNullOrEmpty(diff)) diff = "easy";
        string d = diff.Trim().ToLower();

        if (d == "easy") Max = 550f;
        else if (d == "normal") Max = 800f;
        else if (d == "hard") Max = 1200f;
        else Max = 550f;

        if (targetSlider != null) targetSlider.maxValue = Max;

        // clamp internal value if needed
        internalValue = Mathf.Min(internalValue, Max);
        UpdateSliderUI();
    }

    public void SetMax(float max)
    {
        Max = max;
        if (targetSlider != null) targetSlider.maxValue = Max;
        internalValue = Mathf.Min(internalValue, Max);
        UpdateSliderUI();
    }

    public void ResetValue()
    {
        internalValue = 0f;
        UpdateSliderUI();
    }

    public void AddValue(float amount)
    {
        internalValue += amount;
        internalValue = Mathf.Min(internalValue, Max);
        if (internalValue < 0f) internalValue = 0f;
        UpdateSliderUI();
    }

    private void UpdateSliderUI()
    {
        if (targetSlider != null)
        {
            targetSlider.value = Mathf.Max(0f, internalValue);
        }
    }
}
