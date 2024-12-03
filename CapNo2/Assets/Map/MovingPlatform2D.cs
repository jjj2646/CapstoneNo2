using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class MovingPlatform2D : MonoBehaviour
{
    public Vector3 startPoint;
    public Vector3 endPoint;
    public float speed = 2.0f;

    private float journeyLength;
    private float startTime;

    void Start()
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPoint, endPoint);
    }

    void Update()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distCovered / journeyLength;

        transform.position = Vector3.Lerp(startPoint, endPoint, fractionOfJourney);

        if (fractionOfJourney >= 1.0f)
        {
            // 플랫폼이 끝에 도달하면 시작점으로 돌아가기
            Vector3 temp = startPoint;
            startPoint = endPoint;
            endPoint = temp;
            startTime = Time.time; // 시간을 초기화
        }
    }
}

