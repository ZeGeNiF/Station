using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WeaponInventory : MonoBehaviour
{
    public enum CombatMode
    {
        Melee,
        Ranged
    }

    [Header("Оружие")]
    public bool HasWeapon { get; private set; }

    [SerializeField] private int maxAmmo = 9;
    [SerializeField] private int startingAmmo = 3;

    public int CurrentAmmo { get; private set; }
    public int MaxAmmo => maxAmmo;

    [Header("Режим боя")]
    [SerializeField] private Key switchModeKey = Key.Q;

    public CombatMode CurrentMode { get; private set; } = CombatMode.Melee;

    [Header("Интерфейс")]
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private GameObject ammoIndicator;

    private void Start()
    {
        CurrentAmmo = 0;
        CurrentMode = CombatMode.Melee;
        UpdateUI();
    }

    private void Update()
    {
        if (Keyboard.current[switchModeKey].wasPressedThisFrame)
        {
            SwitchCombatMode();
        }
    }

    private void SwitchCombatMode()
    {
        if (!HasWeapon)
        {
            Debug.Log("Сначала нужно подобрать оружие.");
            return;
        }

        CurrentMode = CurrentMode == CombatMode.Ranged
            ? CombatMode.Melee
            : CombatMode.Ranged;

        Debug.Log("Режим боя: " + (CurrentMode == CombatMode.Ranged
            ? "Огнестрельное оружие" : "Ближний бой"));
    }

    public bool PickupWeapon()
    {
        if (HasWeapon)
            return false;

        HasWeapon = true;
        CurrentAmmo = Mathf.Clamp(startingAmmo, 0, maxAmmo);
        CurrentMode = CombatMode.Ranged;

        UpdateUI();

        Debug.Log("Оружие подобрано! Патроны: " +
                  CurrentAmmo + "/" + maxAmmo);

        return true;
    }

    public bool AddAmmo(int amount = 1)
    {
        if (!HasWeapon || CurrentAmmo >= maxAmmo || amount <= 0)
            return false;

        int previousAmmo = CurrentAmmo;
        CurrentAmmo = Mathf.Min(CurrentAmmo + amount, maxAmmo);

        UpdateUI();

        return CurrentAmmo > previousAmmo;
    }

    public bool TryUseAmmo()
    {
        if (!HasWeapon ||
            CurrentMode != CombatMode.Ranged ||
            CurrentAmmo <= 0)
        {
            return false;
        }

        CurrentAmmo--;
        UpdateUI();
        return true;
    }

    private void UpdateUI()
    {
        if (ammoIndicator != null)
            ammoIndicator.SetActive(HasWeapon);

        if (ammoText == null || !HasWeapon)
            return;

        ammoText.text = CurrentAmmo + "/" + maxAmmo;
    }
}
