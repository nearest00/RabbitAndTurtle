using UnityEngine;

public class N221_CharacterMove : MonoBehaviour
{
    private Vector3 targetPos;
    public float moveSpeed = 20f;

    void Start()
    {
        targetPos = transform.position;
    }

    public void ChangeLane(float targetY)
    {
        targetPos = new Vector3(transform.position.x, targetY, 0);
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
    }
}