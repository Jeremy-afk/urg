using UnityEngine;

public class LoadingIcon : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 200.0f;
    [SerializeField] private RectTransform attachedTransform;

    private void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        transform.position = attachedTransform.position;
    }
}
