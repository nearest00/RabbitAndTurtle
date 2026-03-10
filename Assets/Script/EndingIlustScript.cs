using UnityEngine;
using System.Collections;

public class EndingIlustScript : MonoBehaviour
{
	public CanvasGroup endingPanelGroup;
	private bool canClick = false; // 클릭 감지 시작 플래그

	public void ShowEnding()
	{
		// 이미 실행 중일 수 있으니 코루틴 시작
		StartCoroutine(EndingSequence());
	}

	private IEnumerator EndingSequence()
	{
		// 1. 페이드 인 (1초 동안 서서히 나타남)
		float timer = 0f;
		endingPanelGroup.interactable = true;
		endingPanelGroup.blocksRaycasts = true;

		while (timer < 1.0f)
		{
			timer += Time.deltaTime;
			endingPanelGroup.alpha = Mathf.Lerp(0, 1, timer / 1.0f);
			yield return null;
		}
		endingPanelGroup.alpha = 1f;

		// 2. 패널이 다 켜진 후 5초간 대기
		Debug.Log("패널 활성화 완료, 5초 대기 시작...");
		yield return new WaitForSeconds(5f);

		// 3. 5초 뒤에 클릭 가능하도록 플래그 변경
		canClick = true;
		Debug.Log("이제 클릭하면 로그가 찍힙니다!");
	}

	void Update()
	{
		// 4. 플래그가 true일 때만 마우스 클릭 감지
		if (canClick)
		{
			if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
			{
				Debug.Log("엔딩 패널 활성화 5초 후 마우스 클릭 감지됨!");
			}
		}
	}
}