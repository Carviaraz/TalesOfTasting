using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    [Header("PlayerConfig")]
    public PlayerConfig[] characters;

    [Header("Room Prefabs")]
    public List<GameObject> startRoomPrefabs = new List<GameObject>();
    public List<GameObject> bossRoomPrefabs = new List<GameObject>();
    public List<GameObject> monsterRoomPrefabs = new List<GameObject>();
    public List<GameObject> fireCampRoomPrefabs = new List<GameObject>();
    public List<GameObject> treasureRoomPrefabs = new List<GameObject>();
    public List<GameObject> itemRoomPrefabs = new List<GameObject>();
    public List<GameObject> prepareBossRoomPrefabs = new List<GameObject>();

    [Header("Dungeon Settings")]
    public int dungeonRadius = 2;
    public int minRooms = 10;
    public int maxRooms = 15;
    public Vector2 roomSize = new Vector2(50f, 48f); // Custom spacing between rooms

    [Header("Room Type Limits")]
    [MinMax(0, 10)] public Vector2Int monsterRoomCount = new Vector2Int(2, 5);
    [MinMax(0, 2)] public Vector2Int fireCampRoomCount = new Vector2Int(1, 1);
    [MinMax(0, 3)] public Vector2Int treasureRoomCount = new Vector2Int(1, 2);
    [MinMax(0, 4)] public Vector2Int itemRoomCount = new Vector2Int(1, 3);

    [Header("Room Spawn Chances")]
    [Range(0f, 1f)] public float monsterRoomChance = 0.4f;
    [Range(0f, 1f)] public float fireCampRoomChance = 0.1f;
    [Range(0f, 1f)] public float treasureRoomChance = 0.1f;
    [Range(0f, 1f)] public float itemRoomChance = 0.2f;

    [Header("Variation Settings")]
    [Range(0f, 1f)] public float roomVariationChance = 0.3f;

    public Dictionary<Vector2Int, Room> dungeonGrid = new Dictionary<Vector2Int, Room>();
    private List<Vector2Int> roomPositions = new List<Vector2Int>();

    private int currentMonsterRooms = 0;
    private int currentFireCampRooms = 0;
    private int currentTreasureRooms = 0;
    private int currentItemRooms = 0;
    //private int treasureRoomCount = 0;

    void Start()
    {
        GenerateDungeon();

        // Spawn player
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
        if (selectedIndex >= 0 && selectedIndex < characters.Length)
        {
            Instantiate(characters[selectedIndex].PlayerPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Invalid character index!");
        }
    }

    [ContextMenu("Regenerate Dungeon")]
    public void RegenerateDungeon()
    {
        DestroyExistingDungeon();
        GenerateDungeon();
    }

    private void DestroyExistingDungeon()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        ClearDungeon();
    }

    void GenerateDungeon()
    {
        if (!ValidateDungeonConfiguration())
        {
            Debug.LogError("Invalid dungeon configuration! Generation aborted.");
            return;
        }

        bool validDungeon = false;
        while (!validDungeon)
        {
            ClearDungeon();
            validDungeon = TryGenerateDungeon();
        }
    }

    void ClearDungeon()
    {
        dungeonGrid.Clear();
        roomPositions.Clear();
        currentMonsterRooms = 0;
        currentFireCampRooms = 0;
        currentTreasureRooms = 0;
        currentItemRooms = 0;
    }

    bool TryGenerateDungeon()
    {
        // Step 1: Create Start Room at center (0,0)
        Vector2Int startPosition = Vector2Int.zero;
        Room startRoom = CreateRoom(startPosition, RoomType.Start);
        dungeonGrid[startPosition] = startRoom;
        roomPositions.Add(startPosition);

        // Step 2: Determine target room count
        int targetRoomCount = Random.Range(minRooms, maxRooms + 1);

        // Step 3: Generate Procedural Rooms
        int attempts = 0;
        int maxAttempts = 100;

        while (roomPositions.Count < targetRoomCount && attempts < maxAttempts)
        {
            Vector2Int randomRoom = roomPositions[Random.Range(0, roomPositions.Count)];
            Vector2Int nextPosition = GetRandomAdjacentPosition(randomRoom);

            if (nextPosition != randomRoom && !dungeonGrid.ContainsKey(nextPosition))
            {
                RoomType roomType = DetermineRoomType();
                if (roomType == RoomType.Monster && currentMonsterRooms >= monsterRoomCount.y)
                {
                    attempts++;
                    continue;
                }

                Room newRoom = CreateRoom(nextPosition, roomType);
                dungeonGrid[nextPosition] = newRoom;
                roomPositions.Add(nextPosition);
                ConnectRooms(randomRoom, nextPosition);
            }
            attempts++;
        }

        if (roomPositions.Count < minRooms)
            return false;

        // Step 4: Ensure minimum room requirements
        if (!EnsureMinimumRooms())
            return false;

        // Step 5: Place Boss Room and Prepare Room
        if (!PlaceBossAndPrepareRooms())
            return false;

        // Step 6: Instantiate All Rooms
        foreach (var kvp in dungeonGrid)
        {
            InstantiateRoom(kvp.Value);
        }

        return true;
    }

    private bool IsWithinDungeonBounds(Vector2Int position)
    {
        return Mathf.Abs(position.x) <= dungeonRadius &&
               Mathf.Abs(position.y) <= dungeonRadius;
    }

    private Vector2Int GetRandomAdjacentPosition(Vector2Int current)
    {
        Vector2Int[] directions = {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        // Shuffle directions for randomness
        for (int i = directions.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Vector2Int temp = directions[i];
            directions[i] = directions[j];
            directions[j] = temp;
        }

        foreach (Vector2Int dir in directions)
        {
            Vector2Int newPos = current + dir;
            if (IsWithinDungeonBounds(newPos) && !dungeonGrid.ContainsKey(newPos))
            {
                return newPos;
            }
        }

        return current; // Return current if no valid position found
    }

    bool EnsureMinimumRooms()
    {
        // Check if we have space for minimum rooms
        int totalGridSize = (dungeonRadius * 2 + 1) * (dungeonRadius * 2 + 1);
        int requiredRooms = monsterRoomCount.x + fireCampRoomCount.x + treasureRoomCount.x + itemRoomCount.x;

        if (totalGridSize - roomPositions.Count < requiredRooms)
            return false;

        // Ensure minimum monster rooms
        while (currentMonsterRooms < monsterRoomCount.x)
        {
            if (!ConvertRandomRoomToType(RoomType.Monster))
                return false;
        }

        // Ensure minimum fire camp rooms
        while (currentFireCampRooms < fireCampRoomCount.x)
        {
            if (!ConvertRandomRoomToType(RoomType.FireCamp))
                return false;
        }

        // Ensure minimum treasure rooms
        while (currentTreasureRooms < treasureRoomCount.x)
        {
            if (!ConvertRandomRoomToType(RoomType.Treasure))
                return false;
        }

        // Ensure minimum item rooms
        while (currentItemRooms < itemRoomCount.x)
        {
            if (!ConvertRandomRoomToType(RoomType.Item))
                return false;
        }

        return true;
    }

    bool ConvertRandomRoomToType(RoomType newType)
    {
        foreach (var position in roomPositions)
        {
            Room room = dungeonGrid[position];
            if (room.roomType == RoomType.Monster)
            {
                room.roomType = newType;
                room.prefab = GetPrefabForRoomType(newType);
                UpdateRoomTypeCount(newType, 1);
                UpdateRoomTypeCount(RoomType.Monster, -1);
                return true;
            }
        }
        return false;
    }

    void UpdateRoomTypeCount(RoomType type, int change)
    {
        switch (type)
        {
            case RoomType.Monster:
                currentMonsterRooms += change;
                break;
            case RoomType.FireCamp:
                currentFireCampRooms += change;
                break;
            case RoomType.Treasure:
                currentTreasureRooms += change;
                break;
            case RoomType.Item:
                currentItemRooms += change;
                break;
        }
    }

    Vector2Int FindFarthestPosition()
    {
        Vector2Int farthestPosition = Vector2Int.zero;
        float maxDistance = 0;

        foreach (var position in roomPositions)
        {
            float distance = Vector2Int.Distance(Vector2Int.zero, position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestPosition = position;
            }
        }

        return farthestPosition;
    }

    bool PlaceBossAndPrepareRooms()
    {
        Vector2Int farthestPosition = FindFarthestPosition();
        Vector2Int[] directions = {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        // Shuffle directions
        for (int i = directions.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Vector2Int temp = directions[i];
            directions[i] = directions[j];
            directions[j] = temp;
        }

        // Try to place prepare and boss rooms
        foreach (Vector2Int dir in directions)
        {
            Vector2Int preparePosition = farthestPosition + dir;

            if (!IsWithinDungeonBounds(preparePosition) || dungeonGrid.ContainsKey(preparePosition))
                continue;

            foreach (Vector2Int bossDir in directions)
            {
                Vector2Int bossPosition = preparePosition + bossDir;

                if (!IsWithinDungeonBounds(bossPosition) ||
                    dungeonGrid.ContainsKey(bossPosition) ||
                    bossPosition == farthestPosition)
                    continue;

                // Create prepare room
                Room prepareRoom = CreateRoom(preparePosition, RoomType.PrepareBoss);
                dungeonGrid[preparePosition] = prepareRoom;
                roomPositions.Add(preparePosition);
                ConnectRooms(farthestPosition, preparePosition);

                // Create boss room
                Room bossRoom = CreateRoom(bossPosition, RoomType.Boss);
                dungeonGrid[bossPosition] = bossRoom;
                roomPositions.Add(bossPosition);
                ConnectRooms(preparePosition, bossPosition);

                return true;
            }
        }

        return false;
    }

    Room CreateRoom(Vector2Int position, RoomType type)
    {
        GameObject prefab = GetPrefabForRoomType(type);
        return new Room
        {
            gridPosition = position,
            prefab = prefab,
            roomType = type,
            upDoor = false,
            downDoor = false,
            leftDoor = false,
            rightDoor = false
        };
    }

    RoomType DetermineRoomType()
    {
        List<RoomType> availableTypes = new List<RoomType>();
        Dictionary<RoomType, float> typeChances = new Dictionary<RoomType, float>();

        if (currentMonsterRooms < monsterRoomCount.y)
        {
            availableTypes.Add(RoomType.Monster);
            typeChances[RoomType.Monster] = monsterRoomChance;
        }
        if (currentFireCampRooms < fireCampRoomCount.y)
        {
            availableTypes.Add(RoomType.FireCamp);
            typeChances[RoomType.FireCamp] = fireCampRoomChance;
        }
        if (currentTreasureRooms < treasureRoomCount.y)
        {
            availableTypes.Add(RoomType.Treasure);
            typeChances[RoomType.Treasure] = treasureRoomChance;
        }
        if (currentItemRooms < itemRoomCount.y)
        {
            availableTypes.Add(RoomType.Item);
            typeChances[RoomType.Item] = itemRoomChance;
        }

        if (availableTypes.Count == 0)
            return RoomType.Monster;

        float totalChance = 0f;
        foreach (var type in availableTypes)
        {
            totalChance += typeChances[type];
        }

        if (totalChance != 1f)
        {
            foreach (var type in availableTypes)
            {
                typeChances[type] = typeChances[type] / totalChance;
            }
        }

        float randomValue = Random.value;
        float currentTotal = 0f;

        foreach (var type in availableTypes)
        {
            currentTotal += typeChances[type];
            if (randomValue <= currentTotal)
            {
                UpdateRoomTypeCount(type, 1);
                return type;
            }
        }

        currentMonsterRooms++;
        return RoomType.Monster;
    }

    void ConnectRooms(Vector2Int roomA, Vector2Int roomB)
    {
        Vector2Int direction = roomB - roomA;

        if (direction == Vector2Int.up)
        {
            dungeonGrid[roomA].upDoor = true;
            dungeonGrid[roomB].downDoor = true;
        }
        else if (direction == Vector2Int.down)
        {
            dungeonGrid[roomA].downDoor = true;
            dungeonGrid[roomB].upDoor = true;
        }
        else if (direction == Vector2Int.left)
        {
            dungeonGrid[roomA].leftDoor = true;
            dungeonGrid[roomB].rightDoor = true;
        }
        else if (direction == Vector2Int.right)
        {
            dungeonGrid[roomA].rightDoor = true;
            dungeonGrid[roomB].leftDoor = true;
        }
    }

    void InstantiateRoom(Room room)
    {
        Vector3 position = new Vector3(
            room.gridPosition.x * roomSize.x,
            room.gridPosition.y * roomSize.y,
            0
        );

        GameObject roomObject = Instantiate(room.prefab, position, Quaternion.identity, transform);
        roomObject.name = $"Room_{room.roomType}_{room.gridPosition}";

        RoomComponent roomComponent = roomObject.GetComponent<RoomComponent>();
        if (roomComponent != null)
        {
            roomComponent.SetDoors(room.upDoor, room.downDoor, room.leftDoor, room.rightDoor);
            Debug.Log($"Instantiated {room.roomType} at {room.gridPosition} with doors: Up={room.upDoor}, Down={room.downDoor}, Left={room.leftDoor}, Right={room.rightDoor}");
        }
    }

    private bool ValidateDungeonConfiguration()
    {
        int totalGridSize = (dungeonRadius * 2 + 1) * (dungeonRadius * 2 + 1);
        int minRequiredRooms = monsterRoomCount.x + fireCampRoomCount.x +
                              treasureRoomCount.x + itemRoomCount.x +
                              3; // +3 for start, prepare boss, and boss rooms

        Debug.Log($"Grid Size: {dungeonRadius * 2 + 1}x{dungeonRadius * 2 + 1} = {totalGridSize} possible rooms");
        Debug.Log($"Room Range: {minRooms}-{maxRooms} rooms");
        Debug.Log($"Minimum Required Rooms: {minRequiredRooms}");

        if (maxRooms > totalGridSize)
        {
            Debug.LogError($"Error: Maximum rooms ({maxRooms}) cannot exceed grid size ({totalGridSize})");
            return false;
        }

        if (minRooms > maxRooms)
        {
            Debug.LogError($"Error: Minimum rooms ({minRooms}) cannot be greater than maximum rooms ({maxRooms})");
            return false;
        }

        if (minRequiredRooms > maxRooms)
        {
            Debug.LogError($"Error: Total minimum required rooms ({minRequiredRooms}) exceeds maximum rooms ({maxRooms})");
            return false;
        }

        if (minRooms < minRequiredRooms)
        {
            Debug.LogError($"Error: Minimum rooms ({minRooms}) is less than required minimum rooms ({minRequiredRooms})");
            return false;
        }

        return true;
    }

    GameObject GetRandomRoomPrefab(RoomType type)
    {
        List<GameObject> prefabList = GetPrefabListForRoomType(type);

        // If no prefabs exist for this type, default to monster room
        if (prefabList == null || prefabList.Count == 0)
        {
            Debug.LogWarning($"No prefabs found for room type {type}. Defaulting to monster room.");
            prefabList = monsterRoomPrefabs;
        }

        // Randomly select a prefab, with a chance to use a different variant
        if (Random.value < roomVariationChance && prefabList.Count > 1)
        {
            return prefabList[Random.Range(0, prefabList.Count)];
        }

        // If only one prefab or variation chance fails, return the first prefab
        return prefabList[0];
    }

    List<GameObject> GetPrefabListForRoomType(RoomType type)
    {
        switch (type)
        {
            case RoomType.Start: return startRoomPrefabs;
            case RoomType.Boss: return bossRoomPrefabs;
            case RoomType.Monster: return monsterRoomPrefabs;
            case RoomType.FireCamp: return fireCampRoomPrefabs;
            case RoomType.Treasure: return treasureRoomPrefabs;
            case RoomType.Item: return itemRoomPrefabs;
            case RoomType.PrepareBoss: return prepareBossRoomPrefabs;
            default: return monsterRoomPrefabs;
        }
    }
    GameObject GetPrefabForRoomType(RoomType type)
    {
        return GetRandomRoomPrefab(type);
    }

}