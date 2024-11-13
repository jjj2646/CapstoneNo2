using UnityEngine;
using System.Collections;

public class Sensor_HeroKnight : MonoBehaviour {

    private int m_ColCount = 0;

    private float m_DisableTimer;

    private void OnEnable()
    {
        m_ColCount = 0;
    }

    public bool State()
    {
        // 벽 충돌을 무시하고 바닥에만 접촉한 상태를 확인하도록 수정
        if (m_DisableTimer > 0)
            return false;

        // 바닥과의 충돌만 체크
        return m_ColCount > 0; // 바닥과 접촉 시 true 반환
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        m_ColCount++;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        m_ColCount--;
    }

    void Update()
    {
        m_DisableTimer -= Time.deltaTime;
    }

    public void Disable(float duration)
    {
        m_DisableTimer = duration;
    }
}
