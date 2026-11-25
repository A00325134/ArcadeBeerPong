using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class ThrowableBall : MonoBehaviour
{
    [Header("Reset")]
    [SerializeField] private float resetDelay = 1f;

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

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        // Capture the ball's original pose when the scene starts.
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        startingParent = transform.parent;
    }

    private void OnEnable()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // A new grab starts a new scoring attempt.
        scoredThisThrow = false;

        // Remember the controller that grabbed the ball.
        // A socket normally has no HapticImpulsePlayer, so don't overwrite
        // the last controller reference when the socket catches the ball.
        Transform interactorTransform = args.interactorObject.transform;
        HapticImpulsePlayer foundPlayer =
            interactorTransform.GetComponent<HapticImpulsePlayer>();

        if (foundPlayer == null)
            foundPlayer = interactorTransform.GetComponentInParent<HapticImpulsePlayer>();

        if (foundPlayer != null)
            lastHapticPlayer = foundPlayer;

        // If the player grabs the ball during its reset delay, cancel the reset.
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }

        Debug.Log("Ball grabbed - new throw enabled.");
    }

    public bool TryRegisterScore()
    {
        if (scoredThisThrow)
        {
            Debug.Log("This throw has already scored.");
            return false;
        }

        scoredThisThrow = true;
        return true;
    }

    public void PlayScoreHaptic()
    {
        if (lastHapticPlayer == null)
        {
            Debug.Log("No controller HapticImpulsePlayer was recorded for this throw.");
            return;
        }

        lastHapticPlayer.SendHapticImpulse(
            scoreHapticAmplitude,
            scoreHapticDuration
        );
    }

    public void ResetToStart()
    {
        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);

        resetCoroutine = StartCoroutine(ResetBallRoutine());
    }

    private IEnumerator ResetBallRoutine()
    {
        yield return new WaitForSeconds(resetDelay);

        // Do not teleport a ball while it is still being held by a controller.
        if (grabInteractable.isSelected)
        {
            resetCoroutine = null;
            yield break;
        }

        // Restore its original parent and pose, and clear all physics motion.
        transform.SetParent(startingParent, true);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = startingPosition;
        rb.rotation = startingRotation;
        transform.SetPositionAndRotation(startingPosition, startingRotation);

        Physics.SyncTransforms();
        rb.WakeUp();

        resetCoroutine = null;
        Debug.Log("Ball visually and physically reset to its starting position.");
    }
}