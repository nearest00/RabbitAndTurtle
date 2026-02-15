using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class R3_GameManager : MonoBehaviour
{
    public static R3_GameManager Instance;

    [Header("Song Data")]
    public RRSongData songData;
    public AudioSource audioSource;
    public float manualSongLength;

    [Header("Managers")]
    public R3_NoteManager noteManager;

    [Header("UI & Prefabs")]
    public Slider scoreSlider;
    public GameObject judgePopupPrefab;
    public Transform judgeParent;

    private double _songStartTime;
    private float _gameTimer = 0f;
    private bool _isGamePlaying = false; // 카운트다운 후 true로 변경됨

    void Awake() => Instance = this;

    // 외부 카운트다운 스크립트에서 호출하는 함수
    public void StartMusic()
    {
        _songStartTime = AudioSettings.dspTime;
        audioSource.Play();
        _isGamePlaying = true;
        Debug.Log("Game Started!");
    }

    void Update()
    {
        // 게임 플레이 중이 아니거나 일시정지 중이면 중단
        if (!_isGamePlaying || Time.timeScale <= 0) return;

        // DSP 타임 기반 현재 진행 시간 계산
        _gameTimer = (float)(AudioSettings.dspTime - _songStartTime);

        // 슬라이더 값 업데이트
        if (RLifeSlider.Instance != null && scoreSlider != null)
        {
            scoreSlider.value = RLifeSlider.Instance.internalValue;
        }

        // 곡 종료 체크
        if (_gameTimer >= manualSongLength)
        {
            _isGamePlaying = false;
            // Ending.StageClear() 등 종료 로직 호출
        }
    }

    public double GetCurrentTime() => AudioSettings.dspTime - _songStartTime;

    public void CreateJudgePopup(string label)
    {
        if (judgePopupPrefab == null) return;
        GameObject go = Instantiate(judgePopupPrefab, judgeParent);
        go.transform.localPosition = Vector3.zero;
        go.GetComponent<R3_JudgePopup>().Play(label);
    }
}