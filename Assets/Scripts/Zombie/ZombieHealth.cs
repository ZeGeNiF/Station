using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : MonoBehaviour
{
    [Header("Здоровье")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Смерть")]
    [SerializeField] private string deathTrigger = "Die";
    [SerializeField] private float deathAnimationDuration = 2f;
    [SerializeField] private bool desroyAfterDearh = true;

    [Header("Отталкивание от выстрелов")]
    [SerializeField] private float knockbackImpulse = 3f;
    [SerializeField] private float knockbackDuration = 0.25f;
    [SerializeField] private float navMeshSearchRadius = 5f;

    private float currentHealth;
    private Animator animator;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private ZombieChase zombieChase;
    private ZombieAttack zombieAttack;
    private Coroutine knockbackCoroutine;
    private PhysicsExperimentRecorder experimentRecorder;

    public bool IsDead { get; private set; }
    public bool IsKnockedBack { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;

        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        zombieChase = GetComponent<ZombieChase>();
        zombieAttack = GetComponent<ZombieAttack>();
        experimentRecorder = GetComponent<PhysicsExperimentRecorder>();
    }

    public void TakeDamage(float damage)
    {
        if (IsDead || damage <= 0f)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        Debug.Log($"Зомби получил {damage} урона. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
            Die();
    }

    public void ApplyKnockback(Vector3 direction)
    {
        if (IsDead || rb == null)
            return;

        // Если враг уже отталкивается, перезапускаем реакцию.
        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);

        knockbackCoroutine = StartCoroutine(
            KnockbackRoutine(direction)
        );
    }

    private IEnumerator KnockbackRoutine(Vector3 direction)
    {
        IsKnockedBack = true;
        if (experimentRecorder != null)
            experimentRecorder.StartRecording();

        // Останавливаем управление движением через NavMesh.
        if (agent != null && agent.enabled)
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            agent.enabled = false;
        }

        // Не даём врагу наклоняться от физического импульса.
        rb.constraints |= RigidbodyConstraints.FreezeRotation;

        // Временно разрешаем физическое движение.
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;

        // Отталкиваем только по горизонтали.
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            direction.Normalize();
            rb.AddForce(
                direction * knockbackImpulse,
                ForceMode.Impulse
            );
        }

        if (animator != null)
            animator.SetFloat("Speed", 0f);

        yield return new WaitForSeconds(knockbackDuration);
        if (experimentRecorder != null)
            experimentRecorder.StartRecording();

        if (IsDead)
            yield break;

        // Ищем ближайшую точку на NavMesh.
        Vector3 currentPosition = rb.position;

        bool foundNavMesh = NavMesh.SamplePosition(
            currentPosition,
            out NavMeshHit navHit,
            navMeshSearchRadius,
            agent != null ? agent.areaMask : NavMesh.AllAreas
        );

        // Возвращаем объект к управлению ИИ.
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.constraints &= ~RigidbodyConstraints.FreezeRotation;

        if (foundNavMesh)
        {
            transform.position = navHit.position;

            if (agent != null)
            {
                agent.enabled = true;
                agent.Warp(navHit.position);
                agent.isStopped = false;
            }
        }
        else
        {
            Debug.LogWarning(
                "Не удалось найти NavMesh рядом с зомби после отталкивания.",
                this
            );

            // Без найденной точки не включаем агент вне NavMesh.
            if (agent != null)
                agent.enabled = false;
        }

        IsKnockedBack = false;
        knockbackCoroutine = null;
    }

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;
        if (experimentRecorder != null)
            experimentRecorder.StartRecording();
        IsKnockedBack = false;

        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
            knockbackCoroutine = null;
        }

        if (rb != null)
        {
            if (!rb.isKinematic)
                rb.linearVelocity = Vector3.zero;

            rb.isKinematic = true;
        }

        if (agent != null && agent.enabled)
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            agent.enabled = false;
        }

        if (zombieChase != null)
            zombieChase.enabled = false;

        if (zombieAttack != null)
            zombieAttack.enabled = false;

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger(deathTrigger);
        }
        else
        {
            Debug.LogWarning("У зомби не найден Animator.", this);
        }

        if (desroyAfterDearh)
            StartCoroutine(RemoveAfterDeath());
    }

    private IEnumerator RemoveAfterDeath()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        Destroy(gameObject);
    }
}