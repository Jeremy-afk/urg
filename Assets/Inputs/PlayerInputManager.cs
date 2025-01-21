using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private Movements movements;
    [SerializeField] private ItemManager itemManager;

    private Controls controls;

    private void OnEnable()
    {
        controls.Player.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Awake()
    {
        controls = new Controls();
    }

    private void Start()
    {
        controls.Player.AccelerateDecelerate.performed += movements.AccelerateDecelerate;
        controls.Player.AccelerateDecelerate.canceled += movements.AccelerateDecelerate;

        controls.Player.TurnLeftRight.performed += movements.TurnLeftRight;
        controls.Player.TurnLeftRight.canceled += movements.TurnLeftRight;

        controls.Player.Drift.performed += movements.Drift;
        controls.Player.Drift.canceled += movements.Drift;

        controls.Player.Item.performed += itemManager.RequestItemUse;
    }
}
