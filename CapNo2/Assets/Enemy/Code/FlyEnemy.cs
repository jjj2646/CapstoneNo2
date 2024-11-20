using System.Collections;
using UnityEngine;

public class FlyEnemy : MonoBehaviour
{
    Rigidbody2D rigid;                              // Rigidbody2D 컴포넌트 참조
    SpriteRenderer spriteRenderer;                  // SpriteRenderer 참조
    public int nextMove;                            // 이동 방향 (-1: 왼쪽, 1: 오른쪽)
    public float fixedYPosition = 3.0f;             // 고정된 Y축 위치
    public float moveSpeed = 2.0f;                  // 이동 속도

    public Transform player;                        // 플레이어 Transform 참조
    public float detectionRange = 5.0f;             // 플레이어 감지 범위
    private bool isChasing = false;                 // 추격 상태
    public bool isDetectionEnabled = true;          // 감지 및 추격 기능의 활성화 여부

    private Animator m_animator;                    // 애니메이터
    public int health = 3;                          // 체력 (기본값 3)
    
    private bool isDead = false;                    // 죽었는지 여부

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

        nextMove = 1; // 초기 이동 방향: 오른쪽
        InvokeRepeating("ChangeDirection", 2, 2); // 2초 간격으로 방향 전환
    }

    void Update()
    {
        if (isDead) return; // 죽었으면 더 이상 업데이트하지 않음

        // Y축 고정
        transform.position = new Vector3(transform.position.x, fixedYPosition, transform.position.z);

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

        // Hurt 애니메이션 트리거
        m_animator.SetTrigger("hurt");

        if (health <= 0)
        {
            Die(); // 체력이 0 이하이면 죽음 처리
        }
    }

    void Die()
    {
        isDead = true; // 죽은 상태로 설정

        // 깜빡이는 효과 시작
        StartCoroutine(FadeOutAndDestroy());

        // 더 이상 추격하지 않도록 속도 0으로 설정
        rigid.velocity = Vector2.zero;
    }

    // 깜빡이며 사라지는 코루틴
    IEnumerator FadeOutAndDestroy()
    {
        float fadeDuration = 1.0f; // 사라지는 시간
        float elapsedTime = 0f;

        // 알파 값이 1로 시작하여 0으로 감소
        Color originalColor = spriteRenderer.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.PingPong(elapsedTime * 5f, 1f); // 깜빡임 효과 (ping pong 방식)
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // 사라진 후 0.5초 뒤에 객체 삭제
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
