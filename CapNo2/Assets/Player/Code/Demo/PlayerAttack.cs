using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Collider2D AttackAreaCollider; // 공격 영역 Collider
    public float AttackDuration = 0.2f;   // 공격 활성화 시간

    void Start()
    {
        // 시작 시, 공격 콜라이더를 비활성화
        if (AttackAreaCollider != null)
        {
            AttackAreaCollider.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) // A키를 누르면 공격
        {
            StartCoroutine(ActivateAttackArea());
        }
    }

    IEnumerator ActivateAttackArea()
    {
        // 공격 콜라이더가 설정된 경우에만
        if (AttackAreaCollider != null)
        {
            AttackAreaCollider.enabled = true; // 공격 영역 활성화
            yield return new WaitForSeconds(AttackDuration); // 설정된 시간 동안 활성화
            AttackAreaCollider.enabled = false; // 공격 영역 비활성화
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // AttackAreaCollider가 설정되어 있고, 'Attack' 태그를 가진 경우에만 처리
        if (AttackAreaCollider != null && AttackAreaCollider.CompareTag("Attack"))
        {
            if (collision.CompareTag("Enemy"))
            {
                FlyEnemy enemy = collision.GetComponent<FlyEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(1); // 몬스터의 TakeDamage 호출
                    Debug.Log("몬스터가 공격받았습니다!");
                }
            }
        }
    }
}
