using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonSFX : MonoBehaviour
{
    public AudioClip clickSound;
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
        if (clickSound == null ||clickSound == null) return;

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
    }

    void PlayButtonSound()
    {
        if (SoundManager.Instance != null && clickSound != null)
        {
            SoundManager.Instance.PlaySFX(clickSound);
        }
    }
    private void PlayClickSound()
    {
        // 현재 씬의 SFXList를 매번 찾아서 소리를 재생
        if (clickSound != null)
        {
            SoundManager.Instance.PlaySFX(clickSound);
        }
    }
}
