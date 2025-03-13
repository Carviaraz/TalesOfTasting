using UnityEngine;
using UnityEngine.EventSystems;

public class Projectile : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed;
    [SerializeField] private bool spin;
    [SerializeField] private float spinSpeed = 360f;
    [SerializeField] private LayerMask targetLayer;
    //public float Damage;

    public Vector3 Direction { get; set; }
    public float Damage { get; set; }

    void Update()
    {
        transform.position += Direction * (speed * Time.deltaTime);

        if (spin)
        {
            transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.GetComponent<ITakeDamage>() != null)
        //{
        //    collision.GetComponent<ITakeDamage>().TakeDamage(1f);
        //}

        //Destroy(gameObject);
        //Debug.Log("Projcetile is destroyed");

        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            ITakeDamage damageable = collision.GetComponent<ITakeDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(1f);
            }

            Destroy(gameObject);
            Debug.Log("Projectile hit " + collision.name);
        }
    }
}
