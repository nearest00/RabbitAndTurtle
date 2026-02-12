using UnityEngine;



public class BGMNoDifficulty : MonoBehaviour

{

    public AudioClip sceneBGM;

    void Start()

    {

        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM(sceneBGM);

    }

}