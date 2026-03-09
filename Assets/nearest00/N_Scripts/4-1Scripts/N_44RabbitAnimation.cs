using UnityEngine;

public class N_44RabbitAnimation : MonoBehaviour
{
	private Animator anim;

	void Awake()
	{
		anim = GetComponent<Animator>();
	}

	// 인풋 매니저가 키 입력 시 호출할 함수
	public void RabbitMove(string dir)
	{
		if (anim != null)
		{
			anim.SetTrigger(dir);
		}
	}
}