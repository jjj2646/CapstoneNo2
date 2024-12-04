using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HeroKnight : MonoBehaviour
{
    public AudioClip gemSound; // 젬 사운드 클립

    [SerializeField] float m_speed = 4.0f;          // 이동 속도
    [SerializeField] float m_jumpForce = 10.0f;     // 점프 힘
    [SerializeField] float m_rollForce = 6.0f;      // 구르기 힘
    [SerializeField] GameObject m_slideDust;        // 슬라이드 시 생성되는 먼지 효과 오브젝트
    [SerializeField] int maxHealth = 100;           // 최대 체력

    // 새로 추가된 변수들
    [SerializeField] AudioClip attackSound;         // 공격 사운드
    private AudioSource audioSource;                 // AudioSource 컴포넌트

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
    private bool m_canDoubleJump = false;           // 더블 점프 가능 여부

    public Image fadeImage;                         // Canvas의 Image를 연결
    public Image[] hearts;      // 하트 이미지 배열
    public Sprite fullHeart;    // 채워진 하트 이미지
    public Sprite emptyHeart;   // 빈 하트 이미지

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

        UpdateHealthUI();

        // AudioSource 컴포넌트 추가
        audioSource = GetComponent<AudioSource>();
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

        // 바닥 상태 업데이트
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_canDoubleJump = true; // 바닥에 닿으면 더블 점프 가능
            m_animator.SetBool("Grounded", true);
        }
        else if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", false);
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

        // 점프 입력 처리
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (m_grounded)
            {
                Jump(); // 첫 점프
            }
            else if (m_canDoubleJump)
            {
                m_canDoubleJump = false; // 더블 점프 사용
                Jump();
            }
        }

        // 애니메이터에 수직 속도 전달
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // 벽 슬라이딩 상태 확인
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        // 공격 처리
        if (Input.GetKeyDown("a") && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;
            if (m_currentAttack > 3)
                m_currentAttack = 1;
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            m_animator.SetTrigger("Attack" + m_currentAttack);
            m_timeSinceAttack = 0.0f;

            // 공격 시 사운드 재생
            if (attackSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(attackSound);
            }
        }

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
        if (Mathf.Abs(inputX) > Mathf.Epsilon)
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
    void Jump()
    {
        m_animator.SetTrigger("Jump");
        m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
        if (!m_grounded)
            m_groundSensor.Disable(0.2f);
    }

void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Gem")
    {
        // 다이아 사운드 재생
        audioSource.PlayOneShot(gemSound);

        // 다이아 오브젝트 삭제 (혹은 비활성화)
        Destroy(collision.gameObject);
    }
    // 플랫폼 또는 적과 충돌 시 피해 처리
    else if ((collision.gameObject.tag == "Platform") || (collision.gameObject.tag == "Enemy") || (collision.gameObject.tag == "FlyEnemy"))
    {
        Vector2 collisionPoint = collision.contacts[0].point; 
        OnDamaged(collision.transform.position);
    }
    else if (collision.gameObject.tag == "Goal")
    {
        Debug.Log("Goal Reached! Loading Next Scene...");
        SceneManager.LoadScene("MainMap"); // "NextStageScene" 이름의 씬으로 이동
    }
    // 떨어지면 리셋
    else if (collision.gameObject.tag == "Reset")
    {
        Die();
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

        currentHealth -= 1;  // 체력 감소

        if (currentHealth <= 0)
        {
            currentHealth = 0;  // 체력이 0 이하로 내려가지 않도록 제한
            UpdateHealthUI();    // 체력이 0이면 UI 갱신
            Die();               // 체력이 0이면 Die() 함수 호출
        }
        else
        {
            UpdateHealthUI();    // 체력이 0이 아니면 UI 갱신
        }

    }

    void UpdateHealthUI()
    {
        // 현재 체력에 맞게 하트 이미지 갱신
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;  // 체력이 있으면 채워진 하트
            }
            else
            {
                hearts[i].sprite = emptyHeart; // 체력이 없으면 빈 하트
            }
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

        StartCoroutine(FadeToBlackAndLoadScene("GameOverScene", 5f));

    }
     private IEnumerator FadeToBlackAndLoadScene(string sceneName, float delay)
    {
        // 사망 후 딜레이 (애니메이션 시간을 고려)
        yield return new WaitForSeconds(delay);

        // 페이드 인 (화면 어둡게)
        float fadeDuration = 2f; // 페이드 인 시간
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            Color color = fadeImage.color;
            color.a = Mathf.Lerp(0, 1, normalizedTime); // 알파 값 증가
            fadeImage.color = color;
            yield return null;
        }
        SceneManager.LoadScene("GameOverScene"); //어떤 씬 이름으로 이동할 건지
        Debug.Log("BlackScene Go");
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

