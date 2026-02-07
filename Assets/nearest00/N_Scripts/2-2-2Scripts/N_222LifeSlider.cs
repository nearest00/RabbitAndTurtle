using UnityEngine;
using UnityEngine.UI;

public class N_222LifeSlider : MonoBehaviour
{
    public static N_222LifeSlider Instance;
    public Slider targetSlider;
    private float internalValue = 0f;
    public float Max;
    private string roundDifficulty;
    void Awake()
    {
        // 싱글톤 중복 방지 로직
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        if (targetSlider == null) targetSlider = GetComponent<Slider>();
        if (N_222RoundManager.Instance != null)
        {
            roundDifficulty = N_222RoundManager.Instance.currentDifficulty;
        }
        else
        {
            Debug.LogError("RoundManager가 씬에 없습니다!");
            return;
        }
    }
    private void UpdateSliderUI()
    {
        if (internalValue > 0)
        {
            targetSlider.value = internalValue;
        }
        else
        {
            targetSlider.value = 0f;
        }
    }
    public void AddValue(float amount)
    {
        internalValue += amount;

        internalValue = Mathf.Min(internalValue, Max);

        UpdateSliderUI();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
