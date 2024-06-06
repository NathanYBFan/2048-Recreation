using System.Collections;
using UnityEngine;
using TMPro;

public sealed class GameManager : MonoBehaviour
{
    [SerializeField]
    private TileBoard board;

    [SerializeField]
    private CanvasGroup gameOver;

    [SerializeField]
    private TextMeshProUGUI currScoreText;

    [SerializeField]
    private TextMeshProUGUI hiScoreText;


    private int currentScore;

    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        AudioManager._Instance.PlayAudio(0);

        SetScore(0);
        hiScoreText.text = LoadHighscore().ToString();

        gameOver.alpha = 0f;
        gameOver.interactable = false;

        board.ResetBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()
    {
        AudioManager._Instance.PlayAudio(1);

        SetHighScore(currentScore);
        board.enabled = false;

        gameOver.interactable = true;
        StartCoroutine(Fade(gameOver, 1f, 1f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    public void IncreaseScore (int pointsGained)
    {
        SetScore(currentScore + pointsGained);
    }

    private void SetScore (int score)
    {
        currentScore = score;

        currScoreText.text = score.ToString();
    }

    private void SetHighScore(int highScore)
    {
        int highscore = LoadHighscore();

        if (currentScore > highscore)
        {
            PlayerPrefs.SetInt("hiscore", currentScore);
            hiScoreText.text = currentScore.ToString();
        }
    }

    private int LoadHighscore()
    {
        return PlayerPrefs.GetInt("hiscore", 0);
    }
}
