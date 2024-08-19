using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode shootKey = KeyCode.Space;

    private GameManager _gm;
    private UIManager _ui;

    private bool _inputAllowed;
    public bool InputAllowed
    {
        get { return _inputAllowed; }
        set { _inputAllowed = value; }
    }

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        _gm = GameManager.instance;
        _ui = UIManager.instance;
    }

    public bool IsPressed(KeyCode keyCode)
    {
        if (!InputAllowed) return false;
        return Input.GetKey(keyCode);
    }
}
