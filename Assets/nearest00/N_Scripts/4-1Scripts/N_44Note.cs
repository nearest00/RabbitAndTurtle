using UnityEngine;
using UnityEngine.UI;

public class N_44Note : MonoBehaviour
{
    public NoteInfo Data;
    public bool IsFailed = false;
    public bool IsHolding = false;

    public RectTransform targetReceptor;
    public float noteSpeed;
    private RectTransform rectTransform;
    private N_44GameManager gameManager;
    private N_44InputManager inputManager;

    public void Setup(NoteInfo info, float speed, RectTransform receptor, Sprite noteSprite)
    {
        Data = info;
        noteSpeed = speed;
        targetReceptor = receptor;
        rectTransform = GetComponent<RectTransform>();

        // 1. 이미지 변경 로직 추가
        UnityEngine.UI.Image myImage = GetComponent<UnityEngine.UI.Image>();
        if (myImage != null && noteSprite != null)
        {
            myImage.sprite = noteSprite;
        }

        // 2. 매니저 참조 및 부모 설정
        gameManager = Object.FindFirstObjectByType<N_44GameManager>();
        inputManager = Object.FindFirstObjectByType<N_44InputManager>();
        transform.SetParent(targetReceptor.parent, false);
    }

    void Update()
    {
        if (gameManager == null || inputManager == null || IsFailed) return;

        // 현재 '박자' 차이를 계산합니다.
        float currentBeat = gameManager.GetBeatTime();
        float beatDiff = Data.hitTime - currentBeat; // 초 단위 diff 대신 beatDiff 사용

        // 1. 자동 미스 처리 (0.5박자 이상 지나쳤을 때)
        if (!IsHolding && beatDiff < -0.5f && !IsFailed)
        {
            FailLongNote();
        }

        // 2. 오브젝트 삭제 (2박자 이상 지나가면 삭제)
        if (beatDiff < -1.0f)
        {
            inputManager.RemoveNote(this, (int)Data.direction);
            return;
        }

        // 3. 위치 업데이트 (계산한 beatDiff를 전달)
        UpdatePosition(beatDiff);
    }

    // 매개변수 이름을 beatDiff로 통일합니다.
    private void UpdatePosition(float beatDiff)
    {
        if (targetReceptor != null)
        {
            // beatDiff가 1.0(1박자 남음)일 때, 
            // yOffset은 정확히 pixelsPerBeat(예: 600px)가 됩니다.
            float yOffset = beatDiff * noteSpeed;

            rectTransform.anchoredPosition = targetReceptor.anchoredPosition - new Vector2(0, yOffset);
        }
    }
    public void StartHolding() { IsHolding = true; }
    public void FailLongNote()
    {
        inputManager.RemoveNote(this, (int)Data.direction);
        IsFailed = true;
    }
}