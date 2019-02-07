using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private LayerMask physicsLayerMask;
    [SerializeField] private GameObject explosion = null;

    private bool hitSomething = false;
    private Vector3 direction = Vector3.zero;

    void Start()
    {
        if (direction == Vector3.zero)
        {
            Debug.Log("Projectile instantiated with no direction! De-spawning...");
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(
            transform.position,
            direction,
            out hit,
            speed * Time.deltaTime,
            physicsLayerMask
            ))
        {
            transform.position = hit.point;
            hitSomething = true;
            Destroy(this.gameObject);
        }
        else
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void OnDestroy()
    {
        if (explosion != null && hitSomething)
        {
            Instantiate(explosion, this.transform.position, Quaternion.identity, null);
        }
    }

    public void Initialize(Vector3 spawnPosition, Vector3 spawnDirection)
    {
        transform.position = spawnPosition;
        direction = spawnDirection;
    }
}
