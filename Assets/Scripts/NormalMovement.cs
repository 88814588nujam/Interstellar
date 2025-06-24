using UnityEngine;

public class NormalMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float destroyY = -6f;

    void Update()
    {
        this.transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
        if (this.transform.position.y < destroyY) Destroy(gameObject);
    }
}