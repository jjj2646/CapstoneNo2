using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Collider2D AttackAreaColliderLeft;  // 왼쪽 공격 콜라이더
    public Collider2D AttackAreaColliderRight; // 오른쪽 공격 콜라이더
    public float AttackDuration = 0.2f;        // 공격 활성화 시간

    void Start()
    {
        // 시작 시, 두 콜라이더 비활성화
        if (AttackAreaColliderLeft != null)
        {
            AttackAreaColliderLeft.enabled = false;
        }
        if (AttackAreaColliderRight != null)
        {
            AttackAreaColliderRight.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) // A키를 누르면 공격
        {
            FlipAttackArea(); // 콜라이더 방향 전환
            StartCoroutine(ActivateAttackArea());
        }
    }

    IEnumerator ActivateAttackArea()
    {
        // 공격 콜라이더가 설정된 경우에만
        yield return new WaitForSeconds(AttackDuration); // 설정된 시간 동안 활성화
        DisableAllAttackAreas();  // 일정 시간 후 모든 공격 콜라이더 비활성화
    }

    void DisableAllAttackAreas()
    {
        if (AttackAreaColliderLeft != null)
        {
            AttackAreaColliderLeft.enabled = false;
        }
        if (AttackAreaColliderRight != null)
        {
            AttackAreaColliderRight.enabled = false;
        }
    }

    // 공격 콜라이더를 방향에 맞게 반전
    void FlipAttackArea()
    {
        SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();
        if (playerSprite != null)
        {
            // 플레이어가 왼쪽을 보고 있다면 왼쪽 콜라이더만 활성화
            if (playerSprite.flipX)
            {
                AttackAreaColliderLeft.enabled = true;
                AttackAreaColliderRight.enabled = false;
            }
            else
            {
                AttackAreaColliderLeft.enabled = false;
                AttackAreaColliderRight.enabled = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 공격 콜라이더가 활성화된 경우에만 충돌 처리
        if (AttackAreaColliderLeft.enabled || AttackAreaColliderRight.enabled)
        {
            // 왼쪽 공격 콜라이더와 충돌한 경우
            if (AttackAreaColliderLeft.enabled && collision.CompareTag("FlyEnemy"))
            {
                FlyEnemy enemy = collision.GetComponent<FlyEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(1); // 몬스터의 TakeDamage 호출
                    Debug.Log("왼쪽 공격으로 몬스터가 공격받았습니다!");
                }
            }
            else if (AttackAreaColliderLeft.enabled && collision.CompareTag("GroundEnemy"))
            {
                GroundEnemy enemy = collision.GetComponent<GroundEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(1); // 몬스터의 TakeDamage 호출
                    Debug.Log("왼쪽 공격으로 몬스터가 공격받았습니다!");
                }
            }

            // 오른쪽 공격 콜라이더와 충돌한 경우
            if (AttackAreaColliderRight.enabled && collision.CompareTag("FlyEnemy"))
            {
                FlyEnemy enemy = collision.GetComponent<FlyEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(1); // 몬스터의 TakeDamage 호출
                    Debug.Log("오른쪽 공격으로 몬스터가 공격받았습니다!");
                }
            }

            else if (AttackAreaColliderRight.enabled && collision.CompareTag("GroundEnemy"))
            {
                GroundEnemy enemy = collision.GetComponent<GroundEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(1); // 몬스터의 TakeDamage 호출
                    Debug.Log("오른쪽 공격으로 몬스터가 공격받았습니다!");
                }
            }
        }

        // 플레이어 본체와 충돌을 무시
        if (collision.CompareTag("Player"))
        {
            return; // 본체와 충돌할 때 공격을 처리하지 않음
        }
    }
}
