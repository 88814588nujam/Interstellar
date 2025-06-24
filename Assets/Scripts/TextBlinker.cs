using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextBlinker : MonoBehaviour
{
    public Text textComponent;
    public float blinkInterval = 0.5f;
    private void Start()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<Text>();
        }
        StartCoroutine(BlinkText());
    }

    IEnumerator BlinkText()
    {
        while (true)
        {
            textComponent.color = new Color(textComponent.color.r,
                                          textComponent.color.g,
                                          textComponent.color.b,
                                          0f);
            yield return new WaitForSeconds(blinkInterval);
            textComponent.color = new Color(textComponent.color.r,
                                          textComponent.color.g,
                                          textComponent.color.b,
                                          1f);
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}