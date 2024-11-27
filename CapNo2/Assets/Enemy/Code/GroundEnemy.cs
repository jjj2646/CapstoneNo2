using System.Collections;
using UnityEngine;

public class GroundEnemy : MonoBehaviour
{
    Rigidbody2D rigid;                              // Rigidbody2D 컴포넌트 참조
    SpriteRenderer spriteRenderer;                  // SpriteRenderer 참조
    public int nextMove;                            // 이동 방향 (-1: 왼쪽, 1: 오른쪽)
    
    public float moveSpeed = 2.0f;                  // 이동 속도

    public Transform player;                        // 플레이어 Transform 참조
    public float detectionRange = 5.0f;             // 플레이어 감지 범위
    private bool isChasing = false;                 // 추격 상태
    public bool isDetectionEnabled = true;          // 감지 및 추격 기능의 활성화 여부

    private Animator m_animator;                    // 애니메이터
    public int health = 3;                          // 체력 (기본값 3)
    
    private bool isDead = false;                    // 죽었는지 여부

    private Color originalColor;                    // 원래 색상 저장용 변수
    private bool isFlashing = false;                // 깜빡임 상태 확인용 변수

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;       // 원래 색상 저장

        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

        nextMove = 1; // 초기 이동 방향: 오른쪽
        InvokeRepeating("ChangeDirection", 2, 2); // 2초 간격으로 방향 전환
    }

    void Update()
    {
        if (isDead) return; // 죽었으면 더 이상 업데이트하지 않음

        // 감지 및 추적 기능이 활성화된 경우만 작동
        if (isDetectionEnabled)
        {
            // 플레이어 감지 및 추격
            DetectPlayer();
        }

        // Move (X축 이동)
        rigid.velocity = new Vector2(nextMove * moveSpeed, 0);

        // Direction Adjustment (바라보는 방향 전환)
        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove > 0; // 왼쪽 이동이면 flipX 활성화
        }
    }

    void ChangeDirection()
    {
        if (!isChasing) // 추격 상태가 아닐 때만 좌우 이동 방향 변경
        {
            nextMove = -nextMove; // 이동 방향 반전
        }
    }

    void DetectPlayer()
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 플레이어가 감지 범위 내에 있으면 추격
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true; // 추격 상태로 전환

            // 플레이어 방향 계산
            if (player.position.x > transform.position.x)
                nextMove = 1; // 오른쪽으로 이동
            else if (player.position.x < transform.position.x)
                nextMove = -1; // 왼쪽으로 이동
        }
        else
        {
            isChasing = false; // 추격 상태 해제
        }
    }

    // 감지 및 추격 기능을 토글하는 메서드
    public void ToggleDetection()
    {
        isDetectionEnabled = !isDetectionEnabled; // 감지 상태 토글
        Debug.Log($"Detection and Chasing Enabled: {isDetectionEnabled}");
    }

    // 감지 범위 표시 (씬 뷰에서만 표시됨)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // 감지 범위의 색상
        Gizmos.DrawWireSphere(transform.position, detectionRange); // 감지 범위 원형 표시
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // 이미 죽었으면 체력 감소하지 않음

        health -= damage; // 체력 감소
        Debug.Log("몬스터가 공격받았습니다! 남은 체력: " + health);

        // 빨간색 깜빡임 효과 시작 (죽지 않았을 때만)
        if (!isFlashing && health > 0)
        {
            StartCoroutine(FlashRed());
        }

        if (health <= 0)
        {
            Die(); // 체력이 0 이하이면 죽음 처리
        }
    }

    // 빨간색으로 깜빡이는 코루틴
    IEnumerator FlashRed()
    {
        isFlashing = true;  // 깜빡임 시작

        float flashDuration = 0.5f; // 깜빡이는 시간
        float elapsedTime = 0f;

        while (elapsedTime < flashDuration)
        {
            // 빨간색으로 변경
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f); // 0.05초마다 깜빡이기

            // 원래 색상으로 돌아가기
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);

            elapsedTime += 0.2f;
        }

        // 깜빡임이 끝난 후 색상을 원래대로 되돌림
        spriteRenderer.color = originalColor;
        isFlashing = false;  // 깜빡임 끝
    }

    void Die()
    {
        isDead = true; // 죽은 상태로 설정

        // 죽을 때는 원래 색상으로 깜빡이기
        StartCoroutine(FadeOutAndDestroy());

        // 더 이상 추격하지 않도록 속도 0으로 설정
        rigid.velocity = Vector2.zero;
    }

    // 깜빡이며 사라지는 코루틴 (죽을 때만 사용)
    IEnumerator FadeOutAndDestroy()
    {
        float fadeDuration = 1.0f; // 사라지는 시간
        float elapsedTime = 0f;

        Color originalColor = spriteRenderer.color;  // 원래 색상 저장

        // 원래 색상으로 깜빡이도록 설정
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.PingPong(elapsedTime * 5f, 1f); // 깜빡임 효과 (ping pong 방식)
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);  // 원래 색상으로 설정
            yield return null;
        }

        // 사라진 후 0.5초 뒤에 객체 삭제
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
