using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

    public Slider timeSlider;
    public TextMeshProUGUI timeText;

    void Start()
    {
        timeSlider.onValueChanged.AddListener(valueUpdate);
    }

    private void valueUpdate(float value)
    {
        timeText.text = "Time : " + value;
        Time.timeScale = value;
    }

    public void StartGame()
    {
        GameManager.Get().turnManager.StartGame();
    }

}
