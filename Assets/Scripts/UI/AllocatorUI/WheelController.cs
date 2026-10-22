using UnityEngine;

public class DraggableWheel : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Camera cam;

    [Header("Inertia (optional)")]
    [SerializeField] private bool useInertia = true;
    [Tooltip("How quickly the spin slows down after release (higher = stops faster).")]
    [SerializeField] private float friction = 3f;

    private Vector3 axis;
    private Vector3 referenceDir;
    private float angleOffset;
    private bool wheelBeingDragged;

    private float angularVelocity;
    private float lastWheelAngle;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void OnMouseDown()
    {
        if (!TryGetPointerDirection(out Vector3 dir)) return;

        axis = transform.forward;
        referenceDir = GetReferenceDirection(axis);

        float pointerAngle = Vector3.SignedAngle(referenceDir, dir, axis);
        float wheelAngle = GetWheelAngle();

        angleOffset = wheelAngle - pointerAngle;
        lastWheelAngle = wheelAngle;
        angularVelocity = 0f;
        wheelBeingDragged = true;
    }

    private void OnMouseDrag()
    {
        if (!wheelBeingDragged) return;
        if (!TryGetPointerDirection(out Vector3 dir)) return;

        float pointerAngle = Vector3.SignedAngle(referenceDir, dir, axis);
        float targetAngle = pointerAngle + angleOffset;

        // Rotate by the difference between where the wheel is and where it should be.
        float delta = Mathf.DeltaAngle(GetWheelAngle(), targetAngle);
        transform.Rotate(axis, delta, Space.World);

        // Track velocity for inertia.
        if (Time.deltaTime > 0f)
        {
            float current = GetWheelAngle();
            angularVelocity = Mathf.DeltaAngle(lastWheelAngle, current) / Time.deltaTime;
            lastWheelAngle = current;
        }
    }

    private void OnMouseUp()
    {
        wheelBeingDragged = false;
    }

    private void Update()
    {
        if (wheelBeingDragged || !useInertia || Mathf.Abs(angularVelocity) < 0.01f) return;

        transform.Rotate(axis, angularVelocity * Time.deltaTime, Space.World);
        angularVelocity = Mathf.Lerp(angularVelocity, 0f, friction * Time.deltaTime);
    }

    /// <summary>
    /// Direction from the wheel's center to the mouse, in the wheel's rotation plane.
    /// </summary>
    private bool TryGetPointerDirection(out Vector3 dir)
    {
        dir = Vector3.zero;
        if (cam == null) return false;

        Plane plane = new Plane(transform.forward, transform.position);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!plane.Raycast(ray, out float enter)) return false;

        Vector3 hit = ray.GetPoint(enter);
        dir = hit - transform.position;
        return dir.sqrMagnitude > 0.0001f; // ignore clicks right on the center
    }

    /// <summary>
    /// The wheel's current angle, measured against the same reference direction
    /// used for the pointer angle.
    /// </summary>
    private float GetWheelAngle()
    {
        return Vector3.SignedAngle(referenceDir, transform.right, axis);
    }

    private static Vector3 GetReferenceDirection(Vector3 rotationAxis)
    {
        Vector3 reference = Vector3.ProjectOnPlane(Vector3.right, rotationAxis);
        if (reference.sqrMagnitude < 0.001f)
            reference = Vector3.ProjectOnPlane(Vector3.up, rotationAxis);
        return reference.normalized;
    }
}