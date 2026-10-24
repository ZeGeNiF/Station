using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MedkitInventory : MonoBehaviour
{
    [Header("Настройки аптечек")]
    [SerializeField] private int maxMedkits = 3;
    [SerializeField] private int medkits = 0;

    [Header("Интерфейс")]
    [SerializeField] private TMP_Text medkitText;

    [Header("Лечение")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Key healKey = Key.H;

    private void Start()
    {
        medkits = Mathf.Clamp(medkits, 0, maxMedkits);
        UpdateUI();
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current[healKey].wasPressedThisFrame)
        {
            UseMedkit();
        }
    }

    public bool AddMedkit()
    {
        if (medkits >= maxMedkits)
        {
            Debug.Log("Инвентарь аптечек полон.");
            return false;
        }

        medkits++;
        UpdateUI();

        Debug.Log("Аптечка подобрана. Колличество: " + medkits);

        return true;
    }

    private void UseMedkit()
    {
        if (playerHealth == null)
        {
            Debug.LogWarning("Не назначен PlayerHealth.", this);
            return;
        }

        if (medkits <= 0)
        {
            Debug.Log("Нет аптечек.");
            return;
        }

        if (!playerHealth.HealAllParts())
        {
            Debug.Log("Лечение невозможно: здоровье полное или персонаж погиб.");
            return;
        }

        medkits--;
        UpdateUI();

        Debug.Log("Аптечка использована. Осталось: " + medkits);
    }

    private void UpdateUI()
    {
        if (medkitText != null)
        {
            medkitText.text = medkits + "/" + maxMedkits;
        }
    }
}
