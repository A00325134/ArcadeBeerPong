
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CupTarget : MonoBehaviour
{
    [Header("Cup Settings")]
    [SerializeField] private int points = 1;
    [SerializeField] private string cupName = "Cup";

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

        ThrowableBall ball =
            other.GetComponentInParent<ThrowableBall>();

        if (ball == null)
            return;

        if (scoreManager == null)
        {
            Debug.LogError("CupTarget: Score Manager missing!");
            return;
        }

        if (!ball.TryRegisterScore())
            return;

        hasBeenHit = true;

        scoreManager.AddPoints(points, cupName);

        Debug.Log(cupName + " scored!");
    }
}
