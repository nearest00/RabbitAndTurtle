using UnityEngine;

public class N_222Ending : MonoBehaviour
{
    private N_222RoundManager roundmng;
    [SerializeField] private GameObject ClearPanel;
    [SerializeField] private GameObject FailedPanel;

    public void StageClear()
    {
        SoundManager.Instance.StopBGM();
        ClearPanel.SetActive(true);
    }
    public void StageFailed()
    {
       SoundManager.Instance.StopBGM();
       FailedPanel.SetActive(true);
    }
}
