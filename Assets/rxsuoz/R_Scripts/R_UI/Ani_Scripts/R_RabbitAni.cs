using UnityEngine;

public class R_RabbitAni : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Sliding()
    {
        anim.SetBool("Sliding_Rabbit", true);
    }
    public void StopSliding()
    {
        anim.SetBool("Sliding_Rabbit", false);
    }
}