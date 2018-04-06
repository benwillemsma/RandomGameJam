using UnityEngine;

public class ClimbingNode : MonoBehaviour
{
    private Vector3 playerPosition;
    public Vector3 PlayerPosition
    {
        get {return playerPosition; }
    }

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

    protected void Start ()
    {
        if (!rightHand || !leftHand || !rightFoot || !leftFoot)
            Debug.LogError("Climbing Node: " + gameObject.name + " is not set up properly", this);
        else
        {
            neighbours = new ClimbingNode[8];
            distances = new float[neighbours.Length];

            for (int i = 0; i < distances.Length; i++)
                distances[i] = Mathf.Infinity;

            playerPosition = transform.position - transform.forward * 0.4f - transform.up * 1.7f;
            CalculateNodeNeighbors();
        }
    }

    private void Update()
    {
        if (!transform.gameObject.isStatic)
            Rotate();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawLine(transform.position, rightHand.transform.position);
        Gizmos.DrawLine(transform.position, leftHand.transform.position);
        Gizmos.DrawLine(transform.position, rightFoot.transform.position);
        Gizmos.DrawLine(transform.position, leftFoot.transform.position);

        Gizmos.color = Color.red;
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i])
                Gizmos.DrawLine(transform.position, neighbours[i].transform.position);
        }
    }

    private void Rotate()
    {

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
