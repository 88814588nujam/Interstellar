using UnityEngine;

public class DeatroyObject : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 nowPos = this.transform.position;
        if (nowPos.y > GetScreenTopBorder() || nowPos.y < GetScreenBottomBorder() ||
                nowPos.x > GetScreenRightBorder() || nowPos.x < GetScreenLeftBorder())
        {
            Destroy(gameObject);
        }
    }

    private float GetScreenLeftBorder()
    {
        return mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
    }

    private float GetScreenRightBorder()
    {
        return mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
    }

    private float GetScreenTopBorder()
    {
        return mainCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
    }

    private float GetScreenBottomBorder()
    {
        return mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
    }
}
