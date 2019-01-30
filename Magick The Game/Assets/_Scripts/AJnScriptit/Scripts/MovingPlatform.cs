using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float moveTime = 5.0f;
    private float moveTimeTemp = 0.0f;
    private bool moveDown = false;
    public Text debugText;

    // Start is called before the first frame update
    void Start()
    {
        moveTimeTemp = moveTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveTimeTemp > 0.0f)
        {
            transform.position += (moveDown ? Vector3.down + Vector3.left : Vector3.up + Vector3.right) * 3.0f * Time.deltaTime;
            moveTimeTemp -= Time.deltaTime;
        }
        else
        {
            moveDown = !moveDown;
            moveTimeTemp = moveTime;
        }
    }
}
