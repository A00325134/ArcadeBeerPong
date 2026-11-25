using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreboardUI : MonoBehaviour
{
    [Header("Score Display")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Status Display")]
    [SerializeField] private TMP_Text statusText;

    [Header("Message Timing")]
    [SerializeField] private float hitMessageDuration = 2.5f;
    [SerializeField] private string defaultAfterHitMessage = "THROW AGAIN!";

    private Coroutine messageCoroutine;

    public void RefreshScore(int currentScore, int targetScore, string message)
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + currentScore + " / " + targetScore;
        }

        if (statusText != null)
        {
            statusText.text = message;

            if (messageCoroutine != null)
            {
                StopCoroutine(messageCoroutine);
                messageCoroutine = null;
            }

            if (message.Contains("HIT!"))
            {
                messageCoroutine = StartCoroutine(ResetHitMessage());
            }
        }
    }

    private IEnumerator ResetHitMessage()
    {
        yield return new WaitForSeconds(hitMessageDuration);

        if (statusText != null)
            statusText.text = defaultAfterHitMessage;

        messageCoroutine = null;
    }
}