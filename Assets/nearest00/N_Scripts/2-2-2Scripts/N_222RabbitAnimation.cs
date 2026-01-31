using UnityEngine;

public class N_222RabbitAnimation : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayTalking()
    {
        anim.SetBool("RabbitTalking", true);
    }
    public void StopTalking()
    {
        anim.SetBool("RabbitTalking", false);
    }
}
