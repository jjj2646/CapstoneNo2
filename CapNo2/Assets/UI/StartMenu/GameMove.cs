using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMove : MonoBehaviour
{
    public void GameMoveCtrl()
    {
        // StartMap으로 이동한 후 코루틴 실행
        StartCoroutine(LoadStartMapAndThenMove());
    }

    private IEnumerator LoadStartMapAndThenMove()
    {
        // StartMap으로 즉시 이동
        SceneManager.LoadScene("StartMap");
        Debug.Log("Moved to StartMap");

        // 2초 대기
        yield return new WaitForSeconds(2f);

        // 다른 씬으로 이동
        SceneManager.LoadScene("Map");
        Debug.Log("Moved to Map");
    }
}

