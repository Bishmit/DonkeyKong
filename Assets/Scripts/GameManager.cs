using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int lives;
    private int score;
    private int level;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        NewGame();
    }

    private void NewGame()
    {
        lives = 3;
        score = 0;
        LoadLevel(1);
    }

    private void LoadLevel(int index)
    {
        level = index;

        Camera camera = Camera.main;
        if (camera != null)
        {
            camera.cullingMask = 0; // Optional fade out
        }

        Invoke(nameof(LoadScene), 1f);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(level);
    }

    public void LevelComplete()
    {
        score += 100;
        int nextLevel = level + 1;

        if (nextLevel < SceneManager.sceneCountInBuildSettings)
        {
            LoadLevel(nextLevel);
        }
        else
        {
            Debug.Log("All levels complete! Restarting last level.");
            LoadLevel(level); // Replay the current (last) level
        }
    }

    public void LevelFailed()
    {
        lives--;
        if (lives <= 0)
        {
            Debug.Log("Game Over! Restarting game.");
            NewGame();
        }
        else
        {
            Debug.Log($"Retrying level. Lives left: {lives}");
            LoadLevel(level); // Reload current level
        }
    }
}
