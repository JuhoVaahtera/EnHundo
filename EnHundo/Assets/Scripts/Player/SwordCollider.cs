using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    private Collider2D swordCollider;
    public int damage = 10;
    public float knockbackForce = 5f; // Adjust this value for knockback strength

    void Start()
    {
        swordCollider = GetComponent<Collider2D>();
        swordCollider.enabled = false; // Initially disable the collider
        swordCollider.isTrigger = true; // Ensure the collider is a trigger
    }

    public void EnableSwordCollider()
    {
        swordCollider.enabled = true;
        Debug.Log("Sword Collider Enabled"); // Add a log for debugging
    }

    public void DisableSwordCollider()
    {
        swordCollider.enabled = false;
        Debug.Log("Sword Collider Disabled"); // Add a log for debugging
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Apply damage to the enemy
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);

                // Apply knockback to the enemy
                Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}
