using UnityEngine;

public class Fish : MonoBehaviour
{
    public int lane;
    private Vector3 targetPos;
    public float lerpSpeed = 20f;

    public void SetPosition(int targetLane, float yPos, float xPos)
    {
        lane = targetLane;
        targetPos = new Vector3(xPos, yPos, 0);
        transform.position = targetPos; // 생성 시 즉시 위치 설정
    }

    public void MoveRight(float distance)
    {
        targetPos += new Vector3(distance, 0, 0);
    }

    void Update()
    {
        // 부드러운 이동 (애니메이션이 재생되는 동안에도 자연스럽게 이동)
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * lerpSpeed);
    }
}