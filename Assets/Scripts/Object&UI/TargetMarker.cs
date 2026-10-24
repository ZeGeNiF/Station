using UnityEngine;

public class TargetMarker : MonoBehaviour
{
    [SerializeField] private GameObject markerObject;

    private void Awake()
    {
        if (markerObject != null)
            markerObject.SetActive(false);
    }

    public void SetSelected(bool selected)
    {
        if (markerObject != null)
            markerObject.SetActive(selected);
    }
}
