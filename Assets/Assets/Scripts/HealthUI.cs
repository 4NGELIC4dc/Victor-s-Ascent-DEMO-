using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public static HealthUI Instance { get; private set; }

    [SerializeField] private Image heartPrefab;
    [SerializeField] private Transform heartContainer;
    [SerializeField] private Color activeHeartColor = Color.white;
    [SerializeField] private Color inactiveHeartColor = Color.gray;

    private Image[] hearts;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        InitializeHearts();
    }

    private void InitializeHearts()
    {
        int maxLives = HealthManager.Instance.GetMaxLives();
        hearts = new Image[maxLives];

        // Destroy any existing hearts
        foreach (Transform child in heartContainer)
        {
            Destroy(child.gameObject);
        }

        // Create hearts
        for (int i = 0; i < maxLives; i++)
        {
            Image heart = Instantiate(heartPrefab, heartContainer);
            heart.color = activeHeartColor;
            hearts[i] = heart;
        }

        UpdateHearts(HealthManager.Instance.GetCurrentLives());
    }

    public void UpdateHearts(int currentLives)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentLives)
                hearts[i].color = activeHeartColor;
            else
                hearts[i].color = inactiveHeartColor;
        }
    }
}
