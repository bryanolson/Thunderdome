using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    private float _alpha = 0.0f;
    private float _alphaDesired = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.OnLevelVictory += HandleLevelVictory;
        GameEvents.current.OnBuyRound += HandleBuyToggle;
    }

    private void Update()
    {
        if (_alpha != _alphaDesired)
        {
            _alpha = Mathf.MoveTowards(_alpha, _alphaDesired, Time.deltaTime);
        }

        this.GetComponent<CanvasGroup>().alpha = _alpha;
    }

    void HandleBuyToggle(bool buybool)
    {
        _alphaDesired = 0.0f;
    }

    void HandleLevelVictory()
    {
        _alphaDesired = 1.0f;
    }
}