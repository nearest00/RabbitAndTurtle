using UnityEngine;
using UnityEngine.UI;
using System.Collections; // 코루틴 사용을 위해 필요

public class RLifeSlider : MonoBehaviour
{
    public static RLifeSlider Instance;

    public Slider targetSlider;
    public float internalValue = 0f;
    public float Max;
    private string roundDifficulty;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (targetSlider == null) targetSlider = GetComponent<Slider>();

        // 직접 호출하는 대신, 인스턴스를 찾을 때까지 기다리는 코루틴 시작
        StartCoroutine(WaitAndSetDifficulty());
    }

    private IEnumerator WaitAndSetDifficulty()
    {
        // 1. N_StageSellectButton.Instance가 생길 때까지 무한 대기 (한 프레임씩 쉬면서 체크)
        while (N_StageSellectButton.Instance == null)
        {
            yield return null;
        }

        // 2. 인스턴스를 찾았다면 난이도 값 가져오기
        roundDifficulty = N_StageSellectButton.Instance.StageDifficulty.ToLower();
        Debug.Log($"[RLifeSlider] 인스턴스 발견! 선택된 난이도: {roundDifficulty}");

        // 3. 난이도에 따른 Max 값 설정
        switch (roundDifficulty)
        {
            case "easy": Max = 550f; break;
            case "normal": Max = 800f; break;
            case "hard": Max = 1200f; break;
            default: Max = 550f; break;
        }

        if (targetSlider != null)
        {
            targetSlider.maxValue = Max;
        }

        UpdateSliderUI();
    }

    private void UpdateSliderUI()
    {
        if (targetSlider == null) return;
        targetSlider.value = internalValue > 0 ? internalValue : 0f;
    }

    public void AddValue(float amount)
    {
        internalValue += amount;
        internalValue = Mathf.Min(internalValue, Max);
        UpdateSliderUI();
    }
}