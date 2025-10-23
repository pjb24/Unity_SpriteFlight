using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public GameObject boosterFlame;

    public float maxSpeed = 5f;
    public float thrustForce = 1f;
    Rigidbody2D rb;

    private float elapsedTime = 0f;
    private float score = 0f;
    public float scoreMultiplier = 10f;
    private int highScore = 0;

    public UIDocument uiDocument;

    private Label scoreText;
    private Button restartButton;
    private Label highScoreText;

    public GameObject explosionEffect;

    public GameObject borderParent;

    public InputAction moveForward;
    public InputAction lookPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");

        restartButton.style.display = DisplayStyle.None;

        restartButton.clicked += ReloadScene;

        highScoreText = uiDocument.rootVisualElement.Q<Label>("HighScore");
        highScoreText.style.display = DisplayStyle.None;

        moveForward.Enable();
        lookPosition.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
        MovePlayer();
    }

    void UpdateScore()
    {
        elapsedTime += Time.deltaTime;
        score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);
        Debug.Log("Score: " + score);

        scoreText.text = "점수: " + score;
    }

    void MovePlayer()
    {
        //if (Mouse.current.leftButton.isPressed)
        if (moveForward.IsPressed())
        {
            // Calculate mouse direction
            // 마우스 방향 계산
            //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(lookPosition.ReadValue<Vector2>());
            Vector2 direction = (mousePos - transform.position).normalized;

            // Move player in direction of mouse
            // 마우스 방향으로 플레이어를 이동
            transform.up = direction;
            rb.AddForce(direction * thrustForce);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        //if (Mouse.current.leftButton.wasPressedThisFrame)
        if (moveForward.WasPressedThisFrame())
        {
            boosterFlame.SetActive(true);
        }
        //else if (Mouse.current.leftButton.wasReleasedThisFrame)
        else if (moveForward.WasReleasedThisFrame())
        {
            boosterFlame.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);

        if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");    
        }
        
        if (highScore < score)
        {
            highScore = Mathf.FloorToInt(score);
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        highScoreText.text = "최고 점수: " + highScore;
        highScoreText.style.display = DisplayStyle.Flex;

        restartButton.style.display = DisplayStyle.Flex;

        borderParent.SetActive(false);

        Destroy(gameObject);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
