using UnityEngine;
using Live2D.Cubism.Framework;

public class N_222RabbitAnimation : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayTalking()
    {
        anim.SetTrigger("Rabbittalking");
	}
}
