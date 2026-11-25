using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, Controls.IPlayerActions
{
    [SerializeField] public float speed = 10.0f;

    private Controls _controls;
    private Vector2 _moveInput;
    private bool playingCards = false;
    public CameraController cameraController;

    [SerializeField] private Rigidbody _rb;

    private void Awake()
    {
        _controls = new Controls();
        _controls.Player.SetCallbacks(this);
    }

    public void PlayCards(GameObject cards)
    {
        playingCards = true;
        cameraController.PlayCards(cards);
    }

    public void StopCards()
    {
        playingCards = false;
    }

    private void OnEnable()
    {
        _controls.Player.Enable();
    }

    private void OnDisable()
    {
        _controls.Player.Disable();
    }

    void Update()
    {
        if (playingCards) return;
        Vector3 movement = new Vector3(_moveInput.x, 0f, _moveInput.y);
        if(movement.sqrMagnitude > 1f)
        {
            movement.Normalize();
        }

        transform.Translate(movement * (speed * Time.deltaTime));
        //_rb.AddForce(movement*speed);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context) { }
    public void OnInteract(InputAction.CallbackContext context) { }
    public void OnSprint(InputAction.CallbackContext context) { }
}
