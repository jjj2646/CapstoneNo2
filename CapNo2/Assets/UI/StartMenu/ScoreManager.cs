using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    public TMP_Text scoreText; // TextMeshPro 텍스트 필드
    private int score = 0;

    private void Awake()
    {
        // Singleton 패턴
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 게임 시작 시 점수 UI 초기화
        score = PlayerPrefs.GetInt("Score", 0);  // PlayerPrefs에서 점수 불러오기
        UpdateScoreUI(); // 점수 UI 업데이트
    }

    public void AddScore(int value)
    {
        score += value; // 점수 추가
        PlayerPrefs.SetInt("Score", score); // 점수 저장
        UpdateScoreUI(); // UI 업데이트
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString(); // 점수 텍스트 업데이트
        }
        else
        {
            Debug.LogError("Score Text가 연결되지 않았습니다!");
        }
    }
}
