using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;
    public bool RopeCollected = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
