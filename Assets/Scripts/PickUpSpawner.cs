using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    public int pickupCount = 0;
    public List<GameObject> possiblePickups = new List<GameObject>();

    public void SpawnPickups(bool[,] map, Vector2Int mapSize)
    {
        for (int i = 0; i < pickupCount * possiblePickups.Count; i++)
        {
            (int, int) potentialIndexes = (Random.Range(0, mapSize.x), Random.Range(0, mapSize.y));
            while (map[potentialIndexes.Item2, potentialIndexes.Item1] == false)
                potentialIndexes = (Random.Range(0, mapSize.x), Random.Range(0, mapSize.y));

            Instantiate(possiblePickups[i % possiblePickups.Count], Helpers.ScaleToWorld(potentialIndexes, mapSize), Quaternion.identity);
        }
    }
}
