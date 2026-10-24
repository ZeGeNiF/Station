
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerAim : MonoBehaviour
{
    [Header("Настройки прицеливания")]
    [SerializeField] private float aimRange = 8f;
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Переключение целей")]
    [SerializeField] private float scrollSensitivity = 120f;

    private ZombieHealth currentTarget;
    private bool isAiming;
    private bool aimButtonHeld;

    private readonly List<ZombieHealth> targets = new();
    private float scrollAccumulator;

    public bool IsAiming => isAiming;
    public ZombieHealth CurrentTarget => currentTarget;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            aimButtonHeld = true;
            scrollAccumulator = 0f;

            RefreshTargets();
            SetTarget(FindNearestEnemy());
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            aimButtonHeld = false;
            StopAiming();
            return;
        }

        if (!aimButtonHeld)
            return;

        RefreshTargets();

        if (currentTarget == null || currentTarget.IsDead)
        {
            SetTarget(FindNearestEnemy());
        }

        HandleScrollInput();

        if (isAiming && currentTarget != null)
            RotateTowardsTarget();
    }


    private void HandleScrollInput()
    {
        if (Keyboard.current == null)
            return;

        // X — следующая цель
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            SwitchTarget(true);
        }
        // Z — предыдущая цель
        else if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            SwitchTarget(false);
        }
    }


    private void RefreshTargets()
    {
        ZombieHealth[] enemies =
            FindObjectsByType<ZombieHealth>(
                FindObjectsSortMode.None
            );

        targets.Clear();

        foreach (ZombieHealth enemy in enemies)
        {
            if (enemy == null || enemy.IsDead)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance <= aimRange)
                targets.Add(enemy);
        }

        targets.Sort((a, b) =>
        {
            float distanceA = Vector3.Distance(
                transform.position, a.transform.position
            );

            float distanceB = Vector3.Distance(
                transform.position, b.transform.position
            );

            return distanceA.CompareTo(distanceB);
        });
    }

    private ZombieHealth FindNearestEnemy()
    {
        return targets.Count > 0 ? targets[0] : null;
    }

    private void SwitchTarget(bool next)
    {
        if (targets.Count == 0)
        {
            SetTarget(null);
            return;
        }

        if (targets.Count == 1)
        {
            SetTarget(targets[0]);
            return;
        }

        int index = targets.IndexOf(currentTarget);

        if (index < 0)
        {
            SetTarget(FindNearestEnemy());
            return;
        }

        index = next
            ? (index + 1) % targets.Count
            : (index - 1 + targets.Count) % targets.Count;

        SetTarget(targets[index]);
    }

    private void SetTarget(ZombieHealth newTarget)
    {
        if (currentTarget == newTarget)
        {
            isAiming = currentTarget != null;
            return;
        }

        if (currentTarget != null)
        {
            TargetMarker oldMarker =
                currentTarget.GetComponentInParent<TargetMarker>();

            if (oldMarker != null)
                oldMarker.SetSelected(false);
        }

        currentTarget = newTarget;
        isAiming = currentTarget != null;

        if (currentTarget != null)
        {
            TargetMarker newMarker =
                currentTarget.GetComponentInParent<TargetMarker>();

            if (newMarker != null)
                newMarker.SetSelected(true);
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction =
            currentTarget.transform.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void StopAiming()
    {
        SetTarget(null);
        targets.Clear();
        scrollAccumulator = 0f;
    }

    private void OnDisable()
    {
        if (currentTarget != null)
        {
            TargetMarker marker =
                currentTarget.GetComponentInParent<TargetMarker>();

            if (marker != null)
                marker.SetSelected(false);
        }

        currentTarget = null;
        isAiming = false;
        aimButtonHeld = false;
        targets.Clear();
        scrollAccumulator = 0f;
    }
}
