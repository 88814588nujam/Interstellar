using UnityEngine;
using System.Linq;

public class SuicideController : MonoBehaviour
{
    [Header("Target Control")]
    public Transform playerTarget;

    [Header("Suicide Setting")]
    public float moveSpeed = 3f;
    public float attackSpeed = 8f;
    public float holdPositionTime = 1f;

    private Vector3 targetPosition;
    private Vector3 attackDirection;
    private bool isEntering = true;
    private bool isAttacking = false;
    private bool isDead = false;
    private float holdTimer = 0f;
    private Animator animator;
    private Camera mainCamera;

    void Start()
    {
        targetPosition = new Vector3(this.transform.position.x, 3f, 0);
        animator = this.GetComponent<Animator>();
        mainCamera = Camera.main;
        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTarget = playerObj.transform;
        }
    }

    void Update()
    {
        if (isEntering)
        {
            this.transform.position = Vector3.MoveTowards(
                this.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            if (Vector3.Distance(this.transform.position, targetPosition) < 0.1f)
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdPositionTime && playerTarget != null)
                {
                    isEntering = false;
                    isAttacking = true;
                    attackDirection = (playerTarget.position - this.transform.position).normalized;
                }
            }
        }
        else if (isAttacking && !isDead)
        {
            this.transform.position += attackDirection * attackSpeed * Time.deltaTime;
            Vector3 nowPos = this.transform.position;
            if (nowPos.y > GetScreenTopBorder() || nowPos.y < GetScreenBottomBorder() ||
                    nowPos.x > GetScreenRightBorder() || nowPos.x < GetScreenLeftBorder())
            {
                Destroy(gameObject);
            }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && this.tag != "IgnoreFire")
        {
            isDead = true;
            this.tag = "IgnoreFire";
            AudioSource explosionSource = this.transform.gameObject.GetComponent<AudioSource>();
            explosionSource.Play();
            animator.SetBool("Explosion", true);
            AnimationClip explosionClip = animator.runtimeAnimatorController.animationClips
                .FirstOrDefault(clip => clip.name == "Suicide_Explosion");
            if (explosionClip != null) Destroy(gameObject, explosionClip.length);
            Transform playerTransform = collision.gameObject.transform.parent;
            PlayerController playerController = playerTransform.transform.gameObject.GetComponent<PlayerController>();
            if (playerController != null) playerController.TakeDamage();
        }
    }
}