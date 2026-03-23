using UnityEngine;

public class N_222AddSlider : MonoBehaviour
{
    public N_222LifeSlider N_222LifeSlider;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            N_222LifeSlider.AddValue(50);
        }
    }
}
