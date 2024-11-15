using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameManager>();
            }
            return instance;
        }
    }
    #endregion
    
    public UIManager uiManager;
    
    public int score;
    
    
    public void GameOver()
    {
        uiManager.ShowGameOver();
        Time.timeScale = 0.01f;
    }
    
    public void AddScore(int amount)
    {
        score += amount;
    }
}
