using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    [Header("Room Prefabs")]
    public GameObject startRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject monsterRoomPrefab;
    public GameObject fireCampRoomPrefab;
    public GameObject treasureRoomPrefab;
    public GameObject itemRoomPrefab;
    public GameObject prepareBossRoomPrefab;

    [Header("Dungeon Settings")]
    public int dungeonWidth = 5;
    public int dungeonHeight = 5;
    public int minRooms = 6;
    public int maxRooms = 10;
    public Vector2 roomSize = new Vector2(12f, 12f); // Custom spacing between rooms

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

    public Dictionary<Vector2Int, Room> dungeonGrid = new Dictionary<Vector2Int, Room>();
    private List<Vector2Int> roomPositions = new List<Vector2Int>();

    private int currentMonsterRooms = 0;
    private int currentFireCampRooms = 0;
    private int currentTreasureRooms = 0;
    private int currentItemRooms = 0;
    //private int treasureRoomCount = 0;

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

        // Clear all data structures
        ClearDungeon();

    }

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
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
        // Step 1: Create Start Room
        Vector2Int startPosition = Vector2Int.zero;
        Room startRoom = CreateRoom(startPosition, RoomType.Start);
        dungeonGrid[startPosition] = startRoom;
        roomPositions.Add(startPosition);

        // Step 2: Determine target room count (random between min and max)
        int targetRoomCount = Random.Range(minRooms, maxRooms + 1);

        // Step 3: Generate Procedural Rooms
        int attempts = 0;
        int maxAttempts = 100;

        while (roomPositions.Count < targetRoomCount && attempts < maxAttempts)
        {
            Vector2Int randomRoom = roomPositions[Random.Range(0, roomPositions.Count)];
            Vector2Int nextPosition = GetRandomAdjacentPosition(randomRoom);

            if (!dungeonGrid.ContainsKey(nextPosition))
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

        // Check if we met minimum room requirements
        if (roomPositions.Count < minRooms)
            return false;

        // Step 4: Ensure minimum room type requirements
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


    bool EnsureMinimumRooms()
    {
        // Check if we have space for minimum rooms
        int remainingPositions = dungeonWidth * dungeonHeight - roomPositions.Count;
        int requiredRooms = monsterRoomCount.x + fireCampRoomCount.x + treasureRoomCount.x + itemRoomCount.x;

        if (remainingPositions < requiredRooms)
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

    bool PlaceBossAndPrepareRooms()
    {
        // Find the farthest room from start
        Vector2Int farthestPosition = FindFarthestPosition();
        Debug.Log($"Farthest room position: {farthestPosition}");
        Debug.Log($"Farthest room type: {dungeonGrid[farthestPosition].roomType}");

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

        // Try each direction to place prepare room
        Vector2Int preparePosition = Vector2Int.zero;
        Vector2Int bossPosition = Vector2Int.zero;
        bool foundValidPositions = false;

        Debug.Log("Searching for valid positions...");
        foreach (Vector2Int dir in directions)
        {
            preparePosition = farthestPosition + dir;
            Debug.Log($"Trying prepare room at: {preparePosition}");

            // Skip if prepare position is already occupied
            if (dungeonGrid.ContainsKey(preparePosition))
            {
                Debug.Log($"Position {preparePosition} is already occupied by {dungeonGrid[preparePosition].roomType}");
                continue;
            }

            // Try to find a position for boss room adjacent to prepare room
            foreach (Vector2Int bossDir in directions)
            {
                bossPosition = preparePosition + bossDir;
                Debug.Log($"Trying boss room at: {bossPosition}");

                // Skip if boss position is already occupied or is the farthest room
                if (dungeonGrid.ContainsKey(bossPosition) || bossPosition == farthestPosition)
                {
                    Debug.Log($"Boss position {bossPosition} is invalid");
                    continue;
                }

                // We found valid positions for both rooms
                foundValidPositions = true;
                Debug.Log($"Found valid positions - Prepare: {preparePosition}, Boss: {bossPosition}");
                break;
            }

            if (foundValidPositions)
                break;
        }

        // If we couldn't find valid positions for both rooms, return false
        if (!foundValidPositions)
        {
            Debug.LogWarning("Could not find valid positions for prepare and boss rooms!");
            return false;
        }

        // Create and place prepare room
        Debug.Log("Creating prepare room...");
        Room prepareRoom = CreateRoom(preparePosition, RoomType.PrepareBoss);
        dungeonGrid[preparePosition] = prepareRoom;
        roomPositions.Add(preparePosition);
        ConnectRooms(farthestPosition, preparePosition);

        // Create and place boss room
        Debug.Log("Creating boss room...");
        Room bossRoom = CreateRoom(bossPosition, RoomType.Boss);
        dungeonGrid[bossPosition] = bossRoom;
        roomPositions.Add(bossPosition);
        ConnectRooms(preparePosition, bossPosition);

        // Verify room placement
        Debug.Log("\nFinal Room Configuration:");
        Debug.Log($"Farthest Room: Position={farthestPosition}, Type={dungeonGrid[farthestPosition].roomType}");
        Debug.Log($"Prepare Room: Position={preparePosition}, Type={dungeonGrid[preparePosition].roomType}");
        Debug.Log($"Boss Room: Position={bossPosition}, Type={dungeonGrid[bossPosition].roomType}");

        // Add verification method
        VerifyDungeonRooms();

        return true;


    }

    // Add this new verification method
    void VerifyDungeonRooms()
    {
        Debug.Log("\nVerifying all dungeon rooms:");

        bool foundPrepareBoss = false;
        bool foundBoss = false;
        Vector2Int preparePos = Vector2Int.zero;
        Vector2Int bossPos = Vector2Int.zero;

        foreach (var kvp in dungeonGrid)
        {
            Debug.Log($"Position: {kvp.Key}, Room Type: {kvp.Value.roomType}");

            if (kvp.Value.roomType == RoomType.PrepareBoss)
            {
                foundPrepareBoss = true;
                preparePos = kvp.Key;
            }
            else if (kvp.Value.roomType == RoomType.Boss)
            {
                foundBoss = true;
                bossPos = kvp.Key;
            }
        }

        Debug.Log($"\nVerification Results:");
        Debug.Log($"Found Prepare Boss Room: {foundPrepareBoss}");
        Debug.Log($"Found Boss Room: {foundBoss}");

        if (foundPrepareBoss && foundBoss)
        {
            bool areAdjacent = Vector2Int.Distance(preparePos, bossPos) == 1;
            Debug.Log($"Prepare Room and Boss Room are adjacent: {areAdjacent}");
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

    Vector2Int GetValidAdjacentPosition(Vector2Int current, Vector2Int exclude = default)
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
            if (!dungeonGrid.ContainsKey(newPos) && newPos != exclude)
            {
                return newPos;
            }
        }

        return Vector2Int.zero; // No valid position found
    }


    Vector2Int GetRandomAdjacentPosition(Vector2Int current)
    {
        Vector2Int[] directions = {
            new Vector2Int(0, 1),  // Up
            new Vector2Int(0, -1), // Down
            new Vector2Int(-1, 0), // Left
            new Vector2Int(1, 0)   // Right
        };

        return current + directions[Random.Range(0, directions.Length)];
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

    GameObject GetPrefabForRoomType(RoomType type)
    {
        switch (type)
        {
            case RoomType.Start: return startRoomPrefab;
            case RoomType.Boss: return bossRoomPrefab;
            case RoomType.Monster: return monsterRoomPrefab;
            case RoomType.FireCamp: return fireCampRoomPrefab;
            case RoomType.Treasure: return treasureRoomPrefab;
            case RoomType.Item: return itemRoomPrefab;
            case RoomType.PrepareBoss: return prepareBossRoomPrefab;
            default: return monsterRoomPrefab;
        }
    }

    RoomType DetermineRoomType()
    {
        List<RoomType> availableTypes = new List<RoomType>();
        Dictionary<RoomType, float> typeChances = new Dictionary<RoomType, float>();

        // Add available room types based on their current counts and chances
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

        // If no room types are available, return Monster
        if (availableTypes.Count == 0)
            return RoomType.Monster;

        // Calculate total probability
        float totalChance = 0f;
        foreach (var type in availableTypes)
        {
            totalChance += typeChances[type];
        }

        // Normalize probabilities if total is not 1
        if (totalChance != 1f)
        {
            foreach (var type in availableTypes)
            {
                typeChances[type] = typeChances[type] / totalChance;
            }
        }

        // Select room type based on probability
        float randomValue = Random.value;
        float currentTotal = 0f;

        foreach (var type in availableTypes)
        {
            currentTotal += typeChances[type];
            if (randomValue <= currentTotal)
            {
                switch (type)
                {
                    case RoomType.Monster:
                        currentMonsterRooms++;
                        break;
                    case RoomType.FireCamp:
                        currentFireCampRooms++;
                        break;
                    case RoomType.Treasure:
                        currentTreasureRooms++;
                        break;
                    case RoomType.Item:
                        currentItemRooms++;
                        break;
                }
                return type;
            }
        }

        // Fallback to Monster room
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
        Vector3 position = new Vector3(room.gridPosition.x * roomSize.x, room.gridPosition.y * roomSize.y, 0);
        GameObject roomObject = Instantiate(room.prefab, position, Quaternion.identity, transform);

        // Add room type to GameObject name for easier debugging
        roomObject.name = $"Room_{room.roomType}_{room.gridPosition}";

        // Activate doors based on connectivity
        RoomComponent roomComponent = roomObject.GetComponent<RoomComponent>();
        if (roomComponent != null)
        {
            roomComponent.SetDoors(room.upDoor, room.downDoor, room.leftDoor, room.rightDoor);
            Debug.Log($"Instantiated {room.roomType} at {room.gridPosition} with doors: Up={room.upDoor}, Down={room.downDoor}, Left={room.leftDoor}, Right={room.rightDoor}");
        }

    }
}