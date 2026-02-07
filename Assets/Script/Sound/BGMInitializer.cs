using UnityEngine;

public class BGMInitializer : MonoBehaviour
{
    public AudioClip sceneBGM;
    void Start()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM(sceneBGM);
    }
}