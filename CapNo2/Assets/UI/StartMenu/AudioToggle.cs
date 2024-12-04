using UnityEngine;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{
    public Button audioButton;           // UI 버튼을 연결할 변수
    public Text buttonText;              // 버튼의 텍스트를 변경하기 위한 변수
    public AudioSource audioSource;      // 사운드 재생용 AudioSource
    public AudioClip buttonClickSound;   // 버튼 클릭 시 재생할 사운드 클립
    private bool isAudioOn = true;       // 현재 오디오 상태를 저장하는 변수

    void Start()
    {
        // Null 체크 추가
        if (audioButton == null || buttonText == null)
        {
            Debug.LogError("AudioToggle의 필드가 연결되지 않았습니다. Unity 에디터에서 audioButton과 buttonText를 연결하세요.");
            return;
        }

        // 시작 시 버튼 텍스트를 현재 오디오 상태에 맞게 설정
        UpdateButtonText();

        // 버튼에 이벤트 리스너 추가
        audioButton.onClick.AddListener(() =>
        {
            ToggleAudio();
            PlayButtonClickSound();
        });
    }

    void ToggleAudio()
    {
        // 현재 오디오 상태를 반전
        isAudioOn = !isAudioOn;

        // 오디오 켜기/끄기
        AudioListener.volume = isAudioOn ? 1 : 0;

        // 버튼 텍스트 업데이트
        UpdateButtonText();
    }

    void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = isAudioOn ? "Sound: On" : "Sound: Off";
        }
        else
        {
            Debug.LogError("buttonText가 연결되지 않았습니다.");
        }
    }

    void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound); // 버튼 클릭 사운드 재생
        }
    }
}
