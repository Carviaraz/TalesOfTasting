using UnityEngine;

public class Room
{
    public GameObject prefab;
    public Vector2Int gridPosition;
    public RoomType roomType;
    public bool upDoor, downDoor, leftDoor, rightDoor;
}
