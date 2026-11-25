
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class TrajectoryPreview : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer trajectoryLine;

    [Header("Trajectory Settings")]
    [SerializeField] private int pointCount = 30;
    [SerializeField] private float predictionTime = 1.2f;
    [SerializeField] private float velocityMultiplier = 1f;
    [SerializeField] private float minimumPreviewSpeed = 0.15f;

    [Header("Velocity Tracking")]
    [SerializeField] private float velocitySmoothing = 16f;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private Vector3 previousPosition;
    private Vector3 estimatedVelocity;

    private bool wasSelected;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        if (trajectoryLine != null)
        {
            trajectoryLine.useWorldSpace = true;
            trajectoryLine.positionCount = 0;

            trajectoryLine.startWidth = 0.008f;
            trajectoryLine.endWidth = 0.003f;

            trajectoryLine.enabled = false;
        }
        else
        {
            Debug.LogError("TrajectoryPreview: Line Renderer missing!");
        }
    }

    private void Update()
    {
        if (trajectoryLine == null)
            return;

        if (grabInteractable.isSelected)
        {
            if (!wasSelected)
            {
                previousPosition = transform.position;
                estimatedVelocity = Vector3.zero;
                wasSelected = true;

                return;
            }

            float deltaTime = Mathf.Max(Time.deltaTime, 0.0001f);

            Vector3 currentPosition = transform.position;

            Vector3 currentVelocity =
                (currentPosition - previousPosition) / deltaTime;

            float smoothing = 1f -
                Mathf.Exp(-velocitySmoothing * deltaTime);

            estimatedVelocity = Vector3.Lerp(
                estimatedVelocity,
                currentVelocity,
                smoothing
            );

            previousPosition = currentPosition;

            if (estimatedVelocity.magnitude >= minimumPreviewSpeed)
            {
                DrawTrajectory();
            }
            else
            {
                trajectoryLine.enabled = false;
            }
        }
        else
        {
            wasSelected = false;
            trajectoryLine.enabled = false;
        }
    }

    private void DrawTrajectory()
    {
        trajectoryLine.enabled = true;
        trajectoryLine.positionCount = pointCount;

        Vector3 startPosition = transform.position;
        Vector3 startVelocity =
            estimatedVelocity * velocityMultiplier;

        for (int i = 0; i < pointCount; i++)
        {
            float time = predictionTime *
                i / (pointCount - 1f);

            Vector3 position =
                startPosition +
                startVelocity * time +
                0.5f * Physics.gravity * time * time;

            trajectoryLine.SetPosition(i, position);
        }
    }
}
