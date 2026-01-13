using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private GameObject panelA;
    [SerializeField] private GameObject panelB;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 하나라도 켜져 있으면 입력 차단
            if (panelA.activeSelf || panelB.activeSelf)
                return;

            SceneManager.LoadScene("MiniGame1");
        }
    }
}
