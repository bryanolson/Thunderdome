using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class StartGameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.OnTogglePause += SetContinueText;
        GameEvents.current.OnPlayerKilled += Deactivate;
        GameEvents.current.OnVictory += Deactivate;
    }

    private void Deactivate()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    private void Deactivate(int i)
    {
        Deactivate();
    }

    private void SetContinueText(bool paused)
    {
        if (!paused)
        {
            GetComponent<Text>().text = "Continue Game";
        }
    }
}