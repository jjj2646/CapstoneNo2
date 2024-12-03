using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceToNextScene : MonoBehaviour
{
    public string Map; // 이동할 씬 이름을 Unity Inspector에서 설정

    void Update()
    {
        // 스페이스바 입력 감지
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 다음 씬으로 이동
            SceneManager.LoadScene(Map);
            Debug.Log("Moved to scene: " + Map);
        }
    }
}

