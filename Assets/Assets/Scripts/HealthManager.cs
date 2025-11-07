using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }

    [SerializeField] private int maxLives = 3;
    private int currentLives;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        currentLives = maxLives;
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }

    public int GetMaxLives()
    {
        return maxLives;
    }

    public void TakeDamage(int damage = 1)
    {
        currentLives -= damage;
        currentLives = Mathf.Max(0, currentLives);

        // Update UI
        HealthUI.Instance?.UpdateHearts(currentLives);

        // If lives reach 0, trigger game over
        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    public void Heal(int amount = 1)
    {
        currentLives += amount;
        currentLives = Mathf.Min(maxLives, currentLives);
        HealthUI.Instance?.UpdateHearts(currentLives);
    }

    private void GameOver()
    {
        // TODO: Implement game over logic (restart level, show game over screen, etc.)
        Debug.Log("Game Over!");
    }
}
