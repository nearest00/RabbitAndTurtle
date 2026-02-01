using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; 

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private GameObject panelA;
    [SerializeField] private GameObject panelB;

    private bool isTransitioning = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (SettingPanel.Instance != null && SettingPanel.Instance.IsAnyPanelOpen())
            {
                return;
            } 

            if (isTransitioning) return;

            // 모든 관문을 통과하면 씬 이동 실행
            isTransitioning = true;
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.FadeAndLoadScene("2-2-2Game", 1.5f);
            }
        }
    }
}