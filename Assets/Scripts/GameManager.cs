using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game References")]
    [SerializeField] private Player player;
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;

    [Header("Audio")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip explosionSound;

    public int score { get; private set; } = 0;
    public int lives { get; private set; } = 3;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }

        if (musicAudioSource == null)
        {
            musicAudioSource = gameObject.AddComponent<AudioSource>();
            musicAudioSource.loop = true;
            musicAudioSource.playOnAwake = false;
        }

        if (sfxAudioSource == null)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
            sfxAudioSource.loop = false;
            sfxAudioSource.playOnAwake = false;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
        PlayBackgroundMusic();
    }

    private void Update()
    {
        if (lives <= 0 && gameOverUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Debug.Log("Restarting game from keyboard...");
                NewGame();
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("Restarting game from touch...");
                    NewGame();
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Restarting game from mouse click...");
                NewGame();
            }
        }
    }

    private void NewGame()
    {
        Asteroid[] asteroids = FindObjectsOfType<Asteroid>();

        for (int i = 0; i < asteroids.Length; i++)
        {
            Destroy(asteroids[i].gameObject);
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("GameOverUI is not assigned!");
        }

        SetScore(0);
        SetLives(3);

        if (player != null)
        {
            player.transform.position = Vector3.zero;
            player.transform.rotation = Quaternion.identity;
            player.gameObject.SetActive(true);

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
        else
        {
            Debug.LogError("Player reference is missing in GameManager!");
        }
    }

    private void SetScore(int newScore)
    {
        this.score = newScore;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + this.score.ToString();
        }
        else
        {
            Debug.LogWarning("Score Text is not assigned in GameManager!");
        }
    }

    private void SetLives(int newLives)
    {
        this.lives = newLives;
        if (livesText != null)
        {
            livesText.text = "Lives: " + this.lives.ToString();
        }
        else
        {
            Debug.LogWarning("Lives Text is not assigned in GameManager!");
        }
    }

    private void Respawn()
    {
        player.transform.position = Vector3.zero;
        player.gameObject.SetActive(true);
    }

    private void PlayBackgroundMusic()
    {
        if (musicAudioSource != null && backgroundMusic != null)
        {
            musicAudioSource.clip = backgroundMusic;
            musicAudioSource.Play();
        }
    }

    public void PlayShootSound()
    {
        if (sfxAudioSource != null && shootSound != null)
        {
            sfxAudioSource.PlayOneShot(shootSound);
        }
    }

    private void PlayExplosionSound()
    {
        if (sfxAudioSource != null && explosionSound != null)
        {
            sfxAudioSource.PlayOneShot(explosionSound);
        }
    }

    public void OnAsteroidDestroyed(Asteroid asteroid)
    {
        explosionEffect.transform.position = asteroid.transform.position;
        explosionEffect.Play();
        PlayExplosionSound();

        if (asteroid.size < 0.7f)
        {
            SetScore(score + 100); // small asteroid
        }
        else if (asteroid.size < 1.4f)
        {
            SetScore(score + 50); // medium asteroid
        }
        else
        {
            SetScore(score + 25); // large asteroid
        }
    }

    public void OnPlayerDeath(Player player)
    {
        player.gameObject.SetActive(false);

        explosionEffect.transform.position = player.transform.position;
        explosionEffect.Play();

        SetLives(lives - 1);

        if (lives <= 0)
        {
            gameOverUI.SetActive(true);
        }
        else
        {
            Invoke(nameof(Respawn), player.respawnDelay);
        }
    }
}
