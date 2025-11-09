using UnityEngine;

public class StoneEnemyRight : MonoBehaviour
{
    [Header("Fireball Spawn Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float fireballSpeed = 4f;

    private float spawnTimer = 0f;

    private void Start()
    {
        spawnTimer = spawnInterval;
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
            GameObject fireballInstance = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

            FireballRight fireball = fireballInstance.GetComponent<FireballRight>();
            if (fireball != null)
            {
                fireball.Initialize(fireballSpeed); // Always goes right
            }

            spawnTimer = spawnInterval;
        }
    }
}
