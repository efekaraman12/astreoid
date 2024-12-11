using UnityEngine;
using Terresquall;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField]
    private Bullet bulletPrefab;

    public float thrustSpeed = 1f;
    public float rotationSpeed = 0.1f;

    public float respawnDelay = 3f;
    public float respawnInvulnerability = 3f;

    public bool screenWrapping = true;
    private Bounds screenBounds;

    // Joystick bağlantısı
    public VirtualJoystick movementJoystick;
    public VirtualJoystick fireJoystick;

    // Ateş etme zamanlaması
    public float fireCooldown = 0.5f; // Her ateş arasındaki minimum süre (saniye)
    private float lastFireTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject[] boundaries = GameObject.FindGameObjectsWithTag("Boundary");

        // Disable all boundaries if screen wrapping is enabled
        for (int i = 0; i < boundaries.Length; i++)
        {
            boundaries[i].SetActive(!screenWrapping);
        }

        // Convert screen space bounds to world space bounds
        screenBounds = new Bounds();
        screenBounds.Encapsulate(Camera.main.ScreenToWorldPoint(Vector3.zero));
        screenBounds.Encapsulate(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f)));
    }

    private void Update()
    {
        // Ateş etme joystick veya tuşla
        if (fireJoystick.axis.magnitude > 0.5f && Time.time >= lastFireTime + fireCooldown)
        {
            Shoot();
            lastFireTime = Time.time; // Son ateş zamanını güncelle
        }
    }

    private void FixedUpdate()
    {
        // Hareket joystick'ten alınan eksen değerleri
        Vector2 movementInput = movementJoystick.axis;

        if (movementInput != Vector2.zero)
        {
            // Yönlendirme
            float turnDirection = -movementInput.x; // Joystick yatay ekseni
            rb.AddTorque(rotationSpeed * turnDirection);

            // İleri hareket
            rb.AddForce(transform.right * movementInput.y * thrustSpeed); // Joystick dikey ekseni
        }

        if (screenWrapping)
        {
            ScreenWrap();
        }
    }

    private void ScreenWrap()
    {
        if (rb.position.x > screenBounds.max.x + 0.5f)
        {
            rb.position = new Vector2(screenBounds.min.x - 0.5f, rb.position.y);
        }
        else if (rb.position.x < screenBounds.min.x - 0.5f)
        {
            rb.position = new Vector2(screenBounds.max.x + 0.5f, rb.position.y);
        }
        else if (rb.position.y > screenBounds.max.y + 0.5f)
        {
            rb.position = new Vector2(rb.position.x, screenBounds.min.y - 0.5f);
        }
        else if (rb.position.y < screenBounds.min.y - 0.5f)
        {
            rb.position = new Vector2(rb.position.x, screenBounds.max.y + 0.5f);
        }
    }

    private void Shoot()
    {
        Bullet bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        bullet.Shoot(transform.right);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            // Çarpışma sonrası hızları sıfırla
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;

            // Oyuncunun ölümünü tetikle
            GameManager.Instance.OnPlayerDeath(this);
        }
    }
}
