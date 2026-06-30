using System;
using UnityEngine;

public class AimController : MonoBehaviour
{
    [SerializeField] private float castRadius = 3f;
    [SerializeField] private Transform targetIndicator;
    [SerializeField] private Collider ownerCollider;
    [SerializeField] private Transform targetCharacter;
    [SerializeField] private bool useMouseInput = true;
    public LayerMask groundLayer;

    private bool isAiming = false;
    private Camera mainCam;
    private Vector3 externalAimDirection = Vector3.zero;

    public event Action OnStopAiming;
    public event Action OnAimPositionUpdated;

    private void Start()
    {
        mainCam = Camera.main;

        // Auto-grab the collider if not assigned
        if (ownerCollider == null) ownerCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (!isAiming) return;

        // If no target is set, stop aiming
        if (targetCharacter == null)
        {
            if (targetIndicator != null) targetIndicator.gameObject.SetActive(false);
            return;
        }

        Vector3 aimDirection;

        // Get aim direction from mouse (player) or external input (AI)
        if (useMouseInput)
        {
            Vector3 rawMousePos = GetMouseWorldPosition();
            aimDirection = rawMousePos - transform.position;
        }
        else
        {
            aimDirection = externalAimDirection;
        }

        CalculateAndDisplayAim(aimDirection);

        if (useMouseInput && UnityEngine.Input.GetMouseButtonDown(0))
        {
            StopAiming();
        }

        if (!useMouseInput)
        {
            StopAiming();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCam.ScreenPointToRay(UnityEngine.Input.mousePosition);

        // This creates an infinitely large invisible floor exactly at the player's height.
        // It guarantees the mouse will ALWAYS find a position, even if you aim off the map!
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float hitDistance))
        {
            return ray.GetPoint(hitDistance);
        }

        // Fallback just in case
        return transform.position;
    }

    private void CalculateAndDisplayAim(Vector3 rawAimDirection)
    {
        // CALCULATE DIRECTIONS
        Vector3 targetDirection = targetCharacter.position - transform.position;
        targetDirection.y = 0;

        // FIX 1: Use sqrMagnitude instead of '== Vector3.zero' to catch microscopic floats
        if (targetDirection.sqrMagnitude < 0.01f) targetDirection = transform.forward;
        targetDirection.Normalize();

        Vector3 rightSide = Vector3.Cross(Vector3.up, targetDirection).normalized;

        Vector3 aimDirection = rawAimDirection;
        aimDirection.y = 0;

        // FIX 2: The "Middle" Deadzone. 
        // If the aim is too close to the center, it forces the aim perfectly backwards.
        if (aimDirection.sqrMagnitude < 0.1f)
        {
            aimDirection = -targetDirection;
        }

        // LIMIT TO THE BACK HALF-CIRCLE
        if (Vector3.Dot(aimDirection, targetDirection) > 0)
        {
            if (Vector3.Dot(aimDirection, rightSide) > 0)
                aimDirection = rightSide;
            else
                aimDirection = -rightSide;
        }

        aimDirection.Normalize();

        // THE UNIFORM DISTANCE MATH
        Vector3 farPoint = transform.position + (aimDirection * 5f);
        Vector3 surfacePoint = ownerCollider.ClosestPoint(farPoint);
        Vector3 edgeTargetPos = surfacePoint + (aimDirection * castRadius);

        // Update Indicator
        if (targetIndicator != null)
        {
            targetIndicator.gameObject.SetActive(true);
            targetIndicator.position = edgeTargetPos;
            OnAimPositionUpdated?.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        // Note: The Gizmo still draws from the center for visual reference in the editor
        Gizmos.DrawWireSphere(transform.position, castRadius);
    }

    public Vector3[] GetEdgePoints(int numberOfPoints)
    {
        // Safety check to prevent dividing by zero later
        if (numberOfPoints < 2) numberOfPoints = 2;

        Vector3[] edgePoints = new Vector3[numberOfPoints];

        if (targetCharacter == null || ownerCollider == null) return edgePoints; // Return empty if missing data

        // 1. Get the direction to the target
        Vector3 targetDirection = targetCharacter.position - transform.position;
        targetDirection.y = 0;
        if (targetDirection.sqrMagnitude < 0.01f) targetDirection = transform.forward;
        targetDirection.Normalize();

        // 2. The center of our back-arc is exactly opposite the target
        Vector3 backDirection = -targetDirection;

        // 3. Sweep from Left to Right
        for (int i = 0; i < numberOfPoints; i++)
        {
            // Calculate a percentage from 0 to 1 (0 is far left, 1 is far right)
            float t = (float)i / (numberOfPoints - 1);

            // Map that percentage to an angle between -90 (Left) and 90 (Right)
            float angle = Mathf.Lerp(-90f, 90f, t);

            // Rotate the back direction by the calculated angle
            Vector3 sweepDirection = Quaternion.Euler(0, angle, 0) * backDirection;

            // 4. Run the Uniform Distance Math
            Vector3 farPoint = transform.position + (sweepDirection * 1000f);
            Vector3 surfacePoint = ownerCollider.ClosestPoint(farPoint);

            edgePoints[i] = surfacePoint + (sweepDirection * castRadius);
        }

        return edgePoints;
    }

    public void StartAiming()
    {
        isAiming = true;
    }

    public void StopAiming()
    {
        isAiming = false;

        // Hide the indicator when we stop aiming
        // if (targetIndicator != null) targetIndicator.gameObject.SetActive(false);

        OnStopAiming?.Invoke();
    }

    public void SetTargetIndicator(Transform target)
    {
        targetIndicator = target;
    }

    public void SetTargetCharacter(Transform target)
    {
        targetCharacter = target;
    }

    public void SetAimDirection(Vector3 direction)
    {
        if (!useMouseInput)
        {
            externalAimDirection = direction;
        }
    }

    public void SetUseMouseInput(bool useMouse)
    {
        useMouseInput = useMouse;
    }
}
