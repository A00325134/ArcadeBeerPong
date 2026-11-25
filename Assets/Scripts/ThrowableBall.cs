
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class ThrowableBall : MonoBehaviour
{
    [Header("Reset Settings")]
    [SerializeField] private float resetDelay = 1f;
    [SerializeField] private float missResetDelay = 3f;

    [Header("Scoring Haptics")]
    [Range(0f, 1f)]
    [SerializeField] private float scoreHapticAmplitude = 0.75f;
    [SerializeField] private float scoreHapticDuration = 0.2f;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private Transform startingParent;

    private bool scoredThisThrow;

    private HapticImpulsePlayer lastHapticPlayer;

    private Coroutine resetCoroutine;
    private Coroutine missResetCoroutine;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        startingPosition = transform.position;
        startingRotation = transform.rotation;
        startingParent = transform.parent;
    }

    private void OnEnable()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }

        if (missResetCoroutine != null)
            StopCoroutine(missResetCoroutine);

        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // New throw attempt.
        scoredThisThrow = false;

        // Remember the controller that grabbed the ball.
        Transform interactorTransform = args.interactorObject.transform;

        HapticImpulsePlayer foundPlayer =
            interactorTransform.GetComponent<HapticImpulsePlayer>();

        if (foundPlayer == null)
            foundPlayer = interactorTransform.GetComponentInParent<HapticImpulsePlayer>();

        // Don't overwrite the controller reference when a socket catches the ball.
        if (foundPlayer != null)
            lastHapticPlayer = foundPlayer;

        // Cancel any pending missed-throw reset.
        if (missResetCoroutine != null)
        {
            StopCoroutine(missResetCoroutine);
            missResetCoroutine = null;
        }

        // Cancel any pending ball reset if the player grabs it again.
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }

        Debug.Log("Ball grabbed - new throw enabled.");
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // Don't start a miss countdown if this throw already scored.
        if (scoredThisThrow)
            return;

        if (missResetCoroutine != null)
            StopCoroutine(missResetCoroutine);

        missResetCoroutine = StartCoroutine(MissResetRoutine());

        Debug.Log("Ball thrown - 3 second miss countdown started.");
    }

    private IEnumerator MissResetRoutine()
    {
        yield return new WaitForSeconds(missResetDelay);

        missResetCoroutine = null;

        // Only reset if the ball hasn't scored or been grabbed again.
        if (!scoredThisThrow && !grabInteractable.isSelected)
        {
            Debug.Log("Missed throw! Returning ball to holder.");

            ResetBallImmediately();
        }
    }

    public bool TryRegisterScore()
    {
        if (scoredThisThrow)
        {
            Debug.Log("This throw has already scored.");
            return false;
        }

        scoredThisThrow = true;

        // A successful score cancels the missed-throw countdown.
        if (missResetCoroutine != null)
        {
            StopCoroutine(missResetCoroutine);
            missResetCoroutine = null;
        }

        return true;
    }

    public void PlayScoreHaptic()
    {
        if (lastHapticPlayer == null)
        {
            Debug.Log("No controller HapticImpulsePlayer was recorded.");
            return;
        }

        lastHapticPlayer.SendHapticImpulse(
            scoreHapticAmplitude,
            scoreHapticDuration
        );
    }

    // Normal successful-shot reset (uses existing delay).
    public void ResetToStart()
    {
        BeginReset(resetDelay);
    }

    // Missed-shot reset (returns immediately after the 3 second countdown).
    private void ResetBallImmediately()
    {
        BeginReset(0f);
    }

    private void BeginReset(float delay)
    {
        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);

        resetCoroutine = StartCoroutine(ResetBallRoutine(delay));
    }

    private IEnumerator ResetBallRoutine(float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        if (grabInteractable.isSelected)
        {
            resetCoroutine = null;
            yield break;
        }

        transform.SetParent(startingParent, true);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = startingPosition;
        rb.rotation = startingRotation;

        transform.SetPositionAndRotation(
            startingPosition,
            startingRotation
        );

        Physics.SyncTransforms();
        rb.WakeUp();

        resetCoroutine = null;

        Debug.Log("Ball reset to starting position.");
    }
}
