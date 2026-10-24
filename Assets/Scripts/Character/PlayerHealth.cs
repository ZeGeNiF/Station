using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Здоровье частей тела")]
    [SerializeField] private float headHealth = 100f;
    [SerializeField] private float torsoHealth = 100f;
    [SerializeField] private float leftArmHealth = 100f;
    [SerializeField] private float rightArmHealth = 100f;
    [SerializeField] private float leftLegHealth = 100f;
    [SerializeField] private float rightLegHealth = 100f;

    [Header("Здоровье частей тела")]
    [SerializeField] private Image headImage;
    [SerializeField] private Image torsoImage;
    [SerializeField] private Image leftArmImage;
    [SerializeField] private Image rightArmImage;
    [SerializeField] private Image leftLegImage;
    [SerializeField] private Image rightLegImage;

    [Header("Цвета здоровья")]
    [SerializeField] private Color healthyColor = Color.white;

    [Range(0f, 1f)]
    [SerializeField] private float hudAlpha = 0.7f;

    [SerializeField] private Color damagedColor = Color.red;

    private GameOverManager gameOverManager;
    private bool isDead = false;
    public bool IsDead => isDead;

    private void Start()
    {
        UpdateAllColors();
        gameOverManager = FindFirstObjectByType<GameOverManager>();
    }

    public void TakeDamage(string partName, float damage)
    {
        if (isDead || damage <= 0f)
            return;

        switch (partName)
        {
            case "Head":
                headHealth = Mathf.Max(0f, headHealth - damage);
                break;

            case "Torso":
                torsoHealth = Mathf.Max(0f, torsoHealth - damage);
                break;

            case "LeftArm":
                leftArmHealth = Mathf.Max(0f, leftArmHealth - damage);
                break;

            case "RightArm":
                rightArmHealth = Mathf.Max(0f, rightArmHealth - damage);
                break;

            case "LeftLeg":
                leftLegHealth = Mathf.Max(0f, leftLegHealth - damage);
                break;

            case "RightLeg":
                rightLegHealth = Mathf.Max(0f, rightLegHealth - damage);
                break;

            default:
                Debug.LogWarning("Неизвестная часть тела: " + partName);
                return;
        }

        UpdatePartColor(partName);

        Debug.Log(
            "Попадание: " + partName +
            ". Остаток здоровья: " + GetHealth(partName)
        );

        if (partName == "Torso" && torsoHealth <= 0f && !isDead)
        {
            isDead = true;

            Debug.Log("Персонаж погиб: здоровье торса закончилось.");

            if (gameOverManager != null)
            {
                gameOverManager.ShowGameOver();
            }
            else
            {
                Debug.LogWarning("GameOverManager не найден в сцене.");
            }
        }
    }

    public bool HasDamage =>
        headHealth < 100f ||
        torsoHealth < 100f ||
        leftArmHealth < 100f ||
        rightArmHealth < 100f ||
        leftLegHealth < 100f ||
        rightLegHealth < 100f;

    public bool HealAllParts()
    {
        if (isDead || !HasDamage)
            return false;

        headHealth = 100f;
        torsoHealth = 100f;
        leftArmHealth = 100f;
        rightArmHealth = 100f;
        leftLegHealth = 100f;
        rightLegHealth = 100f;

        UpdateAllColors();

        Debug.Log("Аптечка использована: здоровье всех частей тела восставновлено.");

        return true;
    }

    private float GetHealth(string partName)
    {
        switch (partName)
        {
            case "Head": return headHealth;
            case "Torso": return torsoHealth;
            case "LeftArm": return leftArmHealth;
            case "RightArm": return rightArmHealth;
            case "LeftLeg": return leftLegHealth;
            case "RightLeg": return rightLegHealth;
            default: return 0f;
        }
    }

    private void UpdateAllColors()
    {
        UpdatePartColor("Head");
        UpdatePartColor("Torso");
        UpdatePartColor("LeftArm");
        UpdatePartColor("RightArm");
        UpdatePartColor("LeftLeg");
        UpdatePartColor("RightLeg");
    }

    private void UpdatePartColor(string partName)
    {
        float health = GetHealth(partName);
        float helthPercent = Mathf.Clamp01(health / 100f);

        Image targetImage = null;

        switch (partName)
        {
            case "Head": targetImage = headImage; break;
            case "Torso": targetImage = torsoImage; break;
            case "LeftArm": targetImage = leftArmImage; break;
            case "RightArm": targetImage = rightArmImage; break;
            case "LeftLeg": targetImage = leftLegImage; break;
            case "RightLeg": targetImage = rightLegImage; break;
        }

        if (targetImage == null)
            return;
        Color partColor = Color.Lerp(
            damagedColor,
            healthyColor,
            helthPercent
         );

        partColor.a = hudAlpha;
        targetImage.color = partColor;
    }

    [ContextMenu("Тест: урон торсу 25")]
    private void TestTorseDamage()
    {
        TakeDamage("Torso", 25f);
    }
}
