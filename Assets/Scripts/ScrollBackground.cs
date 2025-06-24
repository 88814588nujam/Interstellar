using UnityEngine;

public class ScrollBackground : MonoBehaviour
{
    public Transform[] backgrounds;
    public float scrollSpeed = 1f;
    public float height = 10f;

    void Update()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].position += Vector3.down * scrollSpeed * Time.deltaTime;
            if (backgrounds[i].position.y <= -height)
            {
                Vector3 newPos = backgrounds[i].position;
                newPos.y += height * 2;
                backgrounds[i].position = newPos;
            }
        }
    }
}