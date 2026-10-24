using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Размеры hitbox")]
    [SerializeField] private Vector3 headSize = new Vector3(0.22f, 0.25f, 0.22f);
    [SerializeField] private Vector3 torsoSize = new Vector3(0.42f, 0.65f, 0.30f);
    [SerializeField] private Vector3 armSize = new Vector3(0.20f, 0.55f, 0.20f);
    [SerializeField] private Vector3 legSize = new Vector3(0.24f, 0.65f, 0.24f);

    [ContextMenu("Создать hitbox частей тела")]
    public void CreateHitboxes()
    {
        Animator animator = GetComponent<Animator>();

        if (animator == null || !animator.isHuman)
        {
            Debug.LogError("Нужен Animator с настроенным Humanoid Avatar.", this);
            return;
        }

        // Не создаём копии при повторном запуске.
        Transform oldRoot = transform.Find("BodyHitboxes");
        if (oldRoot != null)
            DestroyImmediate(oldRoot.gameObject);

        GameObject root = new GameObject("BodyHitboxes");
        root.transform.SetParent(transform, false);

        CreateHitbox(root.transform, animator, HumanBodyBones.Head,
            "Head", headSize);

        CreateHitbox(root.transform, animator, HumanBodyBones.Spine,
           "Torso", torsoSize);

        CreateHitbox(root.transform, animator, HumanBodyBones.LeftUpperArm,
           "LeftArm", armSize);

        CreateHitbox(root.transform, animator, HumanBodyBones.RightUpperArm,
           "RightArm", armSize);

        CreateHitbox(root.transform, animator, HumanBodyBones.LeftUpperLeg,
           "LeftLeg", legSize);

        CreateHitbox(root.transform, animator, HumanBodyBones.RightUpperLeg,
           "RightLeg", legSize);
    }

    private void CreateHitbox(
        Transform root,
        Animator animator, 
        HumanBodyBones boneName,
        string partName,
        Vector3 size)
    {
        Transform bone = animator.GetBoneTransform(boneName);

        if(bone == null)
        {
            Debug.LogWarning("Не найдена кость: " + boneName, this);
        }

        GameObject hitbox = new GameObject(partName + "Hitbox");
        hitbox.transform.SetParent(bone, false);
        hitbox.transform.localPosition = Vector3.zero;
        hitbox.transform.localRotation = Quaternion.identity;

        BoxCollider collider = hitbox.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = size;

        BodyHitbox part = hitbox.AddComponent<BodyHitbox>();
        part.Initialize(partName);
    }
}
