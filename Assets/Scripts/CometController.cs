using UnityEngine;
using System.Collections;

public class CometController : MonoBehaviour
{
    [Header("Comet Control")]
    public float cometHealth = 3f;
    public Color oldColor = Color.white;
    public Color hitColor = Color.white;

    [Header("Explosion Setting")]
    public AudioSource attackedSound;
    public AudioSource destroySound;
    public GameObject fragmentPrefab;
    public int fragmentCount = 5;
    public float fragmentForce = 5f;

    private float addScore;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        addScore = cometHealth * 1550f;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    IEnumerator HitEffect()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = oldColor;
    }

    IEnumerator DestroyComet()
    {
        GetComponent<Collider2D>().enabled = false;
        spriteRenderer.enabled = false;
        if (cometHealth > 0f) attackedSound.Play();
        else destroySound.Play();
        if (fragmentPrefab != null)
        {
            for (int i = 0; i < fragmentCount; i++)
            {
                GameObject piece = Instantiate(fragmentPrefab, this.transform.position, Quaternion.identity);
                Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    float randomAngle = Random.Range(0f, Mathf.PI * 2f);
                    Vector2 randomDir = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
                    float forceVariation = Random.Range(0.8f, 1.2f);
                    rb.AddForce(randomDir * fragmentForce * forceVariation, ForceMode2D.Impulse);
                }
            }
        }
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public void TakeDamage()
    {
        cometHealth--;
        StartCoroutine(HitEffect());
        if (cometHealth <= 0f)
        {
            ScoreDisplay scoreDisplay = FindFirstObjectByType<ScoreDisplay>();
            if (scoreDisplay != null) scoreDisplay.changeNowScore(addScore);
            this.transform.tag = "IgnoreFire";
            StartCoroutine(DestroyComet());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController playerController = FindFirstObjectByType<PlayerController>();
            if (playerController != null) playerController.TakeDamage();
        }
    }
}