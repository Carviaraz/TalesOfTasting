using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, ITakeDamage
{
    [SerializeField] private float health;

    private SpriteRenderer spriteRenderer;
    private float enemyHealth;
    private Color initialColor;
    private Coroutine colorCoroutine;

    public delegate void DeathEventHandler(GameObject deadEnemy);
    public event DeathEventHandler OnDeath;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        enemyHealth = health;
        initialColor = spriteRenderer.color;
    }

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        Debug.Log("enemyHealth = " +enemyHealth);
        ShowDamageColor();
        if (enemyHealth <= 0)
        {
            OnDeath?.Invoke(gameObject);
            Destroy(gameObject);
        }
    }

    private void ShowDamageColor() 
    {
        if (colorCoroutine != null)
        {
            StopCoroutine(colorCoroutine);
        }

        colorCoroutine = StartCoroutine(IETakeDamage());
    }

    private IEnumerator IETakeDamage()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(.1f);
        spriteRenderer.color = initialColor;
    }
}
