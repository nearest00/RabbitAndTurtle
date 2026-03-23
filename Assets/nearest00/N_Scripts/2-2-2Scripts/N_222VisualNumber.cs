using UnityEngine;
using TMPro;

public class N_222VisualNumber : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI targetText;
	N_222LifeSlider slider;
	public float internalValue
	{
		get => N_222LifeSlider.Instance.internalValue;
		set => N_222LifeSlider.Instance.internalValue = value;
	}

	public void UpdateDisplay()
	{
		if (targetText != null)
		{
			// A / B 형식으로 문자열 조합
			targetText.text = $"{internalValue} / 1000";
			if (internalValue < 0) targetText.color = new Color(1, 0, 0, 1f);
		}
	}
	private void Update()
	{
		UpdateDisplay();
	}
}
