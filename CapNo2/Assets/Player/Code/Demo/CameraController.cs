using UnityEngine;

public class CameraController : MonoBehaviour
{
    [System.Serializable]
    public struct CameraBounds
    {
        public Vector2 center; // 구역의 중심점
        public Vector2 size;   // 구역의 크기
    }

    [SerializeField]
    Transform playerTransform; // 플레이어 Transform
    [SerializeField]
    Vector3 cameraPosition;    // 카메라의 상대 위치
    [SerializeField]
    CameraBounds[] cameraBounds; // 여러 제한 구역 설정

    [SerializeField]
    float cameraMoveSpeed = 5f; // 카메라 이동 속도

    Camera mainCamera;
    float height;
    float width;
    CameraBounds currentBounds;

    void Start()
    {
        // "HeroKnight" 오브젝트를 찾고 Transform을 할당
        playerTransform = GameObject.Find("HeroKnight").GetComponent<Transform>();

        // 메인 카메라 가져오기
        mainCamera = Camera.main;

        // 카메라의 너비와 높이 계산
        height = mainCamera.orthographicSize;
        width = height * Screen.width / Screen.height;

        // 처음 시작 시 캐릭터 위치에 따라 초기 영역 설정
        currentBounds = GetCurrentBounds();
    }

    void FixedUpdate()
    {
        // 현재 캐릭터 위치에 따른 제한 구역 업데이트
        CameraBounds newBounds = GetCurrentBounds();

        // 캐릭터가 다른 구역으로 이동한 경우 currentBounds를 업데이트
        if (newBounds.center != currentBounds.center || newBounds.size != currentBounds.size)
        {
            currentBounds = newBounds;
        }

        LimitCameraArea();
    }

    void LimitCameraArea()
    {
        // 플레이어 위치에 카메라의 상대적 위치를 더해 목표 위치 설정
        Vector3 targetPosition = playerTransform.position + cameraPosition;

        // 목표 위치로 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraMoveSpeed);

        // 현재 구역의 너비와 높이를 기준으로 제한
        float lx = currentBounds.size.x / 2 - width;
        float clampX = Mathf.Clamp(transform.position.x, currentBounds.center.x - lx, currentBounds.center.x + lx);

        float ly = currentBounds.size.y / 2 - height;
        float clampY = Mathf.Clamp(transform.position.y, currentBounds.center.y - ly, currentBounds.center.y + ly);

        // 제한된 위치로 카메라 설정
        transform.position = new Vector3(clampX, clampY, -10f);
    }

    CameraBounds GetCurrentBounds()
    {
        // 플레이어 위치에 해당하는 제한 구역을 찾음
        foreach (CameraBounds bounds in cameraBounds)
        {
            float halfWidth = bounds.size.x / 2;
            float halfHeight = bounds.size.y / 2;

            if (playerTransform.position.x > bounds.center.x - halfWidth &&
                playerTransform.position.x < bounds.center.x + halfWidth &&
                playerTransform.position.y > bounds.center.y - halfHeight &&
                playerTransform.position.y < bounds.center.y + halfHeight)
            {
                return bounds;
            }
        }

        // 기본값으로 첫 번째 구역 반환 (또는 기본 구역을 설정 가능)
        return cameraBounds[0];
    }

    private void OnDrawGizmos()
    {
        // 각 카메라 제한 구역 시각화
        Gizmos.color = Color.red;
        foreach (CameraBounds bounds in cameraBounds)
        {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}
