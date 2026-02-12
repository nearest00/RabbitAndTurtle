using UnityEngine;

public class n_221InfiniteBG : MonoBehaviour
{
	[Header("Settings")]
	public float scrollSpeed = 5f;
	public float backgroundWidth; // 이미지의 가로 길이

	void Start()
	{
		// SpriteRenderer에서 자동으로 가로 길이를 가져오기
		if (backgroundWidth <= 0)
		{
			backgroundWidth = GetComponent<SpriteRenderer>().bounds.size.x;
		}
	}

	void Update()
	{
		// 1. 오른쪽으로 이동
		transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime);

		if (transform.position.x >= 30)
		{
			transform.position -= new Vector3(backgroundWidth * 2f, 0, 0);
		}
	}
}