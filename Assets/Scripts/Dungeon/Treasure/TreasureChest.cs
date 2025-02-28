using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    //public GameObject coinPrefab;
    //public int coinCount = 5;
    //[SerializeField] private float explosionRadius = 2f;
    //[SerializeField] private float explosionForce = 5f;
    //public float magnetDelay = 1.0f;
    //public SpriteRenderer hintText;

    //private bool isOpened = false;
    //private bool playerNearby = false;

    //private void Start()
    //{
    //    if (hintText != null) hintText.gameObject.SetActive(false);
    //}

    //private void Update()
    //{
    //    if (playerNearby && !isOpened && Input.GetKeyDown(KeyCode.F))
    //    {
    //        StartCoroutine(OpenChest());
    //    }
    //}

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    Debug.Log("Tag " + other.CompareTag("Player"));
    //    Debug.Log("null? " + hintText != null);
    //    if (other.CompareTag("Player") && hintText != null)
    //    {
    //        hintText.gameObject.SetActive(true);
    //        playerNearby = true;
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player") && hintText != null)
    //    {
    //        hintText.gameObject.SetActive(false);
    //        playerNearby = false;
    //    }
    //}

    //private IEnumerator OpenChest()
    //{
    //    isOpened = true; // Prevent reopening
    //    hintText.gameObject.SetActive(false); // Hide the hint

    //    for (int i = 0; i < coinCount; i++)
    //    {
    //        // Step 1: Spawn the coin at the chest's position
    //        GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
    //        Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();

    //        if (rb != null)
    //        {
    //            rb.velocity = Vector2.zero; // Reset any existing velocity
    //            rb.isKinematic = false; // Ensure physics is enabled

    //            // Step 2: Generate a random explosion direction within the explosion radius
    //            Vector2 randomDirection = Random.insideUnitCircle.normalized;
    //            float randomDistance = Random.Range(explosionRadius * 0.5f, explosionRadius);
    //            Vector2 targetPosition = (Vector2)transform.position + (randomDirection * randomDistance);

    //            // Step 3: Move coin using Rigidbody interpolation
    //            StartCoroutine(MoveCoinToPosition(rb, targetPosition));

    //            // Step 4: Delay magnet activation
    //            Coin coinScript = coin.GetComponent<Coin>();
    //            if (coinScript != null)
    //            {
    //                coinScript.EnableMagnetAfterDelay(magnetDelay);
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogError("Coin prefab is missing Rigidbody2D!");
    //        }
    //    }

    //    yield return null;


    //}
}
