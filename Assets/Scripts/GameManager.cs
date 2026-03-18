using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
public int money = 0;
    public int targetMoney = 100;

    public int ordersCompleted = 0;

    public float dayTime = 190f;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI orderText;
    public GameObject winPanel;
    public GameObject losePanel;

    private bool gameEnded = false;

    void Start()
    {
        Time.timeScale = 1f;
        UpdateUI();

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    void Update()
    {
        if (gameEnded) return;

        dayTime -= Time.deltaTime;

        if (dayTime < 0f)
            dayTime = 0f;

        UpdateUI();

        if (dayTime <= 0f)
        {
            EndDay();
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateUI();
    }

    public void AddCompletedOrder()
    {
        ordersCompleted++;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = "MONEY: $" + money + " / $" + targetMoney;

        if (timerText != null)
            timerText.text = "TIME: " + Mathf.CeilToInt(dayTime);

        if (orderText != null)
            orderText.text = "ORDERS: " + ordersCompleted;
    }

    void EndDay()
    {
        if (money >= targetMoney)
        {
            WinGame();
        }
        else
        {
            LoseGame();
        }
    }

    void WinGame()
    {
        gameEnded = true;
        Debug.Log("You win!");

        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    void LoseGame()
    {
        gameEnded = true;
        Debug.Log("You lose!");

        if (losePanel != null)
            losePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

        public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}