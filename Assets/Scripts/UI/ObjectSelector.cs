using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ObjectSelector : MonoBehaviour
{
    [SerializeField] private LayerMask selectableObjectsMask;
    [SerializeField] private List<Transform> objects = new List<Transform>();

    [Header("UI & Buttons")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI selectionText;
    [SerializeField] private RawImage renderImage; // UI element to display Render Texture
    [SerializeField] private Vector3Int renderFormat = new Vector3Int(512, 512, 16);

    [Header("Movement & Scaling")]
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private float moveDuration = 0.3f;

    [Header("View & Virtual Camera")]
    [SerializeField] private float fieldOfView = 60f;
    [SerializeField] private float maxOffset = 5f;
    [Tooltip("Distance of the objects from the camera")]
    [SerializeField] private float depthOffset = 2f;

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
    [SerializeField] private UnityEvent<GameObject> onSelectionChangedGameObject;
    [SerializeField] private UnityEvent<int> onSelectionChangedInt;

    private int selectedIndex = 0;
    private bool isMoving = false;
    private Camera selectorCamera;
    private RenderTexture renderTexture;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Configure render texture image
        if (renderImage && renderImage.TryGetComponent(out AspectRatioFitter fitter))
        {
            fitter.aspectRatio = (float)renderFormat[0] / renderFormat[1];
        }

        if (selectorCamera == null) return;

        UpdateLightProperties();
        UpdateObjectPositions();
        UpdateCameraProperties();
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
        UpdateCameraProperties();
        ShiftSelection(0);
        UpdateObjectPositions();
    }

    private void Update()
    {
        if (!isMoving) return;

        UpdateScales();
    }

    private void CreateRenderCamera()
    {
        // Create a new Camera dynamically
        GameObject camObj = new GameObject("SelectorCamera");
        selectorCamera = camObj.AddComponent<Camera>();
    }

    private void UpdateCameraProperties()
    {
        // Configure camera properties
        selectorCamera.fieldOfView = fieldOfView;
        selectorCamera.clearFlags = CameraClearFlags.SolidColor;
        selectorCamera.backgroundColor = Color.clear;
        selectorCamera.cullingMask = selectableObjectsMask; // Only render the selection
        selectorCamera.orthographic = false;
        selectorCamera.nearClipPlane = 0.1f;
        selectorCamera.farClipPlane = 10f;
        selectorCamera.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);

        // Create Render Texture
        renderTexture = new RenderTexture(renderFormat[0], renderFormat[1], renderFormat[2]);
        selectorCamera.targetTexture = renderTexture;

        // Assign Render Texture to UI
        if (renderImage != null)
        {
            renderImage.texture = renderTexture;
            if (renderImage && renderImage.TryGetComponent(out AspectRatioFitter fitter))
            {
                fitter.aspectRatio = (float)renderFormat[0] / renderFormat[1];
            }
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
        selectionText.text = objects[selectedIndex].name;
        onSelectionChangedGameObject.Invoke(objects[selectedIndex].gameObject);
        onSelectionChangedInt.Invoke(selectedIndex);

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
