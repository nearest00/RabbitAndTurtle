using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

public class N_StageSellectButton : MonoBehaviour
{
    [SerializeField] public GameObject DifficultPanel;
    public static N_StageSellectButton Instance;
    private int Stage;
    public string StageDifficulty;
    public bool CanSettingOn
    {
        get => SettingPanel.Instance.CanSettingOn;
        set => SettingPanel.Instance.CanSettingOn = value;
    }
    void Start()
    {
        CanSettingOn = false;
        Instance=this;
    }
    public void OnClick111StageButton()
    {
        Stage = 2; //씬번호입력
        DifficultPanel.SetActive(true);
    }
    public void OnClick222StageButton()
    {
        Stage = 3; //씬번호입력
        DifficultPanel.SetActive(true);
    }
    public void OnClick333StageButton()
    {
        Stage = 4; //씬번호입력(임시임)
        DifficultPanel.SetActive(true);
    }
    public void OnClick444StageButton()
    {
        Stage = 3; //씬번호입력(임시임)
        DifficultPanel.SetActive(true);
    }

    public void OnClickEasyButton()
    {
        StageDifficulty = "easy";
        GotoScene();
    }
    public void OnClickNormalButton()
    {
        StageDifficulty = "normal";
        GotoScene();
    }
    public void OnClickHardButton()
    {
        StageDifficulty = "hard";
        GotoScene();
    }
    public void OnClickUndoButton()
    {
        DifficultPanel.SetActive(false);
    }
    void GotoScene()
    {
        CanSettingOn = false;
        EditorSceneManager.LoadScene(Stage);
    }
}
