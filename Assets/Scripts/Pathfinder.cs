using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class Pathfinder : MonoBehaviour
{
    public class Node
    {
        public Node parent;
        public Vector3Int position;

        public Node(Node parent, Vector3Int position)
        {
            this.parent = parent;
            this.position = position;
        }
    }

    public CharacterMovement characterMovement;
    public GameObject obstaclesParentObject;
    public bool hasPath = false;

    private List<Node> path = null;
    private HashSet<Vector3Int> obstacles = new HashSet<Vector3Int>();

    void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
        GatherObstacles();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            point.z = 0;
            Debug.Log(FindPath(point));
        }

        if (path != null && path.Count > 0)
        {
            if (!characterMovement.isMoving)
            {
                Node node = path[0];
                path.RemoveAt(0);
                characterMovement.MoveTowards(node.position);
            }
        }
        else
        {
            hasPath = false;
        }
    }

    public bool FindPath(Vector3 destination)
    {
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        Vector3Int destinationInt = Helpers.SnapToGrid(destination);
        Vector3Int start = Helpers.SnapToGrid(transform.position);

        Node startNode = new Node(null, start);
        Queue<Node> bfsQueue = new Queue<Node>();
        bfsQueue.Enqueue(startNode);

        while (bfsQueue.Count > 0)
        {
            Node node = bfsQueue.Dequeue();

            if (node.position.Equals(destinationInt))
            {
                ConvertToPath(node);
                break;
            }

            visited.Add(node.position);
            for (int i = 0; i < Helpers.neswX.Length; i++)
            {
                Vector3Int newPosition = node.position + new Vector3Int(Helpers.neswX[i], Helpers.neswY[i]);
                if (visited.Contains(newPosition) || obstacles.Contains(newPosition))
                    continue;

                bfsQueue.Enqueue(new Node(node, newPosition));
            }
        }

        return path != null;
    }

    private void ConvertToPath(Node finalNode)
    {
        path = new List<Node>();

        while (finalNode.parent != null)
        {
            path.Add(finalNode);
            finalNode = finalNode.parent;
        }

        path.Reverse();
        hasPath = true;
    }

    private void GatherObstacles()
    {
        Transform[] transforms = obstaclesParentObject.GetComponentsInChildren<Transform>();
        for (int i = 1; i < transforms.Length; i++)
        {
            obstacles.Add(Helpers.SnapToGrid(transforms[i].position));
        }
    }
}
