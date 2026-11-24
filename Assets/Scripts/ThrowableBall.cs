using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class ThrowableBall : MonoBehaviour
{
    private bool scoredThisThrow = false;
    private HapticImpulsePlayer lastHapticPlayer;
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private Transform startingParent;

    private Coroutine resetCoroutine;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        // Remember the original spawn position.
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        startingParent = transform.parent;
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // A new grab allows another scoring attempt.
        scoredThisThrow = false;

        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }
        // Remember which controller grabbed the ball.
        Transform hand = args.interactorObject.transform;

        lastHapticPlayer = hand.GetComponent<HapticImpulsePlayer>();

        if (lastHapticPlayer == null)
        {
            lastHapticPlayer = hand.GetComponentInParent<HapticImpulsePlayer>();
        }

        if (lastHapticPlayer == null)
        {
            Debug.LogWarning("No HapticImpulsePlayer found on grabbing hand!");
        }
    }

    public bool TryRegisterScore()
    {
        if (scoredThisThrow)
            return false;

        scoredThisThrow = true;
        return true;
    }

    public void ResetToStart()
    {
        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);

        resetCoroutine = StartCoroutine(ResetBallRoutine());
    }


    private IEnumerator ResetBallRoutine()
    {
        // Wait briefly after scoring.
        yield return new WaitForSeconds(1f);

        if (grabInteractable.isSelected)
        {
            resetCoroutine = null;
            yield break;
        }

        // Stop all movement.
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Restore original parent.
        transform.SetParent(startingParent, true);

        // Reset the physics position.
        rb.position = startingPosition;
        rb.rotation = startingRotation;

        // Also reset the visible Transform.
        transform.SetPositionAndRotation(
            startingPosition,
            startingRotation
        );

        // Synchronise Unity's physics transforms.
        Physics.SyncTransforms();

        rb.Sleep();

        resetCoroutine = null;

        Debug.Log("Ball visually and physically reset!");
    }
    public void PlayScoreHaptic()
    {
        if (lastHapticPlayer == null)
        {
            Debug.LogWarning("Cannot play score haptic - no hand recorded!");
            return;
        }

        lastHapticPlayer.SendHapticImpulse(0.75f, 0.2f);

        Debug.Log("Scoring haptic sent!");
    }
}
