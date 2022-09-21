using UnityEngine;

public class DamageRed : MonoBehaviour
{
    private float _alpha;
    private float _alphaDesired = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.OnPlayerDamaged += HandleDamage;
    }

    private void Update()
    {
        if (_alpha > _alphaDesired)
        {
            _alpha = Mathf.MoveTowards(_alpha, _alphaDesired, Time.deltaTime);
        }

        GetComponent<CanvasGroup>().alpha = _alpha;
    }

    void HandleDamage()
    {
        _alpha = 0.3f;
    }
}