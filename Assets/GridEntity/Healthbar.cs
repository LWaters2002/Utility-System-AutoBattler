using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Slider slider;

    private float lerpTime;
    private float _targetValue;

    public void Init(GridEntity entity)
    {
            entity.OnHealthChange += UpdateSlider;
        }

    private void UpdateSlider(float health, float maxHealth)
    {
        _targetValue = health / maxHealth;
        StartCoroutine(LerpSliderValue());
    }

    IEnumerator LerpSliderValue()
    {
        float t = 0;
        float startValue = slider.value;

        while (t < lerpTime)
        {
            t += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, _targetValue, t / lerpTime);
            yield return null;
        }

        slider.value = _targetValue;
    }
}
