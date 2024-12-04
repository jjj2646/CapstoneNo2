using UnityEngine;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{
    [SerializeField] private Button audioButton; // 버튼 컴포넌트
    [SerializeField] private Text buttonText;    // 버튼의 텍스트 (상태 표시)

    private bool isSoundOn = true; // 초기 사운드 상태

    private void Start()
    {
        if (audioButton == null || buttonText == null)
        {
            Debug.LogError("AudioToggle: audioButton과 buttonText를 Unity 에디터에서 연결하세요.");
            return;
        }

        // 초기 버튼 상태 설정
        UpdateButtonUI();

        // 버튼 클릭 이벤트 연결
        audioButton.onClick.AddListener(ToggleSound);
    }

    private void ToggleSound()
    {
        // 사운드 상태 변경
        isSoundOn = !isSoundOn;

        // 오디오 전환 로직 (여기서는 Mute를 예로 들었지만, 필요에 따라 수정 가능)
        AudioListener.pause = !isSoundOn;

        // 버튼 UI 업데이트
        UpdateButtonUI();
    }

    private void UpdateButtonUI()
    {
        // 버튼 텍스트를 상태에 따라 변경
        if (buttonText != null)
        {
            buttonText.text = isSoundOn ? "Sound On" : "Sound Off";
        }
    }
}
