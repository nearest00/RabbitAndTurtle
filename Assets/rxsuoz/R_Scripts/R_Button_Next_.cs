using UnityEngine;
using UnityEngine.SceneManagement;

public class R_Button_Next : MonoBehaviour
{
    //인스펙터에서 이동할 씬 이름을 적어줘
    [SerializeField] private string sceneName;

    //버튼이 눌렸을 때 호출
    public void MoveToScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[R_SceneButton] 이동할 씬 이름이 설정되지 않았습니다!");
            return;
        }

        // 🔹 씬 전환 실행
        SceneManager.LoadScene(sceneName);
    }

    // 🔹 종료 버튼 만들고 싶다면 여기에 추가 가능
    public void QuitGame()
    {
        Debug.Log("[R_SceneButton] 게임 종료");
        Application.Quit();
    }
}
