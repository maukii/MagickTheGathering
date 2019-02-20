using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    #region VARIABLES
    
    [SerializeField] private Transform gun = null;
    [SerializeField] private float sightDistance = 30.0f;
    [SerializeField] private float sightRadius = 45.0f;
    [SerializeField] private float spiderSenseRadius = 5.0f;

    public bool bCanSeePlayer { get; private set; } = false;

    private Vector3 lookDirection = Vector3.forward;
    private Vector3 offset = Vector3.zero;
    private Vector3 playerLocation = Vector3.zero;
    private Vector3 playerLastKnownLocation = Vector3.zero;

    #endregion

    #region MAGIC_NUMBERS

    /*--------------------------------------------------------------*/

    //These two variables describe the vision "pyramid".
    //Do not modify!

    private Vector3[] visionVertices = new Vector3[]
    {
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(1.0f, -1.0f, 1.0f),
        new Vector3(-1.0f, 1.0f, 1.0f),
        new Vector3(-1.0f, -1.0f, 1.0f)
    };

    private int[] visionTriangles = new int[]
    {
        0, 1, 2,
        0, 3, 1,
        0, 4, 3,
        0, 2, 4
    };

    /*--------------------------------------------------------------*/

    #endregion
    
    #region UNITY_DEFAULT_METHODS

    void Start()
    {
        offset = Vector3.up * (GlobalVariables.player.GetComponent<CharacterController>().height / 2);

        for (int i = 0; i < visionVertices.Length; i++)
        {
            visionVertices[i].x *= sightRadius / 2;
            visionVertices[i].y *= sightRadius / 2;
            visionVertices[i].z *= sightDistance;
        }
    }

    void Update()
    {
        playerLocation = GlobalVariables.player.transform.position + offset;
        Vector3 playerDirection = -Vector3.Normalize(transform.position - playerLocation);
        float distance = Vector3.Distance(transform.position, playerLocation);

        if (distance < sightDistance)
        {
            if (distance < spiderSenseRadius)
            {
                lookDirection = -Vector3.Normalize(transform.position - playerLocation);
            }

            Vector3[] vvTemp = new Vector3[visionVertices.Length];
            vvTemp = TranslateVertices(visionVertices, transform.position, lookDirection);

            Mesh mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = vvTemp;
            mesh.triangles = visionTriangles;

            bool bIsInside = IsPointInside(mesh, playerLocation);
            if (bIsInside)
            {
                RaycastHit hit;
                if (Physics.Raycast(
                    transform.position,
                    playerDirection,
                    out hit,
                    sightDistance,
                    1
                    ))
                {
                    if (hit.transform.tag == "Player")
                    {
                        bCanSeePlayer = true;
                        playerLastKnownLocation = playerLocation;
                    }
                    else
                    {
                        bCanSeePlayer = false;
                    }
                }
                else
                {
                    bCanSeePlayer = false;
                }
            }

            //Debug lines
            if (bIsInside)
            {
                Debug.DrawLine(transform.position, transform.position + playerDirection * sightDistance, bCanSeePlayer ? Color.green : Color.red);
                Debug.DrawLine(vvTemp[0], vvTemp[1], bCanSeePlayer ? Color.green : Color.yellow);
                Debug.DrawLine(vvTemp[0], vvTemp[2], bCanSeePlayer ? Color.green : Color.yellow);
                Debug.DrawLine(vvTemp[0], vvTemp[3], bCanSeePlayer ? Color.green : Color.yellow);
                Debug.DrawLine(vvTemp[0], vvTemp[4], bCanSeePlayer ? Color.green : Color.yellow);
            }
            else
            {
                Debug.DrawLine(vvTemp[0], vvTemp[1], Color.red);
                Debug.DrawLine(vvTemp[0], vvTemp[2], Color.red);
                Debug.DrawLine(vvTemp[0], vvTemp[3], Color.red);
                Debug.DrawLine(vvTemp[0], vvTemp[4], Color.red);
            }
        }
        else
        {
            bCanSeePlayer = false;
        }
        
        if (bCanSeePlayer)
        {
            lookDirection = playerDirection;
            gun.LookAt(transform.position + lookDirection);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = bCanSeePlayer ?
            Color.red
            : Color.yellow;
        Gizmos.DrawSphere(playerLastKnownLocation, 0.4f);

        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.2f);
        Gizmos.DrawSphere(transform.position, spiderSenseRadius);
    }

    #endregion

    #region CUSTOM_METHODS

    Vector3[] TranslateVertices(Vector3[] inputArray, Vector3 translateVector, Vector3 lookAtVector)
    {
        Vector3[] returnArray = new Vector3[inputArray.Length];
        lookAtVector = lookAtVector.normalized;

        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(
            Vector2.SignedAngle(new Vector2(Mathf.Clamp(1.25f - lookAtVector.y, 0.0f, 1.0f), lookAtVector.y), Vector2.right),
            Vector2.SignedAngle(new Vector2(lookAtVector.x, lookAtVector.z), Vector2.up),
            0.0f
            ));

        for (int i = 0; i < returnArray.Length; i++)
        {
            returnArray[i] = rotationMatrix.MultiplyPoint3x4(inputArray[i]) + translateVector;
        }

        return returnArray;
    }

    bool IsPointInside(Mesh aMesh, Vector3 aLocalPoint)
    {
        var verts = aMesh.vertices;
        var tris = aMesh.triangles;
        int triangleCount = tris.Length / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            var V1 = verts[tris[i * 3]];
            var V2 = verts[tris[i * 3 + 1]];
            var V3 = verts[tris[i * 3 + 2]];
            var P = new Plane(V1, V2, V3);
            if (P.GetSide(aLocalPoint))
                return false;
        }
        return true;
    }

    #endregion
}
