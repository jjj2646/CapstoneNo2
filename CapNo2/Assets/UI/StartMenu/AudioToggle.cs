using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{
    public Button audioButton;           // UI 버튼을 연결할 변수
    public Text buttonText;              // 버튼의 텍스트를 변경하기 위한 변수
    private bool isAudioOn = true;       // 현재 오디오 상태를 저장하는 변수

    void Start()
    {
        // 시작 시 버튼 텍스트를 현재 오디오 상태에 맞게 설정
        UpdateButtonText();
        
        // 버튼에 이벤트 리스너 추가
        audioButton.onClick.AddListener(ToggleAudio);
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
        buttonText.text = isAudioOn ? "Sound: On" : "Sound: Off";
    }
}
