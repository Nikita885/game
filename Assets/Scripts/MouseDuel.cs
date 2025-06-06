using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseDuel : MonoBehaviour
{
    public Transform player1; // мышь посередине
    public Transform player2Prefab; // префаб второго мышонка
    public Text scoreText;
    public float outsideDuration = 1f; // время, чтобы выжить вне

    private int player1Score = 0;
    private int player2Score = 0;
    private Transform currentPlayer2;
    private bool isPlayer2Outside = false;

    private float screenLeft, screenRight, screenTop, screenBottom;

    void Start()
    {
        UpdateScreenBounds();
        SpawnPlayer2();
        UpdateScoreText();
    }

    void UpdateScreenBounds()
    {
        Camera cam = Camera.main;
        Vector3 bottomLeft = cam.ScreenToWorldPoint(Vector3.zero);
        Vector3 topRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        screenLeft = bottomLeft.x;
        screenRight = topRight.x;
        screenBottom = bottomLeft.y;
        screenTop = topRight.y;
    }

    void SpawnPlayer2()
    {
        if (currentPlayer2 != null)
            Destroy(currentPlayer2.gameObject);

        Vector3 spawnPos = GetRandomSpawnPositionOutside();
        currentPlayer2 = Instantiate(player2Prefab, spawnPos, Quaternion.identity);
        isPlayer2Outside = true;
        StartCoroutine(Player2OutsideTimer());
    }

    Vector3 GetRandomSpawnPositionOutside()
    {
        int side = Random.Range(0, 4); // 0=лево,1=право,2=верх,3=низ
        float x = 0, y = 0;
        switch (side)
        {
            case 0: // слева
                x = screenLeft - 1f;
                y = Random.Range(screenBottom, screenTop);
                break;
            case 1: // справа
                x = screenRight + 1f;
                y = Random.Range(screenBottom, screenTop);
                break;
            case 2: // сверху
                x = Random.Range(screenLeft, screenRight);
                y = screenTop + 1f;
                break;
            case 3: // снизу
                x = Random.Range(screenLeft, screenRight);
                y = screenBottom - 1f;
                break;
        }
        return new Vector3(x, y, 0);
    }

    IEnumerator Player2OutsideTimer()
    {
        yield return new WaitForSeconds(outsideDuration);

        if (isPlayer2Outside)
        {
            player2Score++;
            UpdateScoreText();
            CheckGameOver();
            SpawnPlayer2();
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = $"Игрок 1: {player1Score}  |  Игрок 2: {player2Score}";
    }

    void CheckGameOver()
    {
        if (player1Score >= 10)
        {
            scoreText.text = "Игрок 1 победил!";
            enabled = false;
        }
        else if (player2Score >= 10)
        {
            scoreText.text = "Игрок 2 победил!";
            enabled = false;
        }
    }

    // Удары игрока 1
    void Update()
    {
        if (!enabled) return;

        Vector3 hitPos = player1.position;

        if (Input.GetKeyDown(KeyCode.W)) // удар вверх
            hitPos += Vector3.up;
        else if (Input.GetKeyDown(KeyCode.S)) // удар вниз
            hitPos += Vector3.down;
        else if (Input.GetKeyDown(KeyCode.A)) // удар влево
            hitPos += Vector3.left;
        else if (Input.GetKeyDown(KeyCode.D)) // удар вправо
            hitPos += Vector3.right;
        else
            return;

        // Проверяем, находится ли Player2 в позиции удара (+-0.5 по координате)
        if (currentPlayer2 != null && Vector3.Distance(currentPlayer2.position, hitPos) < 0.75f)
        {
            player1Score++;
            UpdateScoreText();
            CheckGameOver();
            SpawnPlayer2();
        }
    }

    // Проверяем, если Player2 вышел за экран — он вне зоны
    void LateUpdate()
    {
        if (currentPlayer2 == null) return;

        if (currentPlayer2.position.x < screenLeft - 0.5f ||
            currentPlayer2.position.x > screenRight + 0.5f ||
            currentPlayer2.position.y < screenBottom - 0.5f ||
            currentPlayer2.position.y > screenTop + 0.5f)
        {
            isPlayer2Outside = true;
        }
        else
        {
            isPlayer2Outside = false;
        }
    }
}
