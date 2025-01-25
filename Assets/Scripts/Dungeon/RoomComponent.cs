using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomComponent : MonoBehaviour
{
    public GameObject upDoor;
    public GameObject downDoor;
    public GameObject leftDoor;
    public GameObject rightDoor;

    public void SetDoors(bool up, bool down, bool left, bool right)
    {
        if (upDoor != null)
        {
            upDoor.SetActive(up);
            if (up) upDoor.GetComponent<DoorTeleporter>().direction = DoorTeleporter.DoorDirection.Up;
        }

        if (downDoor != null)
        {
            downDoor.SetActive(down);
            if (down) downDoor.GetComponent<DoorTeleporter>().direction = DoorTeleporter.DoorDirection.Down;
        }

        if (leftDoor != null)
        {
            leftDoor.SetActive(left);
            if (left) leftDoor.GetComponent<DoorTeleporter>().direction = DoorTeleporter.DoorDirection.Left;
        }

        if (rightDoor != null)
        {
            rightDoor.SetActive(right);
            if (right) rightDoor.GetComponent<DoorTeleporter>().direction = DoorTeleporter.DoorDirection.Right;
        }
    }

    // Optional: Method to unlock all doors in the room
    public void UnlockAllDoors()
    {
        UnlockDoorIfExists(upDoor);
        UnlockDoorIfExists(downDoor);
        UnlockDoorIfExists(leftDoor);
        UnlockDoorIfExists(rightDoor);
    }

    private void UnlockDoorIfExists(GameObject door)
    {
        if (door != null && door.activeInHierarchy)
        {
            var teleporter = door.GetComponent<DoorTeleporter>();
            if (teleporter != null)
            {
                teleporter.UnlockDoor();
            }
        }
    }
}
