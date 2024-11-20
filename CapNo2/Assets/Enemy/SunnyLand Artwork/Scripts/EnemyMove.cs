using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation; // Z축 회전 고정
        nextMove = 1; // 초기값 설정
        Invoke("Think", 3); //3초에 한번씩 방향 바뀜(더 시간 줄일 수 있음)
    }

    void FixedUpdate()
    {
        // Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y); // 이동 방향 반영

        // Direction Adjustment (바라보는 방향 전환)
        if (nextMove != 0) // 정지 상태가 아닌 경우에만 전환(정지상태에는 바라보는 방향 정지)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (nextMove > 0 ? -1 : 1); // 방향에 따라 X축 반전
            transform.localScale = scale;
        }

        // Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y - 0.5f);
        Debug.DrawRay(frontVec, Vector2.down * 1, Color.green); // Debug용 Ray 확인
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 0.3f, LayerMask.GetMask("Default"));//layer을 맞춰야함 중요*

        if (rayHit.collider == null)
        {
            nextMove *= -1; // 방향 전환
            CancelInvoke();
            Invoke("Think", 3);
        }
    }

    void Think()
    {
        nextMove = Random.Range(-1, 2); // -1, 0, 1 중 하나 반환(정지 없앨 수 있음 0?:-1:1)
        Invoke("Think", 3); // 3초 후 다시 호출
    }
}
