using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    [Header("Аниматор")]
    [SerializeField] private Animator animator;

    [Header("Настройки комбо")]
    [SerializeField] private float comboWindow = 0.8f;

    private int comboStep = 0;
    private bool waitingForCombo = false;
    private bool attackRequested = false;
    private bool comboQueued = false;

    private float comboTimer = 0f;
    private bool attackInProgress = false;

    private static readonly int AttackHash =
        Animator.StringToHash("Attack");
    private static readonly int StartPunchHash =
        Animator.StringToHash("StartPunch");
    private static readonly int ComboHash =
        Animator.StringToHash("Combo");
    private WeaponInventory weaponInventory;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        weaponInventory = GetComponent<WeaponInventory>();
    }

    [Header("Урон ближнего боя")]
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackRadius = 0.7f;
    [SerializeField] private float attackHeight = 1.0f;
    [SerializeField] private LayerMask enemyLayers = ~0;

    private readonly HashSet<ZombieHealth> hitZombie = new();

    private void Update()
    {
        if (animator == null || Mouse.current == null)
            return;

        HandleInput();
        HandleComboTimer();
        HandleAnimations();
    }

    private void HandleInput()
    {
        if (weaponInventory != null &&
            weaponInventory.HasWeapon &&
            weaponInventory.CurrentMode == WeaponInventory.CombatMode.Ranged)
        {
            return;
        }
        
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        // Начинаем новое комбо
        if (comboStep == 0 && !attackInProgress)
        {
            comboStep = 1;
            attackInProgress = true;
            attackRequested = true;
            waitingForCombo = true;
            comboQueued = false;
            comboTimer = comboWindow;

            animator.ResetTrigger(ComboHash);
            animator.ResetTrigger(StartPunchHash);
            animator.SetTrigger(AttackHash);
            return;
        }

        // Запоминаем нажатие второго удара
        if (comboStep == 1 && waitingForCombo)
        {
            comboQueued = true;
            waitingForCombo = false;
        }
    }

    private void HandleComboTimer()
    {
        if (!waitingForCombo)
            return;

        comboTimer -= Time.deltaTime;

        if (comboTimer <= 0f)
            waitingForCombo = false;
    }

    private void HandleAnimations()
    {
        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        if (animator.IsInTransition(0))
            return;

        // Переходим от боевой стойки к первому удару
        if (attackRequested && state.IsName("CombatStance"))
        {
            attackRequested = false;
            animator.SetTrigger(StartPunchHash);
            return;
        }

        // Второй удар запускается только из первого
        if (comboQueued && state.IsName("Punching1"))
        {
            comboQueued = false;
            comboStep = 2;
            animator.SetTrigger(ComboHash);
            return;
        }

        // Завершился первый удар без комбо
        if (comboStep == 1 &&
            !waitingForCombo &&
            !comboQueued &&
            state.IsName("CombatStance"))
        {
            ResetCombo();
            return;
        }

        // Завершился второй удар
        if (comboStep == 2 &&
            state.IsName("CombatStance"))
        {
            ResetCombo();
        }
    }

    private void ResetCombo()
    {
        comboStep = 0;
        waitingForCombo = false;
        attackRequested = false;
        comboQueued = false;
        attackInProgress = false;

        animator.ResetTrigger(AttackHash);
        animator.ResetTrigger(StartPunchHash);
        animator.ResetTrigger(ComboHash);
    }

    //Вызывается Animation Event в момент попадания кулака.
    public void Hit()
    {

        Debug.Log("Игрок выполнил проверку попадания!");

        Vector3 hitCenter =
            transform.position +
            Vector3.up * attackHeight +
            transform.forward * attackRange;

        Collider[] hits = Physics.OverlapSphere(
            hitCenter,
            attackRadius,
            enemyLayers
        );

        Debug.Log("Коллайдеров в зоне удара: " + hits.Length);

        foreach (Collider hit in hits)
        {
            ZombieHealth zombie =
                hit.GetComponentInParent<ZombieHealth>();

            if (zombie == null || zombie.IsDead)
                continue;

            // Не наносим урон одному зомби повторно
            // в рамках того же удара.
            if (hitZombie.Add(zombie))
            {
                zombie.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 hitCenter =
            transform.position +
            Vector3.up * attackHeight +
            transform.forward * attackRange;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitCenter, attackRadius);
    }

    // Вызывается перед началом каждого удара.
    public void BeginHit()
    {
        hitZombie.Clear();
    }
}