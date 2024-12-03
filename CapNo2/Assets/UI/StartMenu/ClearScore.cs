using UnityEngine;
using TMPro;

public class ClearScreen : MonoBehaviour
{
    public TMP_Text clearScoreText;  // 클리어 화면에서 점수를 표시할 텍스트 UI 필드

    void Start()
    {
        int score = PlayerPrefs.GetInt("Score", 0);  // PlayerPrefs에서 점수 불러오기
        clearScoreText.text = "Final Score: " + score.ToString();  // 점수 표시
    }
}
