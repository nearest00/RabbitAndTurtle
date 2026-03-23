using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    // 현재 설정된 볼륨 값 (설정창과 동기화됨)
    public float BgmVolume { get; private set; } = 1f;
    public float SfxVolume { get; private set; } = 1f;

    private Coroutine fadeCoroutine;

    private bool isPaused = false;
    public bool CanSettingOn
    {
        get => SettingPanel.Instance.CanSettingOn;
        set => SettingPanel.Instance.CanSettingOn = value;
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else { Destroy(gameObject); }
    }

    // [볼륨 제어] 설정창 슬라이더에서 호출
    public void SetBgmVolume(float val)
    {
        BgmVolume = val;

        // 페이드 아웃 코루틴이 돌고 있다면 볼륨 조절을 방해하므로 강제 종료
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        // 실제 오디오 소스에 값 적용
        bgmSource.volume = BgmVolume;

        // 만약 0으로 줄였는데도 소리가 나면 소스 자체를 뮤트하거나 정지
        bgmSource.mute = (BgmVolume <= 0.001f);
    }

    public void SetSfxVolume(float value)
    {
        SfxVolume = value;
        // SFX는 재생 시점에 볼륨을 적용하므로 변수만 업데이트
    }

    // [배경음 재생] 씬별 BGMInitializer가 호출
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip&&bgmSource.isPlaying) return;
        bgmSource.clip = clip;
        bgmSource.volume = BgmVolume; // 기존 설정 볼륨으로 재생
        bgmSource.Play();
    }

    // [효과음 재생] 씬 내의 어떤 스크립트든 호출 가능
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || isPaused) return;
        sfxSource.PlayOneShot(clip, SfxVolume);
    }

    // [페이드 아웃] 씬 전환 시 호출
    public void FadeOutBGM(float duration)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(DoFadeOut(duration));
    }

    private IEnumerator DoFadeOut(float duration)
    {
        float startVol = bgmSource.volume;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVol, 0, t / duration);
            yield return null;
        }
        bgmSource.volume = 0;
        bgmSource.Stop();
    }
    public void FadeAndLoadScene(string sceneName, float duration)
    {
        StartCoroutine(FadeAndLoadRoutine(sceneName, duration));
    }
    public void StageFadeAndLoadScene(int sceneName, float duration)
    {
        StartCoroutine(StageFadeAndLoadRoutine(sceneName, duration));
    }
	public void StageFadeAndTimeStopLoadScene(int sceneName, float duration)
	{
		StartCoroutine(StageFadeAndTimeStopLoadRoutine(sceneName, duration));
	}

	private IEnumerator FadeAndLoadRoutine(string sceneName, float duration)
    {
		Debug.Log("1");
		FadeOutBGM(duration);
		Debug.Log("2");
		yield return new WaitForSecondsRealtime(duration);
		Debug.Log("3");
		CanSettingOn = true;
		Debug.Log("4");
		SceneManager.LoadScene(sceneName);
    }
    private IEnumerator StageFadeAndLoadRoutine(int sceneName, float duration)
    {
        Debug.Log("열심히 실행 중...");
        FadeOutBGM(duration);
        Debug.Log("1");
        yield return new WaitForSeconds(duration);
		Debug.Log("2");
		CanSettingOn = true;
		Debug.Log("3");
		SceneManager.LoadScene(sceneName);
        Debug.Log("실행 완료");
    }
	private IEnumerator StageFadeAndTimeStopLoadRoutine(int sceneName, float duration)
	{
		FadeOutBGM(duration);
		yield return new WaitForSeconds(duration);
		CanSettingOn = true;
		SceneManager.LoadScene(sceneName);
        Time.timeScale = 0f;
	}
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 이동 후 볼륨 자동 복구
        bgmSource.volume = BgmVolume;
    }
    public void PauseAllSounds()
    {
        isPaused = true;
        if (bgmSource.isPlaying)
        {
            bgmSource.Pause();
        }
        sfxSource.Stop();
    }
    public void ResumeAllSounds()
    {
        isPaused = false;
        bgmSource.UnPause();
        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }
	public void StopLoopingSFX(AudioSource source)
	{
		if (source != null)
		{
			source.Stop();
			Destroy(source.gameObject); // 생성했던 객체 삭제
		}
	}
	public AudioSource PlayLoopingSFX(AudioClip clip)
	{
		if (clip == null || isPaused) return null;

		// 새로운 오디오 소스 생성 (효과음 전용)
		GameObject go = new GameObject("LoopingSFX_" + clip.name);
		AudioSource source = go.AddComponent<AudioSource>();

		source.clip = clip;
		source.volume = SfxVolume;
		source.loop = true; // 루프 설정
		source.Play();

		return source; // 나중에 Stop하기 위해 반환
	}
	public void StopBGM()
    {
        bgmSource.Pause();
    }
    public void ResetBGM()
    {
        bgmSource.Stop();
    }
}