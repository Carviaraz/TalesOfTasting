using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [System.Serializable]
    public class MonsterSpawnConfig
    {
        public GameObject monsterPrefab;
        [Range(0, 10)] public int minCount = 1;
        [Range(0, 10)] public int maxCount = 3;
        //[Range(0f, 1f)] public float spawnChance = 1f;
    }

    [Header("Monster Spawning")]
    public List<MonsterSpawnConfig> monsterSpawnConfigs = new List<MonsterSpawnConfig>();

    [Header("Spawn Settings")]
    [Range(0f, 10f)] public float spawnRadius = 5f;
    [Range(0f, 2f)] public float spawnOffset = 1f;

    private RoomComponent roomComponent;
    private RoomType roomType;
    private List<GameObject> spawnedMonsters = new List<GameObject>();
    private bool isCleared = false;
    private Vector2Int currentGridPosition;
    private DungeonController dungeonController;

    private DoorTeleporter[] doorTeleporters;

    private void Awake()
    {
        roomComponent = GetComponent<RoomComponent>();

        // Find all door teleporters in the room
        doorTeleporters = GetComponentsInChildren<DoorTeleporter>();

        // Get room type from parent dungeon controller
        Vector3 roomPosition = transform.position;
        var dungeonController = FindObjectOfType<DungeonController>();
        Vector2Int gridPosition = new Vector2Int(
            Mathf.RoundToInt(roomPosition.x / dungeonController.roomSize.x),
            Mathf.RoundToInt(roomPosition.y / dungeonController.roomSize.y)
        );
        if (dungeonController.dungeonGrid.ContainsKey(gridPosition))
        {
            roomType = dungeonController.dungeonGrid[gridPosition].roomType;
        }

        // Spawn monsters if this is an enemy room
        if (IsEnemyRoom())
        {
            SpawnMonsters();
        }
        else
        {
            // If it's not an enemy room, unlock and activate doors immediately
            Debug.Log("Door: Unlock because not monster room");
            UnlockAndActivateDoors();
        }
    }

    private void Update()
    {
        // Check if all monsters are defeated
        if (IsEnemyRoom() && !isCleared)
        {
            CheckRoomClear();
        }
    }

    private void SpawnMonsters()
    {
        // Clear any existing monsters
        spawnedMonsters.Clear();

        // Use all configurations without the spawn chance filter
        var availableConfigs = new List<MonsterSpawnConfig>(monsterSpawnConfigs);

        // Randomly determine total monster types (up to the number of available configs)
        int totalMonsterTypes = Random.Range(1, availableConfigs.Count + 1);

        for (int i = 0; i < totalMonsterTypes; i++)
        {
            // Get a random index from the remaining configs
            int randomIndex = Random.Range(0, availableConfigs.Count);
            var config = availableConfigs[randomIndex];

            // Remove this config so it won't be selected again
            availableConfigs.RemoveAt(randomIndex);

            // Determine number of monsters for this type
            int monsterCount = Random.Range(config.minCount, config.maxCount + 1);

            for (int j = 0; j < monsterCount; j++)
            {
                Vector3 spawnPosition = GetRandomSpawnPosition();
                GameObject monster = Instantiate(config.monsterPrefab, spawnPosition, Quaternion.identity, transform);
                spawnedMonsters.Add(monster);

                // Add health component listener
                var healthComponent = monster.GetComponent<EnemyHealth>();
                if (healthComponent != null)
                {
                    healthComponent.OnDeath += HandleMonsterDeath;
                }
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 centerPosition = transform.position;
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        return new Vector3(
            centerPosition.x + randomCircle.x,
            centerPosition.y + randomCircle.y + spawnOffset,
            centerPosition.z
        );
    }

    private void HandleMonsterDeath(GameObject deadMonster)
    {
        spawnedMonsters.Remove(deadMonster);
    }

    private void CheckRoomClear()
    {
        if (spawnedMonsters.Count == 0)
        {
            isCleared = true;
            Debug.Log("Door: Unlock because The room is clear");
            UnlockAndActivateDoors();
            Debug.Log("Room cleared! Unlocking and activating doors.");
        }
    }

    private void UnlockAndActivateDoors()
    {
        // Ensure door teleporters exist
        if (doorTeleporters == null || doorTeleporters.Length == 0)
        {
            doorTeleporters = GetComponentsInChildren<DoorTeleporter>();
        }

        // Unlock each door
        foreach (var doorTeleporter in doorTeleporters)
        {
            // Unlock the door (which will handle activation in UpdateDoorVisual)
            doorTeleporter.UnlockDoor();
        }

        // Make sure to activate the RoomComponent doors as well
        var roomComponent = GetComponent<RoomComponent>();
        if (roomComponent != null)
        {
            // This will ensure the door GameObjects are active if they have connections
            roomComponent.UpdateDoorsVisibility();
        }
    }

    private bool IsEnemyRoom()
    {
        return roomType == RoomType.Monster ||
               roomType == RoomType.Boss;
    }
}
