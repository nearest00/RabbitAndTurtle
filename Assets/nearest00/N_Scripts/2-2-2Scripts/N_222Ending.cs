using UnityEngine;

public class N_222Ending : MonoBehaviour
{
    public bool CanSettingOn
    {
        get => SettingPanel.Instance.CanSettingOn;
        set => SettingPanel.Instance.CanSettingOn = value;
    }
    private N_222RoundManager roundmng;
    [SerializeField] private GameObject ClearPanel;
    [SerializeField] private GameObject FailedPanel;

    public void StageClear()
    {
        CanSettingOn = false;
        SoundManager.Instance.StopBGM();
        ClearPanel.SetActive(true);
    }
    public void StageFailed()
    {
        CanSettingOn = false;
       SoundManager.Instance.StopBGM();
       FailedPanel.SetActive(true);
    }
}
