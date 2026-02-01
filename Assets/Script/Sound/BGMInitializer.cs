using UnityEngine;

public class BGMInitializer : MonoBehaviour
{
    public AudioClip sceneBGM;
    void Start() => SoundManager.Instance.PlayBGM(sceneBGM);
}