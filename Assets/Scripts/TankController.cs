using UnityEngine;

public class TankController : MonoBehaviour
{
    [Header("Target Control")]
    public Transform playerTarget;

    [Header("Fire Control")]
    public GameObject firePrefab;
    public float fireRate = 2f;

    private float nextFireTime = 0f;

    void Start()
    {
        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTarget = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTarget && playerTarget.gameObject)
        {
            Vector2 direction = playerTarget.position - this.transform.position;
            float ang = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float angTmp = ang + 90f;
            this.transform.rotation = Quaternion.Euler(0, 0, angTmp);
            Vector3 nowPos = this.transform.position;
            if (Time.time >= nextFireTime)
            {
                CreateFire(nowPos, 0f, 0f, angTmp + 180f);
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    private void CreateFire(Vector3 pos, float devX, float devY, float ang)
    {
        GameObject newFirePrefab = Instantiate(firePrefab);
        FireController fireController = newFirePrefab.GetComponent<FireController>();
        fireController.position = new Vector3(pos.x + devX, pos.y + devY, pos.z);
        fireController.angle = ang;
    }
}