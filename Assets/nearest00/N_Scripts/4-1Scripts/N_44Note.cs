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

    // 롱노트 꼬리 이미지를 참조하기 위한 변수
    private RectTransform bodyRect;

    public void Setup(NoteInfo info, float speed, RectTransform receptor, Sprite noteSprite)
    {
        // 1. 매니저 참조를 가장 먼저 수행 (NullReferenceException 방지)
        gameManager = Object.FindFirstObjectByType<N_44GameManager>();
        inputManager = Object.FindFirstObjectByType<N_44InputManager>();

        // 2. 기본 데이터 설정
        Data = info;
        noteSpeed = speed;
        targetReceptor = receptor;
        rectTransform = GetComponent<RectTransform>();

        // 3. 부모 설정 (위치 계산 전에 부모가 설정되어야 함)
        if (targetReceptor != null)
        {
            transform.SetParent(targetReceptor.parent, false);
        }

        // 4. 초기 위치 설정 (가운데 깜빡임 방지)
        if (gameManager != null)
        {
            float beatDiff = Data.hitTime - gameManager.GetBeatTime();
            UpdatePosition(beatDiff);
        }

        // 5. 이미지 설정
        Image myImage = GetComponent<Image>();
        if (myImage != null && noteSprite != null)
        {
            myImage.sprite = noteSprite;
        }
    }

    // 롱노트 설정
    public void SetupLongNote(NoteInfo info, float speed, RectTransform receptor, Sprite noteSprite, Sprite bodySprite, float durationInBeats)
    {
        // 1. 기본 머리 셋업 실행
        Setup(info, speed, receptor, noteSprite);

        // 2. 몸통(꼬리) 생성 및 계층 구조 설정
        GameObject bodyObj = new GameObject("LongNoteBody", typeof(Image));
        bodyObj.transform.SetParent(this.transform, false);
        bodyObj.transform.SetAsFirstSibling(); // 머리 뒤로 보냄

        Image bodyImg = bodyObj.GetComponent<Image>();
        bodyImg.sprite = bodySprite;
        bodyImg.type = Image.Type.Sliced; // 9-Slice 적용

        bodyRect = bodyObj.GetComponent<RectTransform>();

        // 피벗 및 앵커 설정 (상단 중앙)
        bodyRect.pivot = new Vector2(0.5f, 1f);
        bodyRect.anchorMin = new Vector2(0.5f, 1f);
        bodyRect.anchorMax = new Vector2(0.5f, 1f);

        // 3. 길이 계산 및 사이즈 적용
        float finalHeight = durationInBeats * speed;
        bodyRect.sizeDelta = new Vector2(100f, finalHeight); // 가로폭 100 고정
        bodyRect.anchoredPosition = Vector2.zero;
    }

    void Update()
    {
        if (gameManager == null || inputManager == null || IsFailed) return;

        float currentBeat = gameManager.GetBeatTime();
        float beatDiff = Data.hitTime - currentBeat;

        // 롱노트 홀딩 중일 때 처리
        if (IsHolding)
        {
            N_44LifeSlider.Instance.AddValue(Time.deltaTime * 5f);
            // 1. 머리 위치를 판정선에 고정
            rectTransform.anchoredPosition = targetReceptor.anchoredPosition;

            // 2. 꼬리 연출: 현재 박자가 노트 시작 박자보다 커질수록 꼬리를 줄임
            if (bodyRect != null)
            {
                // 남은 박자 계산 = (시작박자 + 총길이) - 현재박자
                float remainingBeat = (Data.hitTime + Data.duration) - currentBeat;

                // 꼬리 높이 재계산
                float newHeight = remainingBeat * noteSpeed;

                if (newHeight <= 0)
                {
                    inputManager.RemoveNote(this, (int)Data.direction);
                    return;
                }

                // 세로 길이를 실시간으로 업데이트 (가로 폭은 유지)
                bodyRect.sizeDelta = new Vector2(bodyRect.sizeDelta.x, newHeight);
            }
            return;
        }

        // 미스 판정
        if (beatDiff < -0.5f && !IsFailed)
        {
            FailLongNote();
        }

        // 화면 밖으로 완전히 나가면 삭제
        if (beatDiff < -1.5f)
        {
            inputManager.RemoveNote(this, (int)Data.direction);
            return;
        }

        UpdatePosition(beatDiff);
    }

    private void UpdatePosition(float beatDiff)
    {
        if (targetReceptor != null)
        {
            float yOffset = beatDiff * noteSpeed;
            rectTransform.anchoredPosition = targetReceptor.anchoredPosition - new Vector2(0, yOffset);
        }
    }

    public void StartHolding() { IsHolding = true; }

    public void FailLongNote()
    {
        IsFailed = true;
        // 실패 시 즉시 파괴 (필요에 따라 연출 코드로 대체 가능)
        Destroy(gameObject);
    }
}