using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI challengeText;
    [SerializeField] private TextMeshProUGUI[] playerScoreTexts;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI[] playerPowerupTexts;
    [SerializeField] private float messageDuration = 2f;

    [SerializeField] private float gameTime = 300f; // 5 minutes
    private float remainingTime;
    private bool gameActive = true;

    private void Start()
    {
        remainingTime = gameTime;
        UpdateUI();
    }

    private void Update()
    {
        if (gameActive)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0)
            {
                remainingTime = 0;
                EndGame();
            }

            UpdateTimerDisplay();
        }
    }

    private void EndGame()
    {
        gameActive = false;
        int winningPlayer = GetWinningPlayerIndex();
        ShowGameOverMessage($"¡Jugador {winningPlayer + 1} ganó con {playerScoreTexts[winningPlayer].text} puntos!");
    }

    private int GetWinningPlayerIndex()
    {
        int highestScore = -1;
        int winningPlayer = 0;

        for (int i = 0; i < playerScoreTexts.Length; i++)
        {
            int score = int.Parse(playerScoreTexts[i].text);
            if (score > highestScore)
            {
                highestScore = score;
                winningPlayer = i;
            }
        }

        return winningPlayer;
    }

    public void UpdateChallengeText(string text)
    {
        challengeText.text = text;
    }

    public void UpdateScores(int[] scores)
    {
        for (int i = 0; i < scores.Length && i < playerScoreTexts.Length; i++)
        {
            playerScoreTexts[i].text = scores[i].ToString();
        }
    }

    public void ShowTokenCollected(int playerIndex, int tokenValue)
    {
        StartCoroutine(ShowMessageCoroutine($"Jugador {playerIndex + 1} recogió token {tokenValue}"));
    }

    public void ShowChallengeCompleteMessage(string text)
    {
        StartCoroutine(ShowMessageCoroutine(text));
    }

    public void ShowPowerupMessage(int playerIndex, string text)
    {
        if (playerIndex < playerPowerupTexts.Length)
        {
            playerPowerupTexts[playerIndex].text = text;
            StartCoroutine(FadePowerupText(playerIndex));
        }
    }

    public void ShowGameOverMessage(string text)
    {
        messageText.text = text;
        messageText.gameObject.SetActive(true);
    }

    private IEnumerator ShowMessageCoroutine(string text)
    {
        messageText.text = text;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(messageDuration);

        messageText.gameObject.SetActive(false);
    }

    private IEnumerator FadePowerupText(int playerIndex)
    {
        float duration = 3f;
        float elapsed = 0;

        Color startColor = playerPowerupTexts[playerIndex].color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (elapsed < duration)
        {
            playerPowerupTexts[playerIndex].color = Color.Lerp(startColor, endColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerPowerupTexts[playerIndex].color = startColor;
        playerPowerupTexts[playerIndex].text = "";
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateUI()
    {
        UpdateTimerDisplay();
    }
}