using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Nitrobar : MonoBehaviour
{
    public Nitro nitroComponent;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (!slider || !nitroComponent) { gameObject.SetActive(false); }
    }

    private void OnEnable()
    {
        if (nitroComponent)
        {
            nitroComponent.onNitroUpdated += UpdateSlider;
        }
    }

    private void OnDisable()
    {
        if (nitroComponent)
        {
            nitroComponent.onNitroUpdated -= UpdateSlider;
        }
    }

    public void UpdateSlider(int value, int maxValue)
    {
        float slider_value = (float) value / maxValue;
            slider.value = slider_value;
    }
}
