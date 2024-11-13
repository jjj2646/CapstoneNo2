using UnityEngine;
using System.Collections;

public class HeroKnight : MonoBehaviour
{
    [SerializeField] float m_speed = 4.0f;          // 이동 속도
    [SerializeField] float m_jumpForce = 10.0f;     // 점프 힘
    [SerializeField] float m_rollForce = 6.0f;      // 구르기 힘
    [SerializeField] bool m_noBlood = false;        // 피가 보이지 않도록 설정
    [SerializeField] GameObject m_slideDust;        // 슬라이드 시 생성되는 먼지 효과 오브젝트
    [SerializeField] int maxHealth = 100;           // 최대 체력

    private Animator m_animator;                    // 애니메이터
    private Rigidbody2D m_body2d;                   // 2D 리지드바디
    private Sensor_HeroKnight m_groundSensor;       // 바닥 감지 센서
    private Sensor_HeroKnight m_wallSensorR1;       // 오른쪽 벽 감지 센서1
    private Sensor_HeroKnight m_wallSensorR2;       // 오른쪽 벽 감지 센서2
    private Sensor_HeroKnight m_wallSensorL1;       // 왼쪽 벽 감지 센서1
    private Sensor_HeroKnight m_wallSensorL2;       // 왼쪽 벽 감지 센서2
    private bool m_isWallSliding = false;           // 벽 슬라이딩 상태
    private bool m_grounded = false;                // 바닥에 닿아있는 상태
    private bool m_rolling = false;                 // 구르기 중인지 여부
    private int m_facingDirection = 1;              // 캐릭터의 바라보는 방향 (1: 오른쪽, -1: 왼쪽)
    private int m_currentAttack = 0;                // 현재 공격 단계
    private float m_timeSinceAttack = 0.0f;         // 마지막 공격 이후 시간
    private float m_delayToIdle = 0.0f;             // 대기 상태로 돌아가기 전 딜레이
    private float m_rollDuration = 8.0f / 14.0f;    // 구르기 지속 시간
    private float m_rollCurrentTime;                // 구르기 지속 시간 측정용 타이머
    private int rollDirection;                      // 구르기 시작 시 방향 고정
    private SpriteRenderer spriteRenderer;          // 스프라이트 렌더러
    private int currentHealth;                      // 현재 체력
    private bool isDead = false;                    // 캐릭터가 사망했는지 여부

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();

        spriteRenderer = GetComponent<SpriteRenderer>();  // 스프라이트 렌더러 초기화
        currentHealth = maxHealth;                        // 체력을 최대 체력으로 초기화
    }

    void Update()
    {
        if (isDead) return;  // 사망 상태일 경우 모든 입력을 차단하고 업데이트를 종료
        
        m_timeSinceAttack += Time.deltaTime;  // 공격 시간 측정

        // 구르기 지속 시간 측정 및 종료
        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;
        if (m_rollCurrentTime > m_rollDuration)
        {
            m_rolling = false;
            m_rollCurrentTime = 0;
        }

        // 바닥에 닿았는지 확인
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // 떨어졌는지 확인
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        float inputX = 0.0f;  // 좌우 이동 입력

        // 구르기 중이 아닐 때 방향 전환 및 이동
        if (!m_rolling)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                inputX = 1.0f;
                spriteRenderer.flipX = false;
                m_facingDirection = 1;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                inputX = -1.0f;
                spriteRenderer.flipX = true;
                m_facingDirection = -1;
            }

            // 이동
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        }
        else
        {
            // 구르기 중에는 시작 시 방향으로만 이동
            m_body2d.velocity = new Vector2(rollDirection * m_rollForce, m_body2d.velocity.y);
        }

        // 구르기 시작 처리
        if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            rollDirection = m_facingDirection;  // 구르기 시작 시 방향 고정
            m_body2d.velocity = new Vector2(rollDirection * m_rollForce, m_body2d.velocity.y);
        }

        // 애니메이터에 수직 속도 전달
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // 벽 슬라이딩 상태 확인
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        // 사망 처리
        if (Input.GetKeyDown("e") && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }
        // 피격 처리
        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");

        // 공격 처리
        else if (Input.GetKeyDown("a") && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;
            if (m_currentAttack > 3)
                m_currentAttack = 1;
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            m_animator.SetTrigger("Attack" + m_currentAttack);
            m_timeSinceAttack = 0.0f;
        }

        // 방어 처리
        else if (Input.GetKeyDown("d") && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }
        else if (Input.GetKeyDown("d"))
            m_animator.SetBool("IdleBlock", false);

        // 점프 처리
        else if (Input.GetKeyDown(KeyCode.UpArrow) && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        // 달리기 상태 처리
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        // 대기 상태 처리
        else
        {
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 플랫폼 또는 적과 충돌 시 피해 처리
        if ((collision.gameObject.tag == "Platform") || (collision.gameObject.tag == "Enemy"))
        {
            Vector2 collisionPoint = collision.contacts[0].point; 
            OnDamaged(collision.transform.position);
        }
    }

    // 피격 처리 함수
    void OnDamaged(Vector2 targetPos)
    {
        if (isDead) return;  // 이미 사망한 경우 처리하지 않음

        gameObject.layer = 11;  // 무적 상태 레이어로 변경
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);  // 캐릭터 반투명화
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        m_body2d.AddForce(new Vector2(dirc, 1) * 6, ForceMode2D.Impulse);
        m_animator.SetTrigger("Hurt");
        Invoke("OffDamaged", 2);  // 무적 상태 해제 타이머

        currentHealth -= 50;  // 체력 감소
        if (currentHealth <= 0)
        {
            Die();  // 사망 함수 호출
        }
    }

    // 피격 종료 함수
    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);  // 캐릭터 원래 색으로 복귀
    }

    // 사망 처리 함수
    private void Die()
    {
        isDead = true;  // 사망 처리

        // 애니메이터 트리거 설정
        gameObject.layer = 11;
        spriteRenderer.color = new Color(1, 1, 1, 1);
        m_animator.SetTrigger("Death");

        // 리지드바디 속도 정지 (이동은 정지시키지만 중력은 유지)
        m_body2d.velocity = Vector2.zero;  // 리지드바디 속도 정지
        m_body2d.isKinematic = false;      // 중력과 물리 효과가 다시 적용되도록 설정

        // 중력이 작용하도록 하여 사망 후 떨어지게 함
    }

    /*    
    // 체력 회복 함수
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
    */

    void AE_SlideDust()
    {
        // 슬라이드 먼지 효과 생성 위치 설정
        Vector3 spawnPosition;
        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        // 먼지 효과 생성
        if (m_slideDust != null)
        {
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
}
