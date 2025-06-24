using UnityEngine;
using System.Linq;

public class FireController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public Vector3 position;
    public float angle = 0f;
    public bool isPlayerFire;

    private bool isStop = false;
    private Animator animator;

    void Start()
    {
        this.transform.position = position;
        this.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        if(!isStop) this.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayerFire)
        {
            if (collision.gameObject.tag == "Comet")
            {
                Destroy(gameObject);
                CometController cometHealth = collision.GetComponent<CometController>();
                if (cometHealth != null) cometHealth.TakeDamage();
            } else if (collision.gameObject.tag == "Enemy")
            {
                Destroy(gameObject);
                collision.gameObject.tag = "IgnoreFire";
                AudioSource explosionSource = collision.gameObject.GetComponent<AudioSource>();
                explosionSource.Play();
                Animator enemyAnimator = collision.gameObject.GetComponent<Animator>();
                if (enemyAnimator != null)
                {
                    enemyAnimator.SetBool("Explosion", true);
                    AnimationClip explosionClip = enemyAnimator.runtimeAnimatorController.animationClips
                        .FirstOrDefault(clip => clip.name == "Tank_Explosion");
                    if (explosionClip != null) Destroy(collision.gameObject, explosionClip.length);
                    foreach (Transform child in collision.transform) Destroy(child.gameObject);
                }
                ScoreDisplay scoreDisplay = FindFirstObjectByType<ScoreDisplay>();
                if (scoreDisplay != null) scoreDisplay.changeNowScore(1550f);
            } else if (collision.gameObject.tag == "Boss")
            {
                isStop = true;
                this.gameObject.tag = "IgnoreFire";
                this.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                animator.SetBool("AttackBoss", true);
                AnimationClip attackBossClip = animator.runtimeAnimatorController.animationClips
                    .FirstOrDefault(clip => clip.name == "PlayerFire_AttackBoss");
                if (attackBossClip != null) Destroy(gameObject, attackBossClip.length);
                BossController cometHealth = collision.GetComponent<BossController>();
                if (cometHealth != null) cometHealth.TakeDamage();
            }
        }
        else {
            if (collision.gameObject.tag == "Player")
            {
                Destroy(gameObject);
                Transform playerTransform = collision.gameObject.transform.parent;
                PlayerController playerController = playerTransform.transform.gameObject.GetComponent<PlayerController>();
                if (playerController != null) playerController.TakeDamage();
            }
        }
    }
}