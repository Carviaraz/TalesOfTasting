using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTeleporter : MonoBehaviour
{
    public enum DoorDirection { Up, Down, Left, Right }
    public DoorDirection direction;

    private RoomComponent roomComponent;
    private DungeonController dungeonController;
    private Vector2Int currentGridPosition;
    private bool isLocked = true;

    private SpriteRenderer spriteRenderer; // Reference to door sprite
    private Color lockedColor = Color.gray;
    private Color unlockedColor = Color.white;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        dungeonController = FindObjectOfType<DungeonController>();
        roomComponent = GetComponentInParent<RoomComponent>();

        // Calculate current grid position
        Vector3 roomPosition = transform.parent.position; // Assuming door is child of room
        currentGridPosition = new Vector2Int(
            Mathf.RoundToInt(roomPosition.x / dungeonController.roomSize.x),
            Mathf.RoundToInt(roomPosition.y / dungeonController.roomSize.y)
        );

        UpdateDoorVisual();
    }

    private bool ShouldRoomBeLocked(DungeonController.RoomType roomType)
    {
        switch (roomType)
        {
            case DungeonController.RoomType.FireCamp:
            case DungeonController.RoomType.Item:
            case DungeonController.RoomType.Start:
            case DungeonController.RoomType.Treasure:
                return false;
            default:
                return true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger entered by {other.tag}, Door locked: {isLocked}");
        if (other.CompareTag("Player") && !isLocked)
        {
            Vector2Int targetGridPosition = GetTargetRoomPosition();
            Debug.Log($"Target position: {targetGridPosition}");
            if (FindObjectOfType<DungeonController>().dungeonGrid.ContainsKey(targetGridPosition))
            {
                Debug.Log("Teleporting player");
                TeleportPlayer(other.transform, targetGridPosition);
            }
            else
            {
                Debug.Log("Target room not found in dungeon grid");
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
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isLocked ? lockedColor : unlockedColor;
            // You could also change the sprite or play an animation here
        }
    }
}
