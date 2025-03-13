using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [SerializeField] private float rangeDetection;

    public EnemyHealth EnemyTarget { get; set; }

    private CircleCollider2D myCollider2D;
    private List<EnemyHealth> enemyList = new List<EnemyHealth>();

    private void Awake()
    {
        myCollider2D = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        myCollider2D.radius = rangeDetection;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            enemyList.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemyList.Contains(enemy))
            {
                enemyList.Remove(enemy);
            }

            if (enemy == EnemyTarget)
            {
                EnemyTarget = null;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        int n = enemyList.Count;
        for (int i = 0; i < n; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, enemyList[i].transform.position);
        }
    }
}
