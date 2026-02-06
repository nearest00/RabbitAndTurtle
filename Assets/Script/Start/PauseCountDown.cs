using UnityEngine;
using TMPro;
using System.Collections;

public class PauseCountDown : MonoBehaviour
{
    // 어디서든 접근 가능하게 싱글톤 선언
    public static PauseCountDown Instance;

    public TextMeshProUGUI countdownText;
    public bool isCounting;

    private void Awake()
    {
        Instance = this;
    }

    public void ResumeGameCountDown()
    {
        StopAllCoroutines();
        StartCoroutine(ResumeSequence());
    }

    private IEnumerator ResumeSequence()
    {
        Debug.Log("카운트 코루틴 시작");
        isCounting = true;
        Time.timeScale = 0f;
        countdownText.gameObject.SetActive(true);
        int count = 3;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSecondsRealtime(1f);
            count--;
        }

        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(0.5f);

        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1f;
        SoundManager.Instance.ResumeAllSounds();
        isCounting = false;

        if (SettingPanel.Instance != null)
        {
            SettingPanel.Instance.SetIsCountingDown(false);
        }
    }
}