using UnityEngine;
using UnityEngine.UI;

public class SoundSettingPanelManager : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        if (SoundManager.Instance == null) return;

        // 중요: 리스너를 먼저 제거해서 초기값 대입 시 이벤트가 발생하는 걸 방지합니다.
        bgmSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();

        // 현재 사운드매니저에 저장된 볼륨 값으로 슬라이더 위치 세팅
        bgmSlider.value = SoundManager.Instance.BgmVolume;
        sfxSlider.value = SoundManager.Instance.SfxVolume;

        // 그 다음 리스너를 연결합니다.
        bgmSlider.onValueChanged.AddListener(val => {
            SoundManager.Instance.SetBgmVolume(val);
        });

        sfxSlider.onValueChanged.AddListener(val => {
            SoundManager.Instance.SetSfxVolume(val);
        });
    }
}