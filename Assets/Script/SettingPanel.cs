using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private GameObject targetUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isActive = targetUI.activeSelf;
            targetUI.SetActive(!isActive);

            Time.timeScale = isActive ? 1f : 0f;
        }
    }

}
