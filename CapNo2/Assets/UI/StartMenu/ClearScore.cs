using UnityEngine;
using TMPro;

public class ClearScreen : MonoBehaviour
{
    public TMP_Text clearScoreText;  // 클리어 화면에서 점수를 표시할 텍스트 UI 필드
    public TMP_Text clearRankText;   // 클리어 화면에서 랭크를 표시할 텍스트 UI 필드

    void Start()
    {
        // PlayerPrefs에서 점수 불러오기
        int score = PlayerPrefs.GetInt("Score", 0); 

        // 점수를 텍스트로 업데이트
        clearScoreText.text = "Score : " + score.ToString();

        // 점수에 따라 랭크 계산
        string rank = GetRank(score);

        // 랭크 텍스트 업데이트
        clearRankText.text = "Rank : " + rank;
    }

    // 점수에 따라 랭크를 계산하는 함수
    string GetRank(int score)
    {
        if (score == 1000)
        {
            return "S!!!";
        }
        else if (score >= 800 && score < 1000)
        {
            return "A!!";
        }
        else if (score >= 500 && score < 800)
        {
            return "B!";
        }
        else
        {
            return "C";
        }
       
    }
}
