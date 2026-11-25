using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CupTarget : MonoBehaviour
{
    [Header("Cup Settings")]
    [SerializeField] private int points = 1;
    [SerializeField] private string cupName = "Cup";

    [Header("Cup Removal")]
    [SerializeField] private GameObject cupToRemove;

    [Header("Audio")]
    [SerializeField] private AudioClip cupHitSound;
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.8f;

    [Header("Score Manager")]
    [SerializeField] private TossScoreManager scoreManager;

    private bool hasBeenHit;

    private void Reset()
    {
        Collider triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenHit)
            return;

        ThrowableBall ball = other.GetComponentInParent<ThrowableBall>();
        if (ball == null)
            return;

        if (scoreManager == null)
        {
            Debug.LogError("CupTarget: Score Manager reference is missing on " + cupName);
            return;
        }

        if (!scoreManager.IsPlaying)
            return;

        if (!ball.TryRegisterScore())
            return;

        // Only mark this cup as hit if the score manager accepts the points.
        if (!scoreManager.AddPoints(points, cupName))
            return;

        hasBeenHit = true;

        // Play haptic feedback on the controller that last grabbed the ball.
        ball.PlayScoreHaptic();

        // Play the sound at the cup's world position. This continues even
        // after the cup GameObject is disabled.
        if (cupHitSound != null)
        {
            AudioSource.PlayClipAtPoint(
                cupHitSound,
                transform.position,
                soundVolume
            );
        }

        // Reset the ball and remove this cup from play.
        ball.ResetToStart();

        if (cupToRemove != null)
        {
            cupToRemove.SetActive(false);
            Debug.Log(cupName + " scored and was removed.");
        }
        else
        {
            Debug.LogWarning("CupTarget: Cup To Remove is not assigned on " + cupName);
        }
    }

    public void ResetCup()
    {
        hasBeenHit = false;

        if (cupToRemove != null)
            cupToRemove.SetActive(true);

        Debug.Log(cupName + " restored.");
    }
}