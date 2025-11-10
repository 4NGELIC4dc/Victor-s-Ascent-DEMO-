using UnityEngine;

public class StoneEnemyRight : MonoBehaviour
{
    [Header("Fireball Spawn Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float fireballSpeed = 4f;

    [Header("Audio")]
    [Tooltip("Assign fireballsfx.mp3 here.")]
    [SerializeField] private AudioClip fireballSfx;
    [SerializeField] private float hearingRadius = 8f;
    [SerializeField, Range(0f, 1f)] private float baseVolume = 1f;

    private float spawnTimer = 0f;
    private AudioSource audioSource;
    private Transform player;

    private void Start()
    {
        spawnTimer = spawnInterval;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning($"{name} has no AudioSource component. Add one to play fireball SFX.");
        }
        else
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;

        // (optional) update audioSource.volume as ambient (not strictly necessary for one-shot but fine)
        UpdateAudioVolume();

        if (spawnTimer <= 0f)
        {
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
            GameObject fireballInstance = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

            FireballRight fireball = fireballInstance.GetComponent<FireballRight>();
            if (fireball != null)
            {
                fireball.Initialize(fireballSpeed); // Always goes right
            }

            // Play SFX at spawn (PlayOneShot allows overlap)
            if (audioSource != null && fireballSfx != null)
            {
                float vol = GetDistanceScaledVolume();
                vol = Mathf.Clamp01(vol * 1.5f);
                audioSource.PlayOneShot(fireballSfx, vol);
            }

            spawnTimer = spawnInterval;
        }
    }

    private void UpdateAudioVolume()
    {
        if (audioSource == null) return;
        audioSource.volume = baseVolume * GetDistanceScaling();
    }

    private float GetDistanceScaledVolume()
    {
        // volume value for PlayOneShot (0..1)
        return baseVolume * GetDistanceScaling();
    }

    private float GetDistanceScaling()
    {
        if (player == null) return 1f;
        float dist = Vector2.Distance(player.position, transform.position);
        return Mathf.Clamp01(1f - (dist / hearingRadius));
    }
}
