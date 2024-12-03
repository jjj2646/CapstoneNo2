using UnityEngine;

public class Gem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"충돌한 오브젝트: {other.name}"); // 충돌한 오브젝트 이름 출력
        if (other.CompareTag("Player")) // 플레이어와 충돌했을 때만 실행
        {
            ScoreManager.Instance.AddScore(100); // 점수 추가
            Destroy(gameObject); // 다이아몬드 오브젝트 제거
        }
    }
}
