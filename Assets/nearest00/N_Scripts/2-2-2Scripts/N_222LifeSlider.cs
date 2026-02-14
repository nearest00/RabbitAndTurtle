using UnityEngine;
using UnityEngine.UI;

public class N_222LifeSlider : MonoBehaviour
{
    public static N_222LifeSlider Instance;
    public Slider targetSlider;
    private float internalValue = 0f;
    public float Max=1000;
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
        
    }
    private void UpdateSliderUI(float internalValue)
    {
        Debug.Log(internalValue);
        if (internalValue >= 0)
        {
            targetSlider.value = internalValue;
            Debug.Log(targetSlider.value);
        }
        else
        {
            targetSlider.value = 0f;
        }
    }
    public void AddValue(float amount)
    {
        Debug.Log(amount);
        internalValue += amount;
        Debug.Log(internalValue);
        internalValue = Mathf.Min(internalValue, Max);

        UpdateSliderUI(internalValue);
        Debug.Log("AddValue 실행");
    }
}
