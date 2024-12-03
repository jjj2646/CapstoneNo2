using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMove : MonoBehaviour
{
    public AudioSource audioSource; // AudioSource 컴포넌트를 연결할 변수
    public AudioClip buttonClickSound; // 버튼 클릭 사운드를 연결할 변수

    public void GameMoveCtrl()
    {
        // 버튼 클릭 시 사운드 재생
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound); // 버튼 클릭 사운드 재생
        }

        // StartMap으로 이동한 후 코루틴 실행
        StartCoroutine(LoadStartMapAndThenMove());
    }

    private IEnumerator LoadStartMapAndThenMove()
    {
        // 2초 대기
        yield return new WaitForSeconds(2f);

        // StartMap으로 이동
        SceneManager.LoadScene("StartMap");
        Debug.Log("Moved to StartMap");

        // 2초 후에 다른 씬으로 이동
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Map");
        Debug.Log("Moved to Map");
    }
}
