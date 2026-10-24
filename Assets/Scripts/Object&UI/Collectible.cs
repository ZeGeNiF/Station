using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum CollectibleType
    {
        Medkit,
        Ammo
    }

    [SerializeField] private CollectibleType itemType;

    [Header("Аптечки")]
    [SerializeField] private MedkitInventory medkitInventory;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        switch (itemType)
        {
            case CollectibleType.Medkit:
                PickUpMedkit();
                break;

            case CollectibleType.Ammo:
                PickUpAmmo(other);
                break;
        }
    }

    private void PickUpMedkit()
    {
        if (medkitInventory == null)
        {
            Debug.LogWarning(
                "У аптечки не назначен MedkitInventory.", this);
            return;
        }

        if (medkitInventory.AddMedkit())
        {
            Destroy(gameObject);
        }
    }

    private void PickUpAmmo(Collider other)
    {
        WeaponInventory weaponInventory =
            other.GetComponent<WeaponInventory>();

        if (weaponInventory == null)
        {
            Debug.LogWarning(
                "У игрока не найден WeaponInventory.", this);
            return;
        }

        if (weaponInventory.AddAmmo(1))
        {
            Destroy(gameObject);
            Debug.Log("Подобран 1 патрон.");
        }
    }
}
