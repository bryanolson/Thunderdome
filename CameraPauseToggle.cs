using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPauseToggle : MonoBehaviour
{
    private Camera _camera;
    public bool PauseEnabled;

    void Start()
    {
        _camera = GetComponent<Camera>();

        GameEvents.current.OnTogglePause += HandleTogglePause;
    }

    private void HandleTogglePause(bool paused)
    {
        _camera.enabled = paused == PauseEnabled;
    }
}