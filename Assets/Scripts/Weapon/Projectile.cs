using UnityEngine;
using UnityEngine.EventSystems;

public class Projectile : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed;
    [SerializeField] private bool spin;
    [SerializeField] private float spinSpeed = 360f;

    public Vector3 Direction { get; set; }
    public float Damage { get; set; }

    void Update()
    {
        transform.position += Direction * (speed * Time.deltaTime);

        // Spin the projectile in place
        if (spin)
        {
            transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
        Debug.Log("Projcetile is destroyed");
    }
}
