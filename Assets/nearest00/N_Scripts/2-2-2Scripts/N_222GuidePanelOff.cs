using UnityEngine;
using System;
using System.Collections.Generic;

public class N_222GuidePanelOff : MonoBehaviour
{
    [SerializeField] private GameObject guidePanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Time.timeScale = 0f;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            guidePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
