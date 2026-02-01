using System;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    [Header("캐릭터 위치")]
    public RectTransform[] CharacterSlots = new RectTransform[3];

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            
        }
    }
    
}
