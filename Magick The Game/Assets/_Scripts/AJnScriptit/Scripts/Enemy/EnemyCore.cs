using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyCore : MonoBehaviour
{
    #region VARIABLES

    [Header("Health")]
    [SerializeField] private float healthAmount = 100.0f;
    [SerializeField] private float iFrameTime = 1.0f;
    [SerializeField] private GameObject projectile = null;

    private float shootIntervalTemp = 5.0f;

    #endregion

    #region VARIABLE_PROPERTIES

    //Health
    public float Health { get { return healthAmount; } }
    public float IFrames { get { return iFrameTime; } }

    #endregion

    #region UNITY_DEFAULT_METHODS

    void Start()
    {
        
    }

    void Update()
    {
        if (shootIntervalTemp > 0.0f)
        {
            shootIntervalTemp -= Time.deltaTime;
        }
        else
        {
            shootIntervalTemp = 3.0f;
            if (projectile != null)
            {
                Vector3 direction = -Vector3.Normalize(transform.position + Vector3.up * 1.0f - (GlobalVariables.player.transform.position + Vector3.up * 1.0f));
                Instantiate(projectile).GetComponent<Projectile>().Initialize(transform.position + Vector3.up * 1.0f, direction);
            }
        }
    }

    #endregion

    #region CUSTOM_METHODS

    public void Hurt()
    {
        //Do something, maybe a voiceline?
    }

    public void Dead()
    {
        Destroy(this.gameObject);
    }

    #endregion
}
