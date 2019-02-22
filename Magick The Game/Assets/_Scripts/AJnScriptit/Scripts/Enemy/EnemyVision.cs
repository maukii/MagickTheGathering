using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyCore))]
public class EnemyVision : MonoBehaviour
{
    #region VARIABLES

    public Transform headTransform = null;

    [SerializeField] private float sightDistance = 30.0f;
    [SerializeField] private float sightRadius = 45.0f;
    [SerializeField] private float checkInterval = 0.5f;

    public bool bCanSeePlayer { get; private set; } = false;
    public Vector3 playerLKLocation { get; private set; } = Vector3.zero;

    private float checkTimer = 0.0f;
    private float raycastGraceTimer = 0.0f;
    private Vector3 playerPosition = Vector3.zero;
    private Vector3 playerOffset = Vector3.zero;

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
        playerOffset = Vector3.up * (GlobalVariables.player.GetComponent<CharacterController>().height / 2);
        checkTimer = Random.Range(0.0f, 2.0f);

        for (int i = 0; i < visionVertices.Length; i++)
        {
            visionVertices[i].x *= sightRadius / 2;
            visionVertices[i].y *= sightRadius / 2;
            visionVertices[i].z *= sightDistance;
        }
    }

    void FixedUpdate()
    {
        playerPosition = GlobalVariables.player.transform.position + playerOffset;
        Vector3 playerDirection = -Vector3.Normalize(transform.position - playerPosition);
        Quaternion headRotation = headTransform.rotation;

        if (bCanSeePlayer)
        {
            headTransform.LookAt(playerPosition);
            headTransform.rotation = Quaternion.Lerp(headTransform.rotation, headRotation, 0.9f);

            RaycastHit hit;
            if (Physics.Raycast(
                playerPosition,
                Vector3.down,
                out hit,
                Mathf.Infinity,
                1
                ))
            {
                playerLKLocation = hit.point + playerOffset;
            }
            else
            {
                playerLKLocation = playerPosition;
            }
        }
        else
        {
            headTransform.LookAt(headTransform.position + Vector3.Normalize(GetComponent<NavMeshAgent>().velocity));
            headTransform.rotation = Quaternion.Lerp(headTransform.rotation, headRotation, 0.9f);

            if (Vector3.Distance(transform.position, playerLKLocation) < 1.0f)
            {
                playerLKLocation = Vector3.zero;
            }
        }

        if (!bCanSeePlayer && checkTimer > 0.0f)
        {
            checkTimer -= Time.fixedDeltaTime;
        }
        else
        {
            if (!bCanSeePlayer)
            {
                checkTimer = checkInterval;
            }

            if (Vector3.Distance(headTransform.position, playerPosition) < sightDistance)
            {
                Vector3[] vvTemp = new Vector3[visionVertices.Length];
                vvTemp = TranslateVertices(visionVertices, transform.position, headTransform.rotation);
                Mesh mesh = GenerateMesh(vvTemp, visionTriangles);
                bool bIsInside = IsPointInside(mesh, playerPosition);

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
                            raycastGraceTimer = 0.2f;
                        }
                        else
                        {
                            if (raycastGraceTimer > 0.0f)
                            {
                                raycastGraceTimer -= Time.fixedDeltaTime;
                            }
                            else
                            {
                                bCanSeePlayer = false;
                            }
                        }
                    }
                    else
                    {
                        if (raycastGraceTimer > 0.0f)
                        {
                            raycastGraceTimer -= Time.fixedDeltaTime;
                        }
                        else
                        {
                            bCanSeePlayer = false;
                        }
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
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = bCanSeePlayer ?
            Color.red
            : Color.yellow;
        Gizmos.DrawSphere(playerLKLocation, 0.4f);

        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.2f);
    }

    #endregion

    #region CUSTOM_METHODS

    public void LookAt(Vector3 position)
    {
        headTransform.LookAt(position);
    }

    Vector3[] TranslateVertices(Vector3[] inputArray, Vector3 translateVector, Quaternion headRotation)
    {
        Vector3[] returnArray = new Vector3[inputArray.Length];
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(headRotation);

        for (int i = 0; i < returnArray.Length; i++)
        {
            returnArray[i] = rotationMatrix.MultiplyPoint3x4(inputArray[i]) + translateVector;
        }

        return returnArray;
    }

    Mesh GenerateMesh(Vector3[] vertices, int[] triangles)
    {
        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        return mesh;
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
