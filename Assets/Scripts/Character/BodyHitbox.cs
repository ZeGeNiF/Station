using UnityEngine;

public class BodyHitbox : MonoBehaviour
{
    [SerializeField] private string partName;

    private PlayerHealth playerHealth;

    public string PartName => partName;

    private void Awake()
    {
        playerHealth = GetComponentInParent<PlayerHealth>();

        // Если имя не задано вручную,
        // берём его из названия объекта.
        if (string.IsNullOrEmpty(partName))
        {
            partName = gameObject.name.Replace("Hitbox", "");
        }
    }

    public void Initialize(string nameOfPart)
    {
        partName = nameOfPart;
        playerHealth = GetComponentInParent<PlayerHealth>();
    }

    public void TakeHit(float damage)
    {
        if (playerHealth == null)
            playerHealth = GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError("Не найдена PlayerHealth: " + gameObject.name);
            return;
        }

        if (string.IsNullOrEmpty(partName))
        {
            Debug.LogError("Не задано имя части тела: " + gameObject.name);
            return;
        }

        playerHealth.TakeDamage(partName, damage);
    }

    [ContextMenu("Тест: получить 10 урона")]
    private void TestHit()
    {
        TakeHit(10f);
    }
}
