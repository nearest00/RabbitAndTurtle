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
        anim.SetTrigger("rabbittalk");
    }
}
