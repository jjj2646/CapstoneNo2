using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickStartGame()
    {
        Debug.Log("새 게임");
    }

    public void onClickLoad()
    {
        Debug.Log("불러오기");
    }

    public void onClickOption()
    {
        Debug.Log("옵션");
    }

    public void onClickQuit()
    {
#if UNITY_EDITOR       
        UnityEditor.EditorApplication.isPlaying=false;
#else
        Application.Quit();
#endif
    }
}
