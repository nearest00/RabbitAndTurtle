using UnityEngine;

public class N_221Ending : MonoBehaviour
{
    [SerializeField] private GameObject ClearPanel;
    [SerializeField] private GameObject FailedPanel;
    private N_221SFXList sfx;
    public bool CanSettingOn
    {
        get => SettingPanel.Instance.CanSettingOn;
        set => SettingPanel.Instance.CanSettingOn = value;
    }
    public void Start()
    {
        sfx = Object.FindFirstObjectByType<N_221SFXList>();

    }
    public void StageClear()
    {
        CanSettingOn = false;
        ClearPanel.SetActive(true);
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlaySFX(sfx.Clear);
        
    }
    public void StageFailed()
    {
        CanSettingOn = false;
        FailedPanel.SetActive(true);
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlaySFX(sfx.Failed);
    }
}
