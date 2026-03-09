using UnityEngine;

public class N_44TigerAnimation : MonoBehaviour
{
	private Animator anim;

	void Awake()
	{
		anim = GetComponent<Animator>();
	}

	// 가이드 노트가 박자에 맞춰서 호출할 함수
	public void TigerMove(string dir)
	{
		if (anim != null)
		{
			// 입력받은 dir(Up, Down 등) 앞에 "Tiger"를 붙여서 트리거 실행
			anim.SetTrigger("Tiger" + dir);
			// Debug.Log($"호랑이 재생: Tiger{dir}"); // 안 움직이면 주석 해제해서 확인
		}
	}
}