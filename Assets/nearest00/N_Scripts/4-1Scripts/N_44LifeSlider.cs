using UnityEngine;
using UnityEngine.UI;

public class N_44LifeSlider : MonoBehaviour
{
    public static N_44LifeSlider Instance;

    public Slider targetSlider;
    public float internalValue = 0f;
    public float Max;
    private string roundDifficulty;

    void Awake()
    {
        // 싱글톤 초기화는 Awake에서 하는 것이 가장 안전합니다.
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (targetSlider == null) targetSlider = GetComponent<Slider>();
        if (N_StageSellectButton.Instance != null) roundDifficulty = N_StageSellectButton.Instance.StageDifficulty;
        if (roundDifficulty == "easy")
        {
            Max = 550;
        }
        else if (roundDifficulty == "normal")
        {
            Max = 800;
        }
        else if (roundDifficulty == "hard")
            Max = 1200;
        if (N_StageSellectButton.Instance != null)
        {
            roundDifficulty = N_StageSellectButton.Instance.StageDifficulty.ToLower().Trim();
            Debug.Log($"현재 감지된 난이도: {roundDifficulty}");
        }
        else
        {
            Debug.LogWarning("StageSelectButton 인스턴스가 없습니다!");
        }
        UpdateSliderUI();
    }

    private void UpdateSliderUI()
    {
        if (targetSlider != null)
        {
            targetSlider.value = Mathf.Clamp(internalValue, 0, Max);
        }
    }

    public void AddValue(float amount)
    {
        internalValue += amount;
        internalValue = Mathf.Clamp(internalValue, 0, Max);
        UpdateSliderUI();
    }
}