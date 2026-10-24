using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private GameObject pickupPrompt;

    private WeaponInventory inventory;
    private bool playerNearby;

    private void Start()
    {
        if (pickupPrompt != null)
            pickupPrompt.SetActive(false);
    }

    private void Update()
    {
        if (!playerNearby || inventory == null)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (inventory.PickupWeapon())
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        inventory = other.GetComponent<WeaponInventory>();
        playerNearby = inventory != null;

        if (pickupPrompt != null && playerNearby)
            pickupPrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNearby = false;
        inventory = null;

        if (pickupPrompt != null)
            pickupPrompt.SetActive(false);
    }

}
