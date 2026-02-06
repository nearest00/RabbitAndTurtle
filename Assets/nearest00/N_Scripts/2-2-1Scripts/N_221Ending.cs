using UnityEngine;

public class N_221Ending : MonoBehaviour
{
    [SerializeField] private GameObject ClearPanel;
    [SerializeField] private GameObject FailedPanel;
    private N_221SFXList sfx;
    public void Start()
    {
        sfx = Object.FindFirstObjectByType<N_221SFXList>();

    }
    public void StageClear()
    {
        ClearPanel.SetActive(true);
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlaySFX(sfx.Clear);
        
    }
    public void StageFailed()
    {
        FailedPanel.SetActive(true);
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlaySFX(sfx.Failed);
    }
}
