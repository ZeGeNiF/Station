using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    [Header("Настройки атаки")]
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private float attackRadius = 0.8f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform player;

    public void DealDamage()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("Не назначен Attack Point у зомби.", this);
            return;
        }

        Vector3 directionToPlayer =
            (player.position - transform.position).normalized;

        float angle = Vector3.Angle(
            transform.forward,
            directionToPlayer
        );

        if (angle > 70f)
            return;

        Collider[] hits = Physics.OverlapSphere(
            attackPoint.position,
            attackRadius
        );

        BodyHitbox closestHitbox = null;
        float closestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            BodyHitbox hitbox = hit.GetComponent<BodyHitbox>();

            if (hitbox == null)
                continue;

            float distance = Vector3.Distance(
                attackPoint.position,
                hit.ClosestPoint(attackPoint.position)
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestHitbox = hitbox;
            }
        }

        if (closestHitbox != null)
        {
            closestHitbox.TakeHit(attackDamage);
        }
        else
        {
            Debug.Log("Атака зомби не попала по хитбоксу.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
