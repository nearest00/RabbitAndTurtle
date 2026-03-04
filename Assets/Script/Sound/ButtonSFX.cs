using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonSFX : MonoBehaviour
{
	private void Start()
	{
		// 그냥 실행하지 말고, 코루틴으로 아주 잠깐만 쉬었다가 실행
		StartCoroutine(SetupWithDelay());
	}

	private System.Collections.IEnumerator SetupWithDelay()
	{
		// SoundManager가 나타날 때까지 한 프레임 쉽니다.
		yield return null;

		// 이제 사운드 매니저가 확실히 존재하므로 버튼 세팅 시작
		SetupAllButtonSounds();
		Debug.Log("첫 씬 버튼 사운드 세팅 완료!");
	}
	public AudioClip ButtonSound;
    private void OnEnable()
    {
        // 씬 로드 이벤트 연결
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupAllButtonSounds();
    }
    public void SetupAllButtonSounds()
    {
        // 1. 해당 씬의 SFXList를 찾음
        if (ButtonSound == null ||ButtonSound == null) return;

        // 2. 비활성 포함 모든 버튼 찾기
        Button[] allButtons = Object.FindObjectsByType<Button>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Button btn in allButtons)
        {
            if (btn.CompareTag("NoSound")) continue;
            btn.onClick.RemoveListener(PlayClickSound);
            btn.onClick.AddListener(PlayClickSound);
        }
		Debug.Log(allButtons.Length + "개의 버튼에 소리를 입혔습니다.");
	}

	private void PlayClickSound()
	{
		// 1. 매니저가 없으면 직접 찾기 (싱글톤이 늦게 깨어날 경우 대비)
		if (SoundManager.Instance == null)
		{
			var manager = Object.FindFirstObjectByType<SoundManager>();
			if (manager != null) { /* 여기서 강제로 재생 */ }
		}

		// 2. 오디오 소스 강제 활성화
		var source = SoundManager.Instance.sfxSource;
		if (!source.enabled) source.enabled = true;
		if (source.mute) source.mute = false;
		source.volume = 1f; // 일단 소리 나는지 확인용으로 1로 고정

		source.PlayOneShot(ButtonSound);
		Debug.Log($"[강제재생] {ButtonSound.name} 재생함. 소스상태: {source.enabled}");
	}
}
