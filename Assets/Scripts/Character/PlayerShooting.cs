using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Настройка стрельбы")]
    [SerializeField] private float shotDamage = 25f;
    [SerializeField] private float shotRange = 20f;
    [SerializeField] private float shotHeight = 1.2f;

    private WeaponInventory weaponInventory;
    private PlayerAim playerAim;

    private void Awake()
    {
        weaponInventory = GetComponent<WeaponInventory>();
        playerAim = GetComponent<PlayerAim>();
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (weaponInventory == null ||
            !weaponInventory.HasWeapon ||
            weaponInventory.CurrentMode != WeaponInventory.CombatMode.Ranged)
        {
            return;
        }

        if (playerAim == null ||
            !playerAim.IsAiming ||
            playerAim.CurrentTarget == null ||
            playerAim.CurrentTarget.IsDead)
        {
            return;
        }

        if (!weaponInventory.TryUseAmmo())
            return;

        Shoot(playerAim.CurrentTarget);
    }

    private void Shoot(ZombieHealth target)
    {
        Vector3 origin =
            transform.position + Vector3.up * shotHeight;

        Vector3 targetPoint =
            target.transform.position + Vector3.up * 0.8f;

        Vector3 direction =
            (targetPoint - origin).normalized;

        if (Physics.Raycast(
            origin,
            direction,
            out RaycastHit hit,
            shotRange,
            ~0,
            QueryTriggerInteraction.Ignore))
        {
            ZombieHealth hitZombie =
                hit.collider.GetComponentInParent<ZombieHealth>();

            if (hitZombie != null && !hitZombie.IsDead)
            {
                hitZombie.TakeDamage(shotDamage);

                // Отталкиваем только в том случае,
                // если попадание не убило врага.
                if (!hitZombie.IsDead)
                {
                    Vector3 knockbackDirection =
                        hitZombie.transform.position - transform.position;

                    hitZombie.ApplyKnockback(knockbackDirection);
                }

                Debug.Log("Попадание по зомби! Урон: " + shotDamage);
            }
            else
            {
                Debug.Log("Выстрел заблокирован препятствием.");
            }
        }
        else
        {
            Debug.Log("Выстрел не достиг цели.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 origin =
            transform.position + Vector3.up * shotHeight;

        Gizmos.DrawRay(
            origin,
            transform.forward * shotRange
        );
    }
}