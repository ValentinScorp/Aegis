using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [SerializeField] private Slider _healthBar;
    private void Awake()
    {
        _healthBar = GetComponent<Slider>();
    }
    public void OnHealthChanged(float value, float max)
    {
        _healthBar.maxValue = max;
        _healthBar.value = value;
    }
    public void OnHealthDepleted()
    {
        _healthBar.gameObject.SetActive(false);
    }
}
