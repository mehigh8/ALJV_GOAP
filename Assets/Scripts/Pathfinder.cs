using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStar;
using AStar.Algolgorithms;

[RequireComponent(typeof(CharacterMovement))]
public class Pathfinder : MonoBehaviour
{
    [Header("Map Config")]
    public Vector2Int mapSize;
    public GameObject obstaclesParentObject;
    [Header("References")]
    public CharacterMovement characterMovement;
    [Space]
    public bool hasPath = false;

    [SerializeField] private List<Vector3Int> path = null;
    private bool[,] walkableMap = null;

    void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
        GenerateWalkableMap();
    }

    void Start()
    {
        GameManager.GetInstance().pickupSpawner.SpawnPickups(walkableMap, mapSize);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // For testing
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            point.z = 0;
            Debug.Log(FindPath(point));
        }

        if (path != null && path.Count > 0)
        {
            if (!characterMovement.isMoving)
            {
                Vector3Int nextPoint = path[0];
                path.RemoveAt(0);
                characterMovement.MoveTowards(nextPoint);
            }
        }
        else
        {
            hasPath = false;
        }
    }

    public bool FindPath(Vector3 destination)
    {
        (int, int) start = Helpers.ScaleToMap(Helpers.SnapToGrid(transform.position), mapSize);
        (int, int) dest = Helpers.ScaleToMap(Helpers.SnapToGrid(destination), mapSize);

        (int, int)[] _path = AStarBoolMap.GeneratePath(start.Item1, start.Item2, dest.Item1, dest.Item2, walkableMap);
        if (_path.Length == 0)
            return false;

        path = new List<Vector3Int>();
        for (int i = 0; i < _path.Length; i++)
            path.Add(Helpers.ScaleToWorld(_path[i], mapSize));
        hasPath = true;
        return true;
    }

    private void GenerateWalkableMap()
    {
        walkableMap = new bool[mapSize.y, mapSize.x];
        for (int i = 0; i < mapSize.y; i++)
            for (int j = 0; j < mapSize.x; j++)
                walkableMap[i, j] = true;

        Transform[] transforms = obstaclesParentObject.GetComponentsInChildren<Transform>();
        for (int i = 1; i < transforms.Length; i++)
        {
            (int, int) obstacle = Helpers.ScaleToMap(Helpers.SnapToGrid(transforms[i].position), mapSize);
            walkableMap[obstacle.Item2, obstacle.Item1] = false;
        }
    }
}
