using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Health healthComponent;
    private Slider healthbarSlider;

    private void Awake()
    {
        healthbarSlider = GetComponent<Slider>();
        if (!healthbarSlider || !healthComponent) { gameObject.SetActive(false); }
    }

    private void OnEnable()
    {
        if (healthComponent)
        {
            healthComponent.HealthUpdate += UpdateSlider;
        }
    }

    private void OnDisable()
    {
        if (healthComponent)
        {
            healthComponent.HealthUpdate -= UpdateSlider;
        }
    }

    public void UpdateSlider(int value, int maxValue)
    {
        float slider_value = (float) value / maxValue;
        healthbarSlider.value = slider_value;
    }
}
