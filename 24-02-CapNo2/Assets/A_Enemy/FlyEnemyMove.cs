using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMonster : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;

    // 몬스터가 움직일 좌우 범위
    public float minX = -1.0f;
    public float maxX = 1.0f;
    
    // 몬스터가 나는 고도 (Y축 고정값)
    public float fixedY = 3.0f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        // Rigidbody2D에 중력 영향을 받지 않도록 설정
        rigid.gravityScale = 0;

        // 처음 이동 방향 설정 (오른쪽으로 시작)
        nextMove = 1;

        // 2초 후에 Think 호출
        Invoke("Think", 2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 좌우 이동, Y축은 고정된 높이를 유지
        rigid.velocity = new Vector2(nextMove, 0);

        // X축 범위 안에서만 좌우로 움직임
        if (transform.position.x < minX || transform.position.x > maxX)
        {
            nextMove *= -1;  // 방향 반대로 변경
        }

        // Y축 위치를 고정된 높이로 유지
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
    }

    void Think()
    {
        // 랜덤으로 방향 설정 (-1, 1로만 설정, 0은 제외)
        nextMove = Random.Range(-1, 2);
        if (nextMove == 0)
            nextMove = 1;  // 0이 나올 경우 멈추지 않게 1로 대체

        float nextThinkTime=Random.Range(1f, 4f);
        Invoke("Think", nextThinkTime);  // 2초 후 다시 호출
    }
}
