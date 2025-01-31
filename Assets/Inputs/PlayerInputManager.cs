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
        controls.Player.AccelerateDecelerate.performed += movements.OnMove;
        controls.Player.AccelerateDecelerate.canceled += movements.OnMove;

        controls.Player.TurnLeftRight.performed += movements.OnTurn;
        controls.Player.TurnLeftRight.canceled += movements.OnTurn;

        controls.Player.Drift.performed += movements.OnDrift;
        controls.Player.Drift.canceled += movements.OnDrift;

        controls.Player.Item.performed += itemManager.RequestItemUse;
    }
}
