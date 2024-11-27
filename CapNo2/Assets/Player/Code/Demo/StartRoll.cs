using UnityEngine;

public class StartRoll : MonoBehaviour
{
    private Animator m_animator;
    private Rigidbody2D m_rigidbody;

    public float moveSpeed = 5.0f; // 좌우 이동 속도
    public float rollSpeedMultiplier = 1.0f; // 기본 배속 값 (1.0f는 정상 속도)

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_animator.SetTrigger("Roll");

        // 초기 배속 설정
        m_animator.speed = rollSpeedMultiplier;
    }

    void Update()
    {
        HandleRollAnimation();
        HandleMovement();

        // 테스트용으로 배속 변경 (Q 키로 느리게, E 키로 빠르게)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AdjustAnimationSpeed(rollSpeedMultiplier - 0.5f); // 배속 감소
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            AdjustAnimationSpeed(rollSpeedMultiplier + 0.5f); // 배속 증가
        }
    }

    void HandleRollAnimation()
    {
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Roll") && stateInfo.normalizedTime >= 0.99f)
        {
            m_animator.ResetTrigger("Roll");
            m_animator.SetTrigger("Roll");
        }
        else if (!stateInfo.IsName("Roll"))
        {
            m_animator.SetTrigger("Roll");
        }
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        Vector2 velocity = m_rigidbody.velocity;
        velocity.x = moveInput * moveSpeed;
        m_rigidbody.velocity = velocity;

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // 애니메이션 배속 조정 함수
    void AdjustAnimationSpeed(float newSpeed)
    {
        rollSpeedMultiplier = Mathf.Clamp(newSpeed, 0.1f, 5.0f); // 최소/최대 속도 제한
        m_animator.speed = rollSpeedMultiplier;

        Debug.Log($"애니메이션 배속이 {rollSpeedMultiplier}로 변경되었습니다.");
    }
}
