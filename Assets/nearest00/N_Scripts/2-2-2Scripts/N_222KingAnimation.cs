using UnityEngine;

public class N_222KingAnimation : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayTalking()
    {
        anim.SetBool("KingTalking", true);
    }
    public void StopTalking()
    {
        anim.SetBool("KingTalking", false);
    }
}
