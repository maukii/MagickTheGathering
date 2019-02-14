using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyCore : MonoBehaviour
{
    #region VARIABLES
    
    [SerializeField] private GameObject projectile = null;

    private float shootIntervalTimer = 5.0f;

    #endregion

    #region UNITY_DEFAULT_METHODS

    void Update()
    {
        if (shootIntervalTimer > 0.0f)
        {
            shootIntervalTimer -= Time.deltaTime;
        }
        else
        {
            shootIntervalTimer = 3.0f;
            if (projectile != null)
            {
                Vector3 direction = -Vector3.Normalize(transform.position + Vector3.up * 1.0f - (GlobalVariables.player.transform.position + Vector3.up * 1.0f));
                Instantiate(projectile).GetComponent<Projectile>().Initialize(transform.position + Vector3.up * 1.0f, direction);
            }
        }
    }

    #endregion

    #region CUSTOM_METHODS

    public void OnHurt()
    {
        //Do something, maybe a voiceline?
    }

    public void OnDeath()
    {
        Destroy(this.gameObject);
    }

    #endregion
}
