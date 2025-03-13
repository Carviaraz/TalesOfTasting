using UnityEngine;

public class Prop : MonoBehaviour, ITakeDamage
{
    [SerializeField] private float durability;

    private float counter;

    public void TakeDamage(float damage)
    {
        counter++;
        if (counter >= durability) 
        {
            Destroy(gameObject);
        }
    }
}
