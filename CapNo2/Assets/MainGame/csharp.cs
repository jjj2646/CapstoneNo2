using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public float riseHeight = 1.0f; // 튀어오르는 높이
    public float riseTime = 0.5f; // 튀어오르는 시간
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
        StartCoroutine(ActivateTrap());
    }

    private IEnumerator ActivateTrap()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f); // 대기 시간
            yield return MoveTrap(originalPosition + Vector3.up * riseHeight);
            yield return MoveTrap(originalPosition);
        }
    }

    private IEnumerator MoveTrap(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = transform.position;

        while (elapsedTime < riseTime)
        {
            transform.position = Vector3.Lerp(startingPos, targetPosition, (elapsedTime / riseTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition; // 정확한 위치 설정
    }
}
