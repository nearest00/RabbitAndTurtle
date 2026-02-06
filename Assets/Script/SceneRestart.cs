using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRestart : MonoBehaviour
{
    public void RestartCurrentScene()
    {
        // 1. 멈췄던 시간을 다시 흐르게 함 (이게 안 되면 다음 씬도 멈춤)
        Time.timeScale = 1f;
        SoundManager.Instance.ResetBGM();
        // 2. 현재 씬 이름을 가져와서 다시 로드
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);

        Debug.Log($"{sceneName} 씬을 완전히 새로 시작합니다.");
        
    }
}