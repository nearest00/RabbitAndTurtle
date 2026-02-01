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
            // [방어막 1] 마우스가 UI(버튼 등) 위에 있다면 즉시 중단
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // [방어막 2] 설정 패널들이 하나라도 켜져 있다면 중단
            // 여기서 panelA, panelB가 SettingPanel의 basePanel, soundPanel인지 확인하세요!
            if (panelA.activeSelf || panelB.activeSelf)
            {
                return;
            }

            // [방어막 3] 이미 씬 전환 중이라면 중단
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