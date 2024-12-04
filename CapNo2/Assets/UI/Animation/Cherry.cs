using UnityEngine;
using UnityEngine.SceneManagement;

public class Cherry : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player와 충돌했는지 확인
        if (collision.CompareTag("Player")) // Player의 태그가 "Player"인지 확인
        {
            Debug.Log("Cherry Collected! Loading Clear Scene...");
            SceneManager.LoadScene("ClearScene"); // ClearScene으로 전환
        }
    }
}
