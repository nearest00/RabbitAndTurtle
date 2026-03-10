using UnityEngine;
using Live2D.Cubism.Framework;

public class TA : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void TJump()
    {
        anim.SetTrigger("TTriggerJ");
    }
    public void TSlid()
    {
        anim.SetTrigger("TTriggerS");
    }
}