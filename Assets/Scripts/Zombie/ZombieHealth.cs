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

    private float currentHealth;
    private Animator animator;
    private NavMeshAgent agent;
    private ZombieChase zombieChase;
    private ZombieAttack zombieAttack;

    public bool IsDead { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;

        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        zombieChase = GetComponent<ZombieChase>();
        zombieAttack = GetComponent<ZombieAttack>();
    }

    public void TakeDamage(float damage)
    {
        if (IsDead || damage <= 0f)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        Debug.Log($"Зомби получил {damage} урона. HP):{ currentHealth}/{ maxHealth}");

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        if (agent != null && agent.isActiveAndEnabled)
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
