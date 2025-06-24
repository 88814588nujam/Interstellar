using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    [Header("Boss Control")]
    public float moveSpeed = 2f;
    public float delayAttacks = 2f;
    public float bossHealth = 20f;
    public Color oldColor = Color.white;
    public Color hitColor = Color.white;

    [Header("Sector Setting")]
    public GameObject sectorFirePrefab;
    public int sectorBullet = 15;
    public float sectorFireSpeed = 3f;
    public float sectorAngle = 60f;
    public float sectorDuration = 3f;
    public float sectorFireRate = 0.5f;

    [Header("Spiral Setting")]
    public GameObject spiralFirePrefab;
    public int spiralBullet = 12;
    public float spiralFireSpeed = 3f;
    public float spiralSpeed = 180f;
    public float spiralFireRate = 0.2f;

    [Header("Charge Setting")]
    public GameObject chargePrefab;
    public float fireDuration = 1f;
    public float chargeSpeed = 15f;
    public float sideOffsetX = 1f;
    public float sideOffsetY = 1f;
    public float chargeDelay = 0.5f;

    [Header("Spark Setting")]
    public GameObject sparkPrefab;
    public GameObject[] sparks;

    [Header("Fall Setting")]
    public float fallSpeed = 1f;
    public float shakeIntensity = 0.5f;
    public float shakeFrequency = 10f;

    private float addScore;
    private Vector3 originalPosition;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        addScore = bossHealth * 1550f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(BossBehavior());
    }

    IEnumerator BossBehavior()
    {
        yield return MoveToPosition(new Vector2(0, 2.5f));
        int attackTypeControl = 0;
        while (bossHealth > 0f)
        {
            int attackType = attackTypeControl < 2 ? 0 : (attackTypeControl < 4 ? 1 : 2);
            switch (attackType)
            {
                case 0:
                    yield return StartCoroutine(SectorAttack());
                    break;
                case 1:
                    yield return StartCoroutine(SpiralAttack());
                    break;
                case 2:
                    yield return StartCoroutine(ChargeAttack());
                    break;
            }
            attackTypeControl = attackTypeControl + 1 == 5 ? 0 : attackTypeControl + 1;
            Vector2 randomPos = new Vector2(
                Random.Range(-6.5f, 6.5f),
                Random.Range(1f, 3f)
            );
            yield return MoveToPosition(randomPos);
            yield return new WaitForSeconds(delayAttacks);
        }
    }

    IEnumerator MoveToPosition(Vector2 targetPos)
    {
        while (Vector2.Distance(this.transform.position, targetPos) > 0.1f)
        {
            this.transform.position = Vector2.MoveTowards(
                this.transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    IEnumerator SectorAttack()
    {
        int waves = Mathf.FloorToInt(sectorDuration / sectorFireRate);
        Vector2 baseDirection = Vector2.down;
        for (int wave = 0; wave < waves; wave++)
        {
            if (bossHealth > 0f)
            {
                float angleStep = sectorAngle / (sectorBullet - 1);
                float startAngle = -sectorAngle / 2f;
                for (int i = 0; i < sectorBullet; i++)
                {
                    float currentAngle = startAngle + angleStep * i;
                    Vector2 direction = Quaternion.Euler(0, 0, currentAngle) * baseDirection;
                    GameObject bullet = Instantiate(
                        sectorFirePrefab,
                        this.transform.position + new Vector3(0f, -1.5f, 0f),
                        Quaternion.identity
                    );
                    bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * sectorBullet;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
                yield return new WaitForSeconds(sectorFireRate);
            } else break;
        }
    }

    IEnumerator SpiralAttack()
    {
        for (int i = 0; i < spiralBullet / 2; i++)
        {
            if (bossHealth > 0f)
            {
                LaunchSpiralBullet(true, i, spiralSpeed, spiralFireSpeed);
                yield return new WaitForSeconds(spiralFireRate);
                LaunchSpiralBullet(false, i, spiralSpeed, spiralFireSpeed);
                yield return new WaitForSeconds(spiralFireRate);
            }
            else break;
        }
    }

    private void LaunchSpiralBullet(bool isLeftSide, int bulletIndex, float spiralSpeed, float downwardSpeed)
    {
        Vector2 spawnPos = this.transform.position + new Vector3(isLeftSide ? -1f : 1f, 0, 0);
        GameObject bullet = Instantiate(spiralFirePrefab, spawnPos, Quaternion.identity);
        SpiralBullet spiralComp = bullet.AddComponent<SpiralBullet>();
        spiralComp.Initialize(
            isLeftSide,
            bulletIndex * 0.3f,
            spiralSpeed,
            downwardSpeed
        );
    }

    IEnumerator ChargeAttack()
    {
        SpawnFireAnimations();
        yield return new WaitForSeconds(fireDuration);
        if (bossHealth > 0f)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector2 chargeDirection = (player.transform.position - this.transform.position).normalized;
                GetComponent<Rigidbody2D>().linearVelocity = chargeDirection * chargeSpeed;
                yield return new WaitForSeconds(chargeDelay);
                GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            }
        }
    }

    private void SpawnFireAnimations()
    {
        Vector2 leftPos = (Vector2) this.transform.position + new Vector2(-sideOffsetX, sideOffsetY);
        GameObject leftFire = Instantiate(chargePrefab, leftPos, Quaternion.identity);
        Animator leftFireAnimator = leftFire.GetComponent<Animator>();
        leftFireAnimator.SetBool("Charge", true);
        Vector2 rightPos = (Vector2) this.transform.position + new Vector2(sideOffsetX, sideOffsetY);
        GameObject rightFire = Instantiate(chargePrefab, rightPos, Quaternion.identity);
        Animator rightFireAnimator = rightFire.GetComponent<Animator>();
        rightFireAnimator.SetBool("Charge", true);
        Destroy(leftFire, fireDuration + 0.1f);
        Destroy(rightFire, fireDuration + 0.1f);
    }

    IEnumerator HitEffect()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = oldColor;
    }

    public void TakeDamage()
    {
        bossHealth--;
        StartCoroutine(HitEffect());
        if (bossHealth <= 0f) {
            ScoreDisplay scoreDisplay = FindFirstObjectByType<ScoreDisplay>();
            if (scoreDisplay != null) scoreDisplay.changeNowScore(addScore);
            this.tag = "IgnoreFire";
            AudioSource explosionSource = this.transform.gameObject.GetComponent<AudioSource>();
            explosionSource.Play();
            EnemySpawner enemySpawner = FindFirstObjectByType<EnemySpawner>();
            if (enemySpawner != null) enemySpawner.chageMusic();
            PlayerController playerController = FindFirstObjectByType<PlayerController>();
            if (playerController != null)
            {
                playerController.goToGameEnd();
                playerController.planeAnimator.SetFloat("Speed", 0f);

            }
            StartDeathSequence();
        }
    }

    public void StartDeathSequence()
    {
        originalPosition = this.transform.position;
        for (int i = 0; i < 9; i++)
        {
            if (sparks.Length > 0)
            {
                int posIndex = Random.Range(0, sparks.Length);
                GameObject spark = Instantiate(sparkPrefab,
                                             new Vector3(0f, 0f, 0f),
                                             Quaternion.identity);
                spark.transform.SetParent(this.transform);
                spark.transform.localPosition = sparks[posIndex].transform.localPosition;
            }
        }
        StartCoroutine(DeathAnimation());
    }

    IEnumerator DeathAnimation()
    {
        float fallTime = 0f;
        while (this.transform.position.y > -7f)
        {
            fallTime += Time.deltaTime;
            float currentShake = shakeIntensity * (1 - fallTime / 5f);
            if (currentShake > 0)
            {
                float shakeOffset = Mathf.Sin(Time.time * shakeFrequency) * currentShake;
                this.transform.position = originalPosition + new Vector3(shakeOffset, 0, 0);
            }
            float currentFallSpeed = fallSpeed * (1 + fallTime / 2f);
            originalPosition += Vector3.down * currentFallSpeed * Time.deltaTime;
            yield return null;
        }
        ScoreDisplay scoreDisplay = FindFirstObjectByType<ScoreDisplay>();
        if (scoreDisplay != null) scoreDisplay.hiddenScore();
        Camera mainCamera = Camera.main;
        Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);
        Vector3 worldCenter = mainCamera.ViewportToWorldPoint(screenCenter);
        worldCenter.z = 0;
        GotoEnd gotoend = FindFirstObjectByType<GotoEnd>();
        if (gotoend != null) gotoend.NextScene(true);
        Destroy(gameObject);
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