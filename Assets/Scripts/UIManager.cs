using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public ZombieSpawner spawner;
    public BasicPlayerController playerController;
    
    public TextMeshProUGUI zombieCountText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI healthText;
    public Image healthBar;
    public GameObject gameOverScreen;
    
    private void Update()
    {
        UpdateZombieCount();
        UpdateTime();
        UpdateScore();
        UpdateHealth();
    }
    
    private void UpdateZombieCount()
    {
        zombieCountText.text = $"Zombies: {spawner.currentZombies}/{spawner.maxZombies}";
    }
    
    public void ShowGameOver()
    {
        // Show game over screen
        gameOverScreen.SetActive(true);
    }
    
    private void UpdateTime()
    {
        timeText.text = $"Time: {Time.timeSinceLevelLoad:F2}";
    }
    
    private void UpdateScore()
    {
        scoreText.text = $"Score: {GameManager.Instance.score}";
    }
    
    private void UpdateHealth()
    {
        healthText.text = $"{playerController.CurrentHealth}/{playerController.maxHealth}";
        healthBar.fillAmount = playerController.CurrentHealth / playerController.maxHealth;
    }
}
