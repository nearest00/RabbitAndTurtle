using UnityEngine;

public class Ending : MonoBehaviour
{
    public bool CanSettingOn
    {
        get => SettingPanel.Instance.CanSettingOn;
        set => SettingPanel.Instance.CanSettingOn = value;
    }
	public AudioClip clearSound;
	public AudioClip failedSound;
	private N_222RoundManager roundmng;
    [SerializeField] private GameObject ClearPanel;
    [SerializeField] private GameObject FailedPanel;

    public void StageClear()
    {
        CanSettingOn = false;
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlaySFX(clearSound);
        ClearPanel.SetActive(true);
    }
    public void StageFailed()
    {
        CanSettingOn = false;
       SoundManager.Instance.StopBGM();
		SoundManager.Instance.PlaySFX(failedSound);
		FailedPanel.SetActive(true);
    }
}
