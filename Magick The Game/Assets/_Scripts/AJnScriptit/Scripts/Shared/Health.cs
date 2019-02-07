using UnityEngine;

public class Health : MonoBehaviour
{
    #region VARIABLES

    private bool isPlayer           = false;
    private bool isDead             = false;
    private float health            = 100.0f;
    private float iFrameTime        = 0.5f;
    private float iFrameTimeTemp    = 0.0f;

    #endregion

    #region VARIABLE_PROPERTIES

    public bool IsDead { get { return isDead; } }

    #endregion

    #region UNITY_DEFAULT_METHODS

    void Start()
    {
        if (GetComponent<PlayerCore>() != null)
        {
            isPlayer = true;
            health = GetComponent<PlayerCore>().Health;
            iFrameTime = GetComponent<PlayerCore>().IFrames;
        }
        else if (GetComponent<EnemyCore>() != null)
        {
            health = GetComponent<EnemyCore>().Health;
            iFrameTime = GetComponent<EnemyCore>().IFrames;
        }
    }

    void Update()
    {
        if (iFrameTimeTemp > 0.0f)
        {
            iFrameTimeTemp -= Time.deltaTime;
        }
    }

    #endregion

    #region CUSTOM_METHODS

    void CheckHealth()
    {
        if (health <= 0.0f)
        {
            Dead();
        }
    }

    public void Hurt(float amount)
    {
        if (!isDead)
        {
            if (iFrameTimeTemp <= 0.0f)
            {
                iFrameTimeTemp = iFrameTime;
                health -= amount;

                if (isPlayer)   { GetComponent<PlayerCore>().Hurt(); }
                else            { GetComponent<EnemyCore>().Hurt(); }

                CheckHealth();
            }
        }
    }

    public void Heal(float amount)
    {
        if (!isDead)
        {
            health += amount;
            CheckHealth();
        }
    }

    public void Slay()
    {
        if (!isDead)
        {
            if (isPlayer)   { GetComponent<PlayerCore>().Hurt(); }
            else            { GetComponent<EnemyCore>().Hurt(); }
            Dead();
        }
    }

    void Dead()
    {
        health = 0.0f;
        isDead = true;

        if (isPlayer)   { GetComponent<PlayerCore>().Dead(); }
        else            { GetComponent<EnemyCore>().Dead(); }
    }

    #endregion
}
