using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResetGameButton : MonoBehaviour
{
    private Button myButton;

    // 오브젝트가 꺼져 있어도 수동으로 초기화해서 쓸 수 있게 만듭니다.
    public void SetInteractable(bool value)
    {
        if (myButton == null) myButton = GetComponent<Button>();

        if (myButton != null)
        {
            myButton.interactable = value;
            Debug.Log($"[재시작 버튼] Interactable -> {value}");
        }
    }
}