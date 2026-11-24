
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

    private bool hasBeenHit = false;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenHit)
            return;

        // Only accept our throwable ball.
        ThrowableBall ball =
            other.GetComponentInParent<ThrowableBall>();

        if (ball == null)
            return;

        if (scoreManager == null)
        {
            Debug.LogError("CupTarget: Score Manager missing!");
            return;
        }

        // Don't allow scoring before the game starts.
        if (!scoreManager.IsPlaying)
            return;

        // Prevent the same throw from scoring more than once.
        if (!ball.TryRegisterScore())
            return;

        // Award points. Only continue if scoring succeeded.
        if (!scoreManager.AddPoints(points, cupName))
            return;

        hasBeenHit = true;

        // Play scoring haptic on the throwing controller.
        ball.PlayScoreHaptic();

        // Play cup hit sound.
        if (cupHitSound != null)
        {
            AudioSource.PlayClipAtPoint(
                cupHitSound,
                transform.position,
                soundVolume
            );
        }

        // Return the ball to its starting position.
        ball.ResetToStart();

        // Remove the successfully hit cup.
        if (cupToRemove != null)
        {
            cupToRemove.SetActive(false);

            Debug.Log(cupName + " removed!");
        }
        else
        {
            Debug.LogWarning(
                cupName + " has no removal object assigned!"
            );
        }
    }

    // Restore cup when restarting the game.
    public void ResetCup()
    {
        hasBeenHit = false;

        if (cupToRemove != null)
        {
            cupToRemove.SetActive(true);
        }

        Debug.Log(cupName + " restored!");
    }
}
