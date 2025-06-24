using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Position")]
    public Transform target;
    public float followSpeed = 5f;

    [Header("Trigger Control")]
    [Range(0.1f, 0.5f)]
    public float horizontalThreshold = 0.3f;
    public float maxHorizontalOffset = 2f;

    [Header("Border Control")]
    public float leftBoundary = -10f;
    public float rightBoundary = 10f;

    private Camera mainCamera;
    private Vector3 initialOffset;
    private bool isFollowingX = false;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        initialOffset = this.transform.position - target.position;
        initialOffset.z = -10f;
        if (leftBoundary >= rightBoundary)
        {
            float temp = leftBoundary;
            leftBoundary = rightBoundary;
            rightBoundary = temp;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 targetScreenPos = mainCamera.WorldToViewportPoint(target.position);
        Vector3 newPosition = this.transform.position;
        float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float cameraHalfHeight = mainCamera.orthographicSize;
        float effectiveLeftBound = leftBoundary + cameraHalfWidth;
        float effectiveRightBound = rightBoundary - cameraHalfWidth;
        if (targetScreenPos.x < horizontalThreshold ||
            targetScreenPos.x > 1f - horizontalThreshold)
        {
            isFollowingX = true;
            float targetX = target.position.x + initialOffset.x;
            float exceedAmount = 0f;
            if (targetScreenPos.x < horizontalThreshold)
            {
                exceedAmount = (horizontalThreshold - targetScreenPos.x) / horizontalThreshold;
                targetX -= maxHorizontalOffset * exceedAmount;
            }
            else
            {
                exceedAmount = (targetScreenPos.x - (1f - horizontalThreshold)) / horizontalThreshold;
                targetX += maxHorizontalOffset * exceedAmount;
            }
            targetX = Mathf.Clamp(targetX, effectiveLeftBound, effectiveRightBound);
            newPosition.x = Mathf.Lerp(this.transform.position.x, targetX, followSpeed * Time.deltaTime);
        }
        else if (isFollowingX)
        {
            float centerX = target.position.x + initialOffset.x;
            centerX = Mathf.Clamp(centerX, effectiveLeftBound, effectiveRightBound);
            newPosition.x = Mathf.Lerp(this.transform.position.x, centerX, followSpeed * Time.deltaTime);
            if (Mathf.Abs(newPosition.x - centerX) < 0.05f)
            {
                newPosition.x = centerX;
                isFollowingX = false;
            }
        }
        newPosition.x = Mathf.Clamp(newPosition.x, effectiveLeftBound, effectiveRightBound);
        this.transform.position = newPosition;
    }
}