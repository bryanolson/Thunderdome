using input;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class MenuController : MonoBehaviour
{
    private CanvasGroup _mainMenuGroup;
    public bool Interactable = true;
    public bool PauseEnabled = true;
    public Button startButton;
    public Button restartButton;
    private Button _primaryButton;
    private InputController inputController;

    private void Awake()
    {
        inputController = new InputController();
    }

    private void Start()
    {
        _mainMenuGroup = GetComponent<CanvasGroup>();
        GameEvents.current.OnTogglePause += HandleTogglePause;
        GameEvents.current.OnVictory += HandleVictory;
        GameEvents.current.OnPlayerKilled += HandleVictory;
        _primaryButton = startButton;
    }

    private void HandleVictory(int obj)
    {
        HandleVictory();
    }

    private void HandleVictory()
    {
        inputController.Player.Disable();

        _primaryButton = restartButton;
        OnEnable();
    }

    private void OnEnable()
    {
        if (_primaryButton)
        {
            _primaryButton.Select();
        }
    }

    private void HandleTogglePause(bool paused)
    {
        //TODO: Make Bryan Happy
        bool show = paused == PauseEnabled;
        _mainMenuGroup.alpha = show ? 1f : 0f;
        _mainMenuGroup.interactable = Interactable && show;
        _mainMenuGroup.blocksRaycasts = Interactable && show;
    }
}