using UnityEngine;

public class Health : MonoBehaviour
{
    #region VARIABLES

    public bool bIsDead { get; private set; }   = false;

    [SerializeField] private float health       = 100.0f;
    [SerializeField] private float iFrameTime   = 0.5f;

    private bool bIsPlayer                       = false;
    private float iftTimer                      = 0.0f;

    #endregion

    #region UNITY_DEFAULT_METHODS

    void Start()
    {
        if (GetComponent<PlayerCore>() != null)
        {
            bIsPlayer = true;
        }
    }

    void Update()
    {
        if (iftTimer > 0.0f)
        {
            iftTimer -= Time.deltaTime;
        }
    }

    #endregion

    #region CUSTOM_METHODS
    
    public void Hurt(float amount)
    {
        if (!bIsDead)
        {
            if (iftTimer <= 0.0f)
            {
                iftTimer = iFrameTime;
                health -= amount;

                if (bIsPlayer)
                {
                    GetComponent<PlayerCore>().OnHurt();
                }
                else
                {
                    GetComponent<EnemyCore>().OnHurt();
                }

                if (health <= 0.0f)
                {
                    Kill();
                }
            }
        }
    }

    public void Heal(float amount)
    {
        if (!bIsDead)
        {
            health += Mathf.Abs(amount);
        }
    }

    public void Kill()
    {
        health = 0.0f;
        bIsDead = true;

        if (bIsPlayer)
        {
            GetComponent<PlayerCore>().OnDeath();
        }
        else
        {
            GetComponent<EnemyCore>().OnDeath();
        }
    }

    #endregion
}
