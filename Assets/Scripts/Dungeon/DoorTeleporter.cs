using UnityEngine;

public class DoorTeleporter : MonoBehaviour
{
    public enum DoorDirection { Up, Down, Left, Right }
    public DoorDirection direction;

    private RoomComponent roomComponent;
    private DungeonController dungeonController;
    private Vector2Int currentGridPosition;
    private bool isLocked = true;

    private void Start()
    {
        dungeonController = FindObjectOfType<DungeonController>();
        roomComponent = GetComponentInParent<RoomComponent>();

        // Calculate current grid position
        Vector3 roomPosition = transform.parent.position;
        currentGridPosition = new Vector2Int(
            Mathf.RoundToInt(roomPosition.x / dungeonController.roomSize.x),
            Mathf.RoundToInt(roomPosition.y / dungeonController.roomSize.y)
        );

        // Start with the door locked (invisible) if in combat room
        Room currentRoom = dungeonController.dungeonGrid[currentGridPosition];
        if (currentRoom != null)
        {
            isLocked = ShouldRoomBeLocked(currentRoom.roomType);
        }

        UpdateDoorVisual();
    }

    private bool ShouldRoomBeLocked(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.FireCamp:
            case RoomType.Item:
            case RoomType.Start:
            case RoomType.Treasure:
            case RoomType.PrepareBoss:
                return false;
            case RoomType.Monster:
            case RoomType.Boss:
                return true;
            default:
                return true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isLocked)
        {
            Vector2Int targetGridPosition = GetTargetRoomPosition();
            if (dungeonController.dungeonGrid.ContainsKey(targetGridPosition))
            {
                TeleportPlayer(other.transform, targetGridPosition);
            }
        }
    }

    private Vector2Int GetTargetRoomPosition()
    {
        switch (direction)
        {
            case DoorDirection.Up:
                return currentGridPosition + Vector2Int.up;
            case DoorDirection.Down:
                return currentGridPosition + Vector2Int.down;
            case DoorDirection.Left:
                return currentGridPosition + Vector2Int.left;
            case DoorDirection.Right:
                return currentGridPosition + Vector2Int.right;
            default:
                return currentGridPosition;
        }
    }

    private void TeleportPlayer(Transform player, Vector2Int targetGridPosition)
    {
        // Calculate world position of target room
        Vector2 targetRoomWorldPos = new Vector2(
            targetGridPosition.x * dungeonController.roomSize.x,
            targetGridPosition.y * dungeonController.roomSize.y
        );

        // Calculate entry position based on which door we came from
        Vector2 entryOffset = GetEntryOffset();
        Vector2 targetPosition = targetRoomWorldPos + entryOffset;

        // Teleport the player
        player.position = targetPosition;
    }

    private Vector2 GetEntryOffset()
    {
        // Offset from the center of the room based on entry direction
        float offsetAmount = 15f; // Adjust this value to control how far from the door the player spawns

        switch (direction)
        {
            case DoorDirection.Up:    // Coming from bottom
                return new Vector2(0, -dungeonController.roomSize.y / 2 + offsetAmount);
            case DoorDirection.Down:  // Coming from top
                return new Vector2(0, dungeonController.roomSize.y / 2 - offsetAmount);
            case DoorDirection.Left:  // Coming from right
                return new Vector2(dungeonController.roomSize.x / 2 - offsetAmount, 0);
            case DoorDirection.Right: // Coming from left
                return new Vector2(-dungeonController.roomSize.x / 2 + offsetAmount, 0);
            default:
                return Vector2.zero;
        }
    }

    // Call this when all enemies in the room are defeated
    public void UnlockDoor()
    {
        isLocked = false;
        UpdateDoorVisual();
    }

    private void UpdateDoorVisual()
    {
        // Skip if dungeonController isn't initialized
        if (dungeonController == null || currentGridPosition == null)
        {
            Debug.LogWarning("DungeonController or currentGridPosition is null!");
            return;
        }

        // Skip if the room doesn't exist in grid
        if (!dungeonController.dungeonGrid.ContainsKey(currentGridPosition))
        {
            Debug.LogWarning($"Room at position {currentGridPosition} not found in dungeon grid!");
            return;
        }

        Room currentRoom = dungeonController.dungeonGrid[currentGridPosition];

        // If door is locked, keep it inactive
        if (isLocked)
        {
            gameObject.SetActive(false);
            return;
        }

        // Check if this door has a valid connection
        bool hasConnection = false;
        Vector2Int targetPosition = GetTargetRoomPosition();

        switch (direction)
        {
            case DoorDirection.Up:
                hasConnection = currentRoom.upDoor;
                break;
            case DoorDirection.Down:
                hasConnection = currentRoom.downDoor;
                break;
            case DoorDirection.Left:
                hasConnection = currentRoom.leftDoor;
                break;
            case DoorDirection.Right:
                hasConnection = currentRoom.rightDoor;
                break;
        }

        // Check if target room exists in the grid
        bool targetRoomExists = dungeonController.dungeonGrid.ContainsKey(targetPosition);

        // Only activate the door if there's a valid connection and the target room exists
        if (hasConnection && targetRoomExists)
        {
            // Special case for boss room connections
            if (dungeonController.dungeonGrid[targetPosition].roomType == RoomType.Boss)
            {
                // Only allow access from the prepare boss room
                if (currentRoom.roomType == RoomType.PrepareBoss)
                {
                    gameObject.SetActive(true);
                    Debug.Log($"Door to boss room activated at {gameObject.name}");
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                gameObject.SetActive(true);
                Debug.Log($"Door: {gameObject.name} is set active. Current position: {currentGridPosition}, Target position: {targetPosition}");
            }
        }
        else
        {
            gameObject.SetActive(false);
            Debug.Log($"Door: {gameObject.name} deactivated. No valid connection or target room.");
        }

    }

}
