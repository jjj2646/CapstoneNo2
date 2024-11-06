using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuMove : MonoBehaviour
{
    public void MainMenuMoveCtrl()
    {
        SceneManager.LoadScene("FirstScene"); //어떤 씬 이름으로 이동할 건지
        Debug.Log("FirstScene Go");
    }
}
