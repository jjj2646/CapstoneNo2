using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    
    // 왔다갔다 할 범위 설정 (최소, 최대 X 좌표)
    public float minX = -3.0f;
    public float maxX = 3.0f;

    void Start()
{
    // Monsters 레이어끼리 충돌을 무시
    Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("monster"), LayerMask.NameToLayer("monster"));
}

    
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        // 처음에 움직이도록 기본 nextMove 값 설정
        nextMove = 1;  // 기본적으로 오른쪽으로 이동 시작

        // 2초 후에 Think 호출
        Invoke("Think", 2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 이동
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // 캐릭터가 minX보다 왼쪽으로 넘어가면 오른쪽으로 이동하게, maxX보다 오른쪽으로 넘어가면 왼쪽으로 이동하게 설정
        if (transform.position.x < minX || transform.position.x > maxX)
        {
            nextMove *= -1;  // 방향 반대로 변경
        }
    }

    void Think()
    {
        // 랜덤으로 방향 설정 (-1, 1로만 설정, 0은 제외)
        nextMove = Random.Range(-1, 2);
        if (nextMove == 0)
            nextMove = 1;  // 멈추지 않도록 0이 나올 경우 1로 대체

        Invoke("Think", 2);  // 다시 2초 후 호출
    }
}
