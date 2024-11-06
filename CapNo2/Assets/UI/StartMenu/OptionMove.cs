using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionMove : MonoBehaviour
{
    public void OptionMoveCtrl()
    {
        SceneManager.LoadScene("OptionScene"); //어떤 씬 이름으로 이동할 건지
        Debug.Log("OptionScene Go");
    }
    
}
