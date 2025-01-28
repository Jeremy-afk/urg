using System.Collections;
using UnityEngine;

public class RotationAnimation : MonoBehaviour
{
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float duration = 1f;
    [SerializeField] private uint loops = 0;
    [SerializeField] private bool infinite = false;
    [SerializeField] private bool rotateOnAwake = false;

    private bool isRotating = false;

    private void Start()
    {
        isRotating = rotateOnAwake;
    }

    public void SetDuration(float duration)
    {
        this.duration = duration;
    }

    public void Rotate()
    {
        StopAllCoroutines();
        isRotating = true;
        StartCoroutine(RotateCoroutine());
    }

    public void StopRotation()
    {
        isRotating = false;
    }

    private IEnumerator RotateCoroutine()
    {
        float elapsedTime = 0;
        float speed = loops * 360 / duration;

        while (isRotating && (infinite || elapsedTime < duration))
        {
            transform.Rotate(rotationAxis, speed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
