using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class N_222GuidePanelOff : MonoBehaviour
{
    [SerializeField] private GameObject[] guidePanel;
    int i = 0;
    public bool isCountingDown
    {
        get => PauseCountDown.Instance.isCounting;
        set => PauseCountDown.Instance.isCounting = value;
    }
    public bool CanSettingOn
    {
        get => SettingPanel.Instance.CanSettingOn;
        set => SettingPanel.Instance.CanSettingOn = value;
    }
    private bool tutorialing;
    private IEnumerator Start()
    {

        yield return null;
        CanSettingOn = false;
        if (SoundManager.Instance != null) SoundManager.Instance.PauseAllSounds();
        Time.timeScale = 0f;
        tutorialing = true;
        Debug.Log("기본세팅 완료");

    }
    void Update()
    {
        if(PauseCountDown.Instance!=null) if (isCountingDown) return;
        if (Input.GetMouseButtonDown(0)&&tutorialing)
        {
            if (guidePanel.Length == 0) return;

            guidePanel[i].SetActive(false);
            if (i<guidePanel.Length-1)
            {
                i++;
                guidePanel[i].SetActive(true);
            }
            else if(i==guidePanel.Length-1)
            {
                Debug.Log("튜토리얼 종료");
                StartResumeSequence();
                if(SettingPanel.Instance != null) CanSettingOn = true;
                tutorialing = false;
            }
        }
    }
    private void StartResumeSequence()
    {
        if (PauseCountDown.Instance != null)
        {
            isCountingDown = true; // 입력 차단 시작
            PauseCountDown.Instance.ResumeGameCountDown();
        }
        else Debug.Log("카운트다운 실패ㅋㅋ");
    }
}
