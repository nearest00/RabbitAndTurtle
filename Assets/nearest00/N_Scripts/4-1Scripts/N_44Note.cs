using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class N_44Note : MonoBehaviour
{
    public NoteInfo Data;
    public bool IsFailed = false;
    public bool IsHolding = false;
    public bool IsFinished = false;

    public RectTransform targetReceptor;
    public float noteSpeed;
    private float holdScoreTimer = 0f;
    private float lastCheckedBeat = 0f;
    private int longNoteTickCount = 0;

    private RectTransform rectTransform;
    private N_44GameManager gameManager;
    private N_44InputManager inputManager;
    private N_44JudgeEffectManager judgeEffectManager;
    private N_444SFXList sfx;

    // 롱노트 꼬리 이미지를 참조하기 위한 변수
    private RectTransform bodyRect;

	private AudioSource holdSFXSource; // 현재 재생중인 사운드 저장용
	public void Start()
	{
		sfx=Object.FindFirstObjectByType<N_444SFXList>();
	}
	public void Setup(NoteInfo info, float speed, RectTransform receptor, Sprite noteSprite)
    {
        // 1. 매니저 참조를 가장 먼저 수행 (NullReferenceException 방지)
        gameManager = Object.FindFirstObjectByType<N_44GameManager>();
        inputManager = Object.FindFirstObjectByType<N_44InputManager>();
        judgeEffectManager =Object.FindFirstObjectByType<N_44JudgeEffectManager>();

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

        if (IsHolding)
        {
            // 1. 머리 위치를 판정선에 고정
            rectTransform.anchoredPosition = targetReceptor.anchoredPosition;

            float endBeat = Data.hitTime + Data.duration;

            // 2. 꼬리 연출 (남은 길이에 비례해 줄어듦)
            if (bodyRect != null)
            {
                // 남은 박자 계산 = 종료 박자 - 현재 박자
                float remainingBeat = endBeat - currentBeat;

                // 꼬리 높이 업데이트 (최솟값 0 유지)
                float newHeight = Mathf.Max(0, remainingBeat * noteSpeed);
                bodyRect.sizeDelta = new Vector2(bodyRect.sizeDelta.x, newHeight);
            }

            float beatDelta = currentBeat - lastCheckedBeat;
            holdScoreTimer += beatDelta;
            lastCheckedBeat = currentBeat;

            if (holdScoreTimer >= 1.0f)
            {
                int ticks = Mathf.FloorToInt(holdScoreTimer);
                holdScoreTimer -= ticks;
                longNoteTickCount += ticks;
                N_44LifeSlider.Instance.AddValue(10);
                judgeEffectManager.ShowJudge("perfect");
                Debug.Log($"+10점 (롱노트 유지 중)");
            }

            if (currentBeat >= endBeat)
            {
                if (!IsFinished)
                {
                    IsFinished = true;
                }

                if (currentBeat > endBeat + 0.2f)
                {
					float diff = Data.duration - GetTickCount(); // 여기서도 diff 계산 적용
					if (diff <= 0.6f)
					{
						Debug.Log("롱노트 엔딩 미스(약간느림)");
						N_44LifeSlider.Instance.AddValue(-40);
					}
					else
					{
						Debug.Log("롱노트 엔딩 미스(느림)");
						N_44LifeSlider.Instance.AddValue(-50);
					}

					if (judgeEffectManager != null) judgeEffectManager.ShowJudge("miss");
					FailLongNote();
					return;
				}
            }
            return;
        }

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
	public int GetTickCount()
	{
		return longNoteTickCount;
	}
	private void UpdatePosition(float beatDiff)
    {
        if (targetReceptor != null)
        {
            float yOffset = beatDiff * noteSpeed;
            rectTransform.anchoredPosition = targetReceptor.anchoredPosition - new Vector2(0, yOffset);
        }
    }

    public void StartHolding() 
    { 
        IsHolding = true;
        lastCheckedBeat = Data.hitTime;
		if ( sfx.NoteSound != null && SoundManager.Instance != null)
		{
			holdSFXSource = SoundManager.Instance.PlayLoopingSFX(sfx.HoldSound);
		}
		Image headImage = GetComponent<Image>();
        if (headImage != null) headImage.enabled = false;
    }
	public void StopHoldSFX()
	{
		if (holdSFXSource != null && SoundManager.Instance != null)
		{
			SoundManager.Instance.StopLoopingSFX(holdSFXSource);
			holdSFXSource = null;
		}
        Debug.Log("사운드 정지");
	}
	public void FailLongNote()
	{
		IsFailed = true;
		StopHoldSFX(); // 사운드 정지
		Destroy(gameObject);
	}
	private void OnDestroy()
	{
		// 어떤 이유로든 이 오브젝트가 파괴될 때 사운드가 살아있다면 강제로 정지
		StopHoldSFX();
	}
}