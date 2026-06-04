using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    [SerializeField] private Health _health;

    private void OnEnable()
    {
        _health.OnHPChanged += ChangeUI;
    }

    private void OnDisable()
    {
        _health.OnHPChanged -= ChangeUI;
    }

    private void ChangeUI(float percent)
    {
        _slider.value = percent;
    }
}
