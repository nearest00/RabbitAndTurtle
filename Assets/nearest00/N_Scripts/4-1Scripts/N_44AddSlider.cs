using UnityEngine;

public class N_44AddSlider : MonoBehaviour
{
	public N_44LifeSlider N_44LifeSlider;
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			N_44LifeSlider.AddValue(50);
		}
	}
}
