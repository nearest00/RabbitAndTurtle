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
        if (SoundManager.Instance == null) return;
        SoundManager.Instance.PlaySFX(TestSound);
    }
}
