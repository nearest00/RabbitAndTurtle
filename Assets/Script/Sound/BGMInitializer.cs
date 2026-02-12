using UnityEngine;


public class BGMInitializer : MonoBehaviour
{
    public string currentDifficulty
    {
        get => N_StageSellectButton.Instance.StageDifficulty;
        set => N_StageSellectButton.Instance.StageDifficulty = value;
    }
    // 인스펙터에서 난이도별로 브금을 넣을 수 있게 구성
    [Header("Difficulty BGM Settings")]
    public AudioClip easyBGM;
    public AudioClip normalBGM;
    public AudioClip hardBGM;

    void Start()
    {
        if (SoundManager.Instance != null)
        {
            AudioClip clipToPlay = GetClipByDifficulty(currentDifficulty);
            SoundManager.Instance.PlayBGM(clipToPlay);
        }
    }

    private AudioClip GetClipByDifficulty(string difficulty)
    {
        switch (difficulty)
        {
            case "easy": return easyBGM;
            case "normal": return normalBGM;
            case "hard": return hardBGM;
            default: return normalBGM;
        }
    }
}