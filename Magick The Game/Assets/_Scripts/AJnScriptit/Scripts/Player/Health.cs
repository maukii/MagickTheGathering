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

    #region UNITY_DEFAULT_METHODS

    void Start()
    {
        if (GetComponent<PlayerCore>() != null)
        {
            isPlayer = true;
            health = GetComponent<PlayerCore>().Health;
            iFrameTime = GetComponent<PlayerCore>().IFrames;
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

    public bool CheckIfDead()
    {
        return isDead;
    }

    void CheckHealth()
    {
        if (health <= 0.0f)
        {
            Dead();
        }
    }

    public void Hit(float amount)
    {
        if (!isDead)
        {
            if (iFrameTimeTemp <= 0.0f)
            {
                iFrameTimeTemp = iFrameTime;
                health -= amount;

                if (isPlayer)
                {
                    GetComponent<PlayerCore>().Hurt();
                }
                
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
            if (isPlayer)
            {
                GetComponent<PlayerCore>().Hurt();
            }
            Dead();
        }
    }

    void Dead()
    {
        health = 0.0f;
        isDead = true;

        if (isPlayer)
        {
            GetComponent<PlayerCore>().Dead();
        }
    }

    #endregion
}
