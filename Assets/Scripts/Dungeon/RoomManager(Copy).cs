//using UnityEngine;

//public class RoomManager : MonoBehaviour
//{
//    private RoomComponent roomComponent;
//    private RoomType roomType;

//    private void Awake()
//    {
//        roomComponent = GetComponent<RoomComponent>();

//        // Get room type from parent dungeon controller
//        Vector3 roomPosition = transform.position;
//        var dungeonController = FindObjectOfType<DungeonController>();
//        Vector2Int gridPosition = new Vector2Int(
//            Mathf.RoundToInt(roomPosition.x / dungeonController.roomSize.x),
//            Mathf.RoundToInt(roomPosition.y / dungeonController.roomSize.y)
//        );

//        if (dungeonController.dungeonGrid.ContainsKey(gridPosition))
//        {
//            roomType = dungeonController.dungeonGrid[gridPosition].roomType;
//        }

//        // Find all enemies in the room if this is a combat room
//        if (IsEnemyRoom())
//        {
//            //// Find enemies in all child objects
//            //Enemy[] roomEnemies = GetComponentsInChildren<Enemy>(true);
//            //enemies.AddRange(roomEnemies);

//            //// Subscribe to enemy death events
//            //foreach (var enemy in enemies)
//            //{
//            //    MonitorEnemy(enemy);
//            //}

//            // TODO Remove later
//            //roomComponent.UnlockAllDoors();
//        }
//        else
//        {
//            // If it's not an enemy room, unlock doors immediately
//            roomComponent.UnlockAllDoors();
//        }
//    }

//    private bool IsEnemyRoom()
//    {
//        return roomType == RoomType.Monster ||
//               roomType == RoomType.Boss;
//    }

//    //private void MonitorEnemy(Enemy enemy)
//    //{
//    //    // Use a coroutine to check enemy existence every frame
//    //    StartCoroutine(CheckEnemyStatus(enemy));
//    //}

//    //private System.Collections.IEnumerator CheckEnemyStatus(Enemy enemy)
//    //{
//    //    while (enemy != null && enemy.CurrentHealth > 0)
//    //    {
//    //        yield return null;
//    //    }

//    //    // Enemy has been destroyed, remove it from the list
//    //    enemies.Remove(enemy);

//    //    // Check if all enemies are defeated
//    //    CheckRoomCompletion();
//    //}

//    //private void CheckRoomCompletion()
//    //{
//    //    if (enemies.Count == 0)
//    //    {
//    //        // All enemies defeated, unlock doors
//    //        roomComponent.UnlockAllDoors();
//    //    }
//    //}

//    //// Call this when new enemies spawn in the room
//    //public void AddEnemy(Enemy enemy)
//    //{
//    //    if (!enemies.Contains(enemy))
//    //    {
//    //        enemies.Add(enemy);
//    //        MonitorEnemy(enemy);
//    //    }
//    //}
//}
