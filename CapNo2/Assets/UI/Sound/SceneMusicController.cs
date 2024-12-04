using UnityEngine;

public class SceneMusicController : MonoBehaviour
{
    public AudioClip sceneMusic; // 이 씬의 배경음악

    private void Start()
    {
        // BackgroundMusicManager에서 음악 재생
        BackgroundMusicManager musicManager = FindObjectOfType<BackgroundMusicManager>();
        if (musicManager != null && sceneMusic != null)
        {
            musicManager.PlayMusic(sceneMusic);
        }
    }
}
