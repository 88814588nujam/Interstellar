using UnityEngine;

public class SpiralBullet : MonoBehaviour
{
    private bool isLeftSide;
    private float phaseOffset;
    private float spiralSpeed;
    private float downwardSpeed;
    private float timeAlive;

    public void Initialize(bool isLeft, float offset, float speed, float downSpeed)
    {
        isLeftSide = isLeft;
        phaseOffset = offset;
        spiralSpeed = speed;
        downwardSpeed = downSpeed;
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;
        float spiralDirection = isLeftSide ? 1f : -1f;
        float currentAngle = (timeAlive + phaseOffset) * spiralSpeed * spiralDirection;
        Vector2 moveDir = new Vector2(
            Mathf.Sin(currentAngle * Mathf.Deg2Rad),
            -1f
        ).normalized;
        GetComponent<Rigidbody2D>().linearVelocity = moveDir * downwardSpeed;
        float lookAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(lookAngle, Vector3.forward);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}