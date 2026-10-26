using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ZombieChase : MonoBehaviour
{
    [Header("Обнаружение игрока")]
    [Header("Атака")]
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float attackDistance = 1.8f;
    [SerializeField] private float attackCooldown = 1.5f;
    private Transform player;
    private PlayerHealth playerHealth;
    private NavMeshAgent agent;
    private Animator animator;
    private ZombieHealth zombieHealth;
    private bool hasDetectedPlayer = false;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        zombieHealth = GetComponent<ZombieHealth>();
    }

    private void Update()
    {
        if (zombieHealth != null && zombieHealth.IsKnockedBack)
            return;

        if (GetComponent<ZombieHealth>()?.IsKnockedBack == true)
            return;

        if (playerHealth != null && playerHealth.IsDead)
        {
            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            if (animator != null)
            {
                animator.ResetTrigger("Attack");
                animator.SetFloat("Speed", 0f);
            }

            return;
        }
        //Ищем игрока, пока не обнаружили его.
        if (!hasDetectedPlayer)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject == null)
                return;

            float distance = Vector3.Distance(
                transform.position,
                playerObject.transform.position
            );

            if (distance <= detectionRadius)
            {
                player = playerObject.transform;
                playerHealth = playerObject.GetComponent<PlayerHealth>();
                hasDetectedPlayer = true;
            }
        }

        // После обнаружения преследуем игрока постоянно.
        if (player != null && agent != null)
        {
            if (agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                float distance = Vector3.Distance(
                    transform.position,
                    player.position
                );

                if (distance > attackDistance)
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                }
                else
                {
                    agent.isStopped = true;

                    if (Time.time >= nextAttackTime)
                    {
                        nextAttackTime = Time.time + attackCooldown;

                        Debug.Log("Зомби атакует игрока!");

                        if (animator != null)
                        {
                            animator.SetTrigger("Attack");
                        }
                    }
                }
            }
        }

        //Передаём скорость в Animator для Idle и Walk.
        if (animator != null && agent != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
        }
    }
}