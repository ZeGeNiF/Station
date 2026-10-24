using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Настройка стрельбы")]
    [SerializeField] private float shotDamage = 25f;
    [SerializeField] private float shotRange = 20;
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

        // Один выстрел на каждое нажатие ЛКМ.
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        // Стрелять можно только с оружием в руках
        // и в режиме огнестрельного оружия.
        if (weaponInventory == null ||
            !weaponInventory.HasWeapon ||
            weaponInventory.CurrentMode != WeaponInventory.CombatMode.Ranged)
        {
            return;
        }

        // Для выстрела необходимо прицелиться во врага.
        if (playerAim == null ||
            !playerAim.IsAiming ||
            playerAim.CurrentTarget == null ||
            playerAim.CurrentTarget.IsDead)
        {
            return;
        }

        // Без патронов выстрел не возможен.
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

        Vector3 origin = transform.position + Vector3.up * shotHeight;

        Gizmos.DrawRay(
            origin,
            transform.forward * shotRange
        );
    }
}
