using input;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class MenuSwitcher : MonoBehaviour
    {
        public bool _tutorialBool { get; private set; }
        public bool _creditBool { get; private set; }

        private InputController inputController;

        private void Awake()
        {
            inputController = new InputController();
        }

        private void Start()
        {
            GameEvents.current.OnPlayerKilled += TriggerTogglePause;
            GameEvents.current.OnPlayerKilled += HandleVictoryDefeat;
        }

        private void OnEnable()
        {
            inputController.Player.Pause.performed += InputTogglePause;
            inputController.Player.Pause.Enable();
        }

        private void OnDisable()
        {
            inputController.Player.Pause.performed -= InputTogglePause;
        }

        private void InputTogglePause(InputAction.CallbackContext inputContext)
        {
            if (GameState.current.PlayerAlive)
            {
                TriggerTogglePause();
            }
        }

        public void TriggerTogglePause()
        {
            GameEvents.current.TogglePause(!GameState.current.Paused);
        }

        public void RestartGame()
        {
            GameEvents.current.RestartGame();
        }

        //Copied from M1, but there's really only one way to do this...
        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }

        public void HandleVictoryDefeat()
        {
            if (_tutorialBool)
            {
                ToggleTutorial();
            }

            if (!_creditBool)
            {
                ToggleCredits();
            }
        }

        public void ToggleTutorial()
        {
            // Switch value of _tutorialBool
            if (!_tutorialBool)
            {
                _tutorialBool = true;
            }
            else if (_tutorialBool)
            {
                _tutorialBool = false;
            }

            // Call GameEvent and pass _tutorialBool
            GameEvents.current.ToggleTutorial(_tutorialBool);
        }

        public void ToggleCredits()
        {
            // Switch value of _tutorialBool
            if (_creditBool == false)
            {
                _creditBool = true;
            }
            else if (_creditBool)
            {
                _creditBool = false;
            }

            // Call GameEvent and pass _tutorialBool
            GameEvents.current.ToggleCredits(_creditBool);
        }
    }
}