using UnityEngine;
using UnityEngine.UI;

public class N_221LifeSlider : MonoBehaviour
{
    public static N_221LifeSlider Instance;

    public Slider targetSlider;
    public float internalValue = 0f;
    public float Max;
    private string roundDifficulty;
    void Start()
    {
        if(Instance==null) Instance = this;
        if (targetSlider == null) targetSlider = GetComponent<Slider>();
        
        if (roundDifficulty == "easy")
        {
            Max = 550;
        }
        if (roundDifficulty == "normal")
        {
            Max = 800;
        }
        if (roundDifficulty == "hard")
        {
            Max = 1200;
        }
        else Max = 550;
            targetSlider.maxValue = Max;
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
