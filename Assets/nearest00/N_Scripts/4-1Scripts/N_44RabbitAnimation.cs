using UnityEngine;

public class N_44RabbitAnimation : MonoBehaviour
{
	private Animator anim;

	void Awake()
	{
		anim = GetComponent<Animator>();
	}

	public void RabbitMove(string dir)
	{
		if (anim != null)
		{
			// 트리거 이름이 "RabbitUp", "RabbitDown" 등인지 다시 확인!
			anim.SetTrigger("Rabbit" + dir);
			Debug.Log($"토끼 애니메이션 호출: Rabbit{dir}"); // <-- 이 로그가 찍히는지 보세요
		}
	}
}