using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ClimbingNode : MonoBehaviour
{
    // Public
    [Space(10)]
    public Transform rightHand;
    public Transform leftHand;
    public Transform rightFoot;
    public Transform leftFoot;

    public ClimbingNode[] neighbours;
    public float[] distances;

    public float DetectionRadius = 2f;

    private ClimbingNode currentNode;
    private Vector3[] CompareDirection = new Vector3[8];

    public Vector3 PlayerOffset;
    public Vector3 PlayerPosition
    {
        get { return transform.position + transform.rotation * PlayerOffset; }
    }

    //Private
    private Collider col;

    private bool m_active = true;
    public bool Active
    {
        get { return m_active; }
    }
    private int m_rotation;
    public int Rotation
    {
        get { return m_rotation; }
        set { m_rotation = value; }
    }

    protected void Start ()
    {
        col = GetComponent<Collider>();

        if (!rightHand || !leftHand || !rightFoot || !leftFoot)
            Debug.LogError("Climbing Node: " + gameObject.name + " is not set up properly", this);
        else
        {
            neighbours = new ClimbingNode[8];
            distances = new float[neighbours.Length];

            for (int i = 0; i < distances.Length; i++)
                distances[i] = Mathf.Infinity;
            
            CalculateNodeNeighbors();
        }
    }

    private void Update()
    {
        if (!transform.gameObject.isStatic)
            Rotate();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawLine(transform.position, rightHand.transform.position);
        Gizmos.DrawLine(transform.position, leftHand.transform.position);
        Gizmos.DrawLine(transform.position, rightFoot.transform.position);
        Gizmos.DrawLine(transform.position, leftFoot.transform.position);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(rightHand.transform.position, 0.1f);
        Gizmos.DrawSphere(leftHand.transform.position, 0.1f);
        Gizmos.DrawSphere(rightFoot.transform.position, 0.1f);
        Gizmos.DrawSphere(leftFoot.transform.position, 0.1f);

        Gizmos.color = Color.red;
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i])
                Gizmos.DrawLine(transform.position, neighbours[i].transform.position);
        }
    }

    public virtual void Rotate()
    {
        m_active = Vector3.Dot(-transform.forward, Vector3.up) < 0.9f;
        col.enabled = m_active;

        if (Vector3.Dot(transform.up, Vector3.up) < 0)
        {
            m_rotation = (m_rotation + 4) % 8;
            transform.rotation = Quaternion.AngleAxis(180, transform.forward) * transform.rotation;
        }

        if (transform.rotation.eulerAngles.z > 0.5f && transform.rotation.eulerAngles.z < 359.5f)
        {
            m_rotation = Mathf.RoundToInt((360 - transform.localEulerAngles.z) / 45);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }
    }

    #region NeighbourFunctions
    private void CalculateNodeNeighbors()
    {
        currentNode = GetComponent<ClimbingNode>();
        Collider[] NodeTriggers = Physics.OverlapSphere(transform.position, DetectionRadius);
        ClimbingNode[] detectedNodes = new ClimbingNode[NodeTriggers.Length];

        for (int i = 0; i < NodeTriggers.Length; i++)
        {
            ClimbingNode checkNode = NodeTriggers[i].GetComponent<ClimbingNode>();
            if (checkNode)
                if (checkNode != detectedNodes[i])
                    detectedNodes[i] = checkNode;
        }

        CompareDirection[0] = transform.up;
        CompareDirection[1] = (transform.up + transform.right).normalized;
        CompareDirection[2] = transform.right;
        CompareDirection[3] = (-transform.up + transform.right).normalized;
        CompareDirection[4] = -transform.up;
        CompareDirection[5] = (-transform.up - transform.right).normalized;
        CompareDirection[6] = -transform.right;
        CompareDirection[7] = (transform.up - transform.right).normalized;

        ResetNeighbours();
        
        foreach (ClimbingNode checkNode in detectedNodes)
        {
            if (checkNode && (checkNode != currentNode))
                DetermineNeighbour(checkNode);
        }
    }

    private void DetermineNeighbour(ClimbingNode checkNode)
    {
        for (int i = 0; i < CompareDirection.Length; i++)
        {
            Vector3 relativeDirection = Quaternion.AngleAxis(checkNode.transform.eulerAngles.y - transform.eulerAngles.y, transform.up) * CompareDirection[i];

            float compareAngle = 22.5f + 45f * (1f - (checkNode.transform.position - transform.position).magnitude);
            if (Vector3.Angle(relativeDirection, checkNode.transform.position - transform.position) < compareAngle)
            {
                float newDistance = (checkNode.transform.position - transform.position).magnitude;
                if (currentNode.neighbours[i] != null)
                {
                    if (newDistance < currentNode.distances[i])
                    {
                        currentNode.neighbours[i] = checkNode;
                        currentNode.distances[i] = newDistance;
                    }
                }
                else
                {
                    currentNode.neighbours[i] = checkNode;
                    currentNode.distances[i] = newDistance;
                }
            }
        }
    }

    private void ResetNeighbours()
    {
        for (int i = 0; i < currentNode.neighbours.Length; i++)
        {
            if (currentNode.neighbours[i])
                currentNode.neighbours[i] = null;
        }
    }
    #endregion
}
