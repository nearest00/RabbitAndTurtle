using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class N_222GuidePanelOff : MonoBehaviour
{
    [SerializeField] private GameObject[] guidePanel = new GameObject[3];
    int i = 0;
    private void Start()
    {
        Time.timeScale = 0f;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            guidePanel[i].SetActive(false);
            if (i < 2)
            {
                guidePanel[i + 1].SetActive(true);
                i++;
            }
            if (i == 2)
            {

            }
        }
    }
}
