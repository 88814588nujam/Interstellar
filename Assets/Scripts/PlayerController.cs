using UnityEngine;
using System.Linq;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Player Setting")]
    public float moveSpeed = 10f;
    public float playerHealth = 3f;

    [Header("Plane Control")]
    public Animator planeAnimator;

    [Header("Flame Control")]
    public Transform flameTransform;

    [Header("Fire Control")]
    public GameObject firePrefab;
    public float fireRate = 0.2f;
    public AudioSource fireSource;
    public AudioSource fireMultipleSource;
    private float nextFireTime = 0f;

    [Header("Border Control")]
    public float leftBoundary = -10f;
    public float rightBoundary = 10f;
    public float topBoundary = -5f;
    public float bottomBoundary = 5f;

    [Header("Attacked Control")]
    public float invincibleDuration = 2f;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.2f;
    public float blinkInterval = 0.1f;
    public AudioSource failedSource;

    private float holdTime = 0f;
    private bool isHolding = false;

    private Vector3 touchOffset;
    private bool gameEnd = false;
    private bool isTouching = false;
    Transform childPlane;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 originalPosition;
    private bool invisible = false;

    private void Start()
    {
        childPlane = this.transform.Find("Plane");
        spriteRenderer = childPlane.gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!planeAnimator || !planeAnimator.gameObject) return;
        if (!gameEnd) { 
            Vector3 nowPos = this.transform.position;
            nowPos.x = Mathf.Clamp(nowPos.x, leftBoundary, rightBoundary);
            nowPos.y = Mathf.Clamp(nowPos.y, topBoundary, bottomBoundary);
            this.transform.position = nowPos;
            //#if UNITY_EDITOR || UNITY_STANDALONE
                HandleKeyboardInput();
                HandleMouseFollow();
            //#elif UNITY_ANDROID || UNITY_IOS
                HandleTouchInput();
            //#endif
            //Keyboard
            if ((Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.J)) && Time.time >= nextFireTime)
            {
                CreateFire(nowPos, 0f, 0.78f, 0f);
                fireSource.Play();
                nextFireTime = Time.time + fireRate;
            }
            if ((Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.K)) && Time.time >= nextFireTime)
            {
                CreateFire(nowPos, 0f, 0.78f, 0f);
                CreateFire(nowPos, -0.2f, 0.78f, 20f);
                CreateFire(nowPos, 0.2f, 0.78f, -20f);
                fireMultipleSource.Play();
                nextFireTime = Time.time + fireRate;
            }
            if ((Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.L)) && Time.time >= nextFireTime)
            {
                CreateFire(nowPos, 0f, 0.78f, 0f);
                CreateFire(nowPos, -0.2f, 0.78f, 20f);
                CreateFire(nowPos, 0.2f, 0.78f, -20f);
                CreateFire(nowPos, -0.2f, -0.78f, 160f);
                CreateFire(nowPos, 0.2f, -0.78f, -160f);
                fireMultipleSource.Play();
                nextFireTime = Time.time + fireRate;
            }
            //Mouse
            if (Input.GetMouseButtonDown(0))
            {
                isHolding = true;
                holdTime = 0f;
            }
            if (isHolding && Input.GetMouseButton(0)) holdTime += Time.deltaTime;
            if (Input.GetMouseButtonUp(0)) isHolding = false;
            if (!isHolding) return;
            if ((holdTime >= 0f && holdTime < 3f) && Time.time >= nextFireTime) {
                CreateFire(nowPos, 0f, 0.78f, 0f);
                fireSource.Play();
                nextFireTime = Time.time + fireRate;
            }
            if ((holdTime >= 3f && holdTime < 6f) && Time.time >= nextFireTime)
            {
                CreateFire(nowPos, 0f, 0.78f, 0f);
                CreateFire(nowPos, -0.2f, 0.78f, 20f);
                CreateFire(nowPos, 0.2f, 0.78f, -20f);
                fireMultipleSource.Play();
                nextFireTime = Time.time + fireRate;
            }
            if ((holdTime >= 6f) && Time.time >= nextFireTime)
            {
                CreateFire(nowPos, 0f, 0.78f, 0f);
                CreateFire(nowPos, -0.2f, 0.78f, 20f);
                CreateFire(nowPos, 0.2f, 0.78f, -20f);
                CreateFire(nowPos, -0.2f, -0.78f, 160f);
                CreateFire(nowPos, 0.2f, -0.78f, -160f);
                fireMultipleSource.Play();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void CreateFire(Vector3 pos, float devX, float devY, float ang) {
        GameObject newFirePrefab = Instantiate(firePrefab);
        FireController fireController = newFirePrefab.GetComponent<FireController>();
        fireController.position = new Vector3(pos.x + devX, pos.y + devY, pos.z);
        fireController.angle = ang;
    }

    void HandleKeyboardInput()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(moveX, moveY, 0f);
        this.transform.position += move * moveSpeed * Time.deltaTime;
        planeAnimator.SetFloat("Horizontal", move.x);
        planeAnimator.SetFloat("Vertical", move.y);
        planeAnimator.SetFloat("Speed", move.magnitude);
        if(flameTransform != null) { 
            if (move.magnitude > 0.01f) flameTransform.localScale = new Vector3(1.8f, 2.2f, 1f);
            else flameTransform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    void HandleMouseFollow()
    {
        if (Input.GetMouseButton(1))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            this.transform.position = Vector3.Lerp(this.transform.position, mousePosition, moveSpeed / 2 * Time.deltaTime);
            planeAnimator.SetFloat("Horizontal", mousePosition.x);
            planeAnimator.SetFloat("Vertical", mousePosition.y);
            planeAnimator.SetFloat("Speed", mousePosition.magnitude);
            if(flameTransform != null) { 
                if (mousePosition.magnitude > 0.01f) flameTransform.localScale = new Vector3(1.8f, 2.2f, 1f);
                else flameTransform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0f;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Vector3 worldTouch = Camera.main.ScreenToWorldPoint(touch.position);
                    worldTouch.z = 0f;
                    if (Vector2.Distance(worldTouch, this.transform.position) < 1f)
                    {
                        isTouching = true;
                        touchOffset = this.transform.position - worldTouch;
                    }
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (isTouching)
                        this.transform.position = touchPosition + touchOffset;
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isTouching = false;
                    break;
            }
        }
    }

    private IEnumerator MoveUpAndDestroy()
    {
        yield return new WaitForSeconds(5.5f);
        while (this.transform.position.y < 5.5f)
        {
            this.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
        yield return new WaitForSeconds(2f);
    }

    public void goToGameEnd() {
        gameEnd = true;
        this.transform.Find("Plane").gameObject.tag = "IgnoreFire";
        StartCoroutine(MoveUpAndDestroy());
    }

    public void TakeDamage()
    {
        if (invisible) return;
        invisible = true;
        originalColor = spriteRenderer.color;
        originalPosition = childPlane.gameObject.transform.localPosition;
        playerHealth--;
        failedSource.Play();
        if (playerHealth > 0)
        {
            StartCoroutine(HitEffect());
            if (playerHealth == 2)
            {
                Transform childA = this.transform.Find("Friend1");
                Destroy(childA.gameObject);
            }
            else if (playerHealth == 1)
            {
                Transform childB = this.transform.Find("Friend2");
                Destroy(childB.gameObject);
            }
        }
        else
        {
            gameEnd = true;
            planeAnimator.SetBool("Explosion", true);
            AnimationClip explosionClip = planeAnimator.runtimeAnimatorController.animationClips
                .FirstOrDefault(clip => clip.name == "Plane_Explosion");
            if (explosionClip != null) Destroy(gameObject, explosionClip.length);
            foreach (Transform child in childPlane.transform) Destroy(child.gameObject);
            ScoreDisplay scoreDisplay = FindFirstObjectByType<ScoreDisplay>();
            if (scoreDisplay != null) scoreDisplay.hiddenScore();
            GotoEnd gotoend = FindFirstObjectByType<GotoEnd>();
            if (gotoend != null) gotoend.NextScene(false);
        }
    }
    

    private IEnumerator HitEffect()
    {
        spriteRenderer.color = Color.red;
        float shakeTime = 0f;
        while (shakeTime < shakeDuration)
        {
            shakeTime += Time.deltaTime;
            childPlane.transform.localPosition = originalPosition + Random.insideUnitSphere * shakeIntensity;
            yield return null;
        }
        childPlane.transform.localPosition = originalPosition;
        float invincibleTime = 0f;
        bool visible = true;
        while (invincibleTime < invincibleDuration)
        {
            invincibleTime += Time.deltaTime;
            if (invincibleTime % blinkInterval < Time.deltaTime)
            {
                visible = !visible;
                spriteRenderer.enabled = visible;
            }
            yield return null;
        }
        spriteRenderer.enabled = true;
        spriteRenderer.color = originalColor;
        invisible = false;
    }
}