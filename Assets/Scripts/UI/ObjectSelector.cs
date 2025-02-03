using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ObjectSelector : MonoBehaviour
{
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [SerializeField] private List<Transform> objects = new List<Transform>(); // 3D objects
    [SerializeField] private RawImage renderImage; // UI element to display Render Texture
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private float maxOffset = 5f;
    [SerializeField] private float moveDuration = 0.3f;
    [SerializeField] private float depthOffset = 2f; // Distance from the camera

    [Header("Lighting")]
    [SerializeField] private LightType lightType = LightType.Spot; // Choose between Directional, Point, or Spot
    [SerializeField] private Color lightColor = Color.white;
    [SerializeField] private float lightIntensity = 3f;
    [SerializeField] private Vector3 lightOffset = new Vector3(0, 0, -1);
    [SerializeField] private Vector3 lightRotationOffset = new Vector3(0, 0, 0);
    [SerializeField] private float lightAngle = 40f;
    [SerializeField] private float lightRange = 10f;
    private Light selectorLight;

    [Header("Selection Events")]
    [SerializeField] private UnityEvent<GameObject> onSelectionChanged;

    private int selectedIndex = 0;
    private bool isMoving = false;
    private Camera selectorCamera;
    private RenderTexture renderTexture;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (selectorCamera == null) return;

        UpdateLightProperties();
        UpdateObjectPositions();
        UpdateScales();
    }
#endif

    private void OnEnable()
    {
        leftButton.onClick.AddListener(ShiftLeft);
        rightButton.onClick.AddListener(ShiftRight);
    }

    private void OnDisable()
    {
        leftButton.onClick.RemoveListener(ShiftLeft);
        rightButton.onClick.RemoveListener(ShiftRight);
    }

    private void Start()
    {
        CreateSelectorLight();
        CreateRenderCamera();
        UpdateLightProperties();
        UpdateObjectPositions();
        UpdateScales();
    }

    private void Update()
    {
        UpdateScales();
    }

    private void CreateRenderCamera()
    {
        // Create a new Camera dynamically
        GameObject camObj = new GameObject("SelectorCamera");
        selectorCamera = camObj.AddComponent<Camera>();

        // Configure camera properties
        selectorCamera.clearFlags = CameraClearFlags.SolidColor;
        selectorCamera.backgroundColor = Color.clear;
        selectorCamera.cullingMask = LayerMask.GetMask("SelectableObjects"); // Only render the selection
        selectorCamera.orthographic = false;
        selectorCamera.nearClipPlane = 0.1f;
        selectorCamera.farClipPlane = 10f;
        selectorCamera.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);

        // Create Render Texture
        renderTexture = new RenderTexture(512, 512, 16);
        selectorCamera.targetTexture = renderTexture;

        // Assign Render Texture to UI
        if (renderImage != null)
        {
            renderImage.texture = renderTexture;
        }
        else
        {
            Debug.LogWarning("Render Image not assigned!");
        }
    }

    private void CreateSelectorLight()
    {
        if (selectorLight == null)
        {
            GameObject lightObj = new GameObject("SelectorLight");
            selectorLight = lightObj.AddComponent<Light>();
        }

        UpdateLightProperties();
    }

    private void UpdateLightProperties()
    {
        if (selectorLight == null || selectorCamera == null) return;

        selectorLight.type = lightType;
        selectorLight.color = lightColor;
        selectorLight.intensity = lightIntensity;
        selectorLight.range = lightRange;
        selectorLight.spotAngle = lightAngle;

        selectorLight.transform.position = selectorCamera.transform.position + lightOffset;
        selectorLight.transform.rotation = selectorCamera.transform.rotation * Quaternion.Euler(lightRotationOffset);
    }

    public void ShiftRight() => ShiftSelection(1);
    public void ShiftLeft() => ShiftSelection(-1);

    private void ShiftSelection(int direction)
    {
        if (isMoving) return;

        selectedIndex = (selectedIndex + direction + objects.Count) % objects.Count;

        onSelectionChanged.Invoke(objects[selectedIndex].gameObject);

        StopAllCoroutines();
        StartCoroutine(MoveObjects());
    }

    private IEnumerator MoveObjects()
    {
        isMoving = true;

        Vector3[] startPositions = new Vector3[objects.Count];
        Vector3[] targetPositions = new Vector3[objects.Count];

        for (int i = 0; i < objects.Count; i++)
        {
            startPositions[i] = objects[i].position;
            targetPositions[i] = CalculateObjectPosition(i);
        }

        float elapsedTime = 0;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsedTime / moveDuration);

            for (int i = 0; i < objects.Count; i++)
                objects[i].position = Vector3.Lerp(startPositions[i], targetPositions[i], t);

            yield return null;
        }

        for (int i = 0; i < objects.Count; i++)
            objects[i].position = targetPositions[i];

        UpdateScales();
        isMoving = false;
    }

    private void UpdateObjectPositions()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].position = CalculateObjectPosition(i);
        }
    }

    private Vector3 CalculateObjectPosition(int index)
    {
        // Center objects in front of the camera
        float offsetX = (index - selectedIndex) * maxOffset;
        return selectorCamera.transform.position + selectorCamera.transform.right * offsetX + selectorCamera.transform.forward * depthOffset;
    }

    private void UpdateScales()
    {
        foreach (Transform obj in objects)
        {
            float offset = Mathf.Abs(obj.position.x - selectorCamera.transform.position.x);
            float normalizedOffset = Mathf.Clamp01(offset / maxOffset);
            float scaleFactor = scaleCurve.Evaluate(normalizedOffset);
            obj.localScale = Vector3.one * scaleFactor;
        }
    }
}
