using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] private AudioClip collectSound; // 다이아몬드를 먹을 때 재생할 사운드 클립
    [SerializeField] private float soundVolume = 1.0f; // 사운드 볼륨 (기본값 1.0)

    private AudioSource audioSource; // AudioSource 컴포넌트 참조

    private void Awake()
    {
        // Gem 오브젝트에 AudioSource 컴포넌트 추가
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = soundVolume; // 사운드 볼륨 설정
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"충돌한 오브젝트: {other.name}"); // 충돌한 오브젝트 이름 출력
        if (other.CompareTag("Player")) // 플레이어와 충돌했을 때만 실행
        {
            // 점수 추가
            ScoreManager.Instance.AddScore(100);

            // 사운드 효과 재생
            if (collectSound != null)
            {
                audioSource.PlayOneShot(collectSound); // 사운드 재생
            }

            // 다이아몬드 오브젝트 제거
            Destroy(gameObject);
        }
    }
}
