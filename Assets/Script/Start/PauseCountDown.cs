using UnityEngine;
using TMPro;
using System.Collections;

public class PauseCountDown : MonoBehaviour
{
    // 어디서든 접근 가능하게 싱글톤 선언
    public static PauseCountDown Instance;

    public TextMeshProUGUI countdownText;

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null) Instance = this;
    }

    public void ResumeGameCountDown()
    {
        // 중복 실행 방지
        StopAllCoroutines();
        StartCoroutine(ResumeSequence());
    }

    private IEnumerator ResumeSequence()
    {
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
        if (SettingPanel.Instance != null)
        {
            SettingPanel.Instance.SetIsCountingDown(false);
        }
    }
}