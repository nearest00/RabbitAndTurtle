using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; 

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private GameObject panelA;
    [SerializeField] private GameObject panelB;

    private bool isTransitioning = false;
    public bool CanSettingOn
    {
        get => SettingPanel.Instance.CanSettingOn;
        set => SettingPanel.Instance.CanSettingOn = value;
    }
    [SerializeField] private GameObject resetBtnObject; // 버튼 오브젝트 자체를 연결 (스크립트 말고)

    public void SetResetButtonActive(bool value)
    {
        if (resetBtnObject != null)
        {
            // 1. 오브젝트가 꺼져 있어도 일단 가져옵니다.
            var script = resetBtnObject.GetComponent<ResetGameButton>();
            if (script != null)
            {
                script.SetInteractable(value);
            }
            else
            {
                // 스크립트를 못 찾을 경우를 대비해 직접 Button 컴포넌트 건드리기
                var btn = resetBtnObject.GetComponent<Button>();
                if (btn != null) btn.interactable = value;
            }
        }
    }

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
            CanSettingOn = false;
            SetResetButtonActive(true);
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.FadeAndLoadScene("2-2-1Game", 1.5f);
            }
        }
    }
}