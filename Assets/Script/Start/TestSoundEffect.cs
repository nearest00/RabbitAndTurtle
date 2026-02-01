using UnityEngine;

public class TestSoundEffect : MonoBehaviour
{
    public AudioClip TestSound;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Sound();
        }    
    }
    void Sound()
    {
        SoundManager.Instance.PlaySFX(TestSound);
    }
}
