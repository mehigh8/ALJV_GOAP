using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static int[] neswX = { 0, 1, 0, -1 };
    public static int[] neswY = { 1, 0, -1, 0 };

    public static Vector3Int ConvertToInt(Vector3 v)
    {
        return new Vector3Int((int)v.x, (int)v.y, (int)v.z);
    }

    public static Vector3Int SnapToGrid(Vector3 position)
    {
        return ConvertToInt(new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z)));
    }

    public static (int, int) ScaleToMap(Vector3Int worldPos, Vector2Int mapSize)
    {
        return (worldPos.x + mapSize.x / 2, mapSize.y / 2 - worldPos.y);
    }

    public static Vector3Int ScaleToWorld((int, int) mapPosition, Vector2Int mapSize)
    {
        return new Vector3Int(mapPosition.Item1 - mapSize.x / 2, mapSize.y / 2 - mapPosition.Item2, 0);
    }
}
