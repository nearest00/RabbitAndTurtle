using UnityEngine;
using UnityEngine.UI;

public class RLifeSlider : MonoBehaviour
{
    public static RLifeSlider Instance;

    public Slider targetSlider;
    public float internalValue = 0f;
    public float Max;
    private string roundDifficulty;

    void Start()
    {
        if (Instance == null) Instance = this;
        if (targetSlider == null) targetSlider = GetComponent<Slider>();
        
        UpdateSliderUI();
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
}
