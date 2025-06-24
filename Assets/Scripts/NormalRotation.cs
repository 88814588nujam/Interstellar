using UnityEngine;

public class NormalRotation : MonoBehaviour
{
    public float rotationSpeed = 30f;

    void Update()
    {
        this.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}