using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timeValue = 90;
    public TextMeshProUGUI timeText;
    private bool timerActive = false;

    private void OnEnable()
    {
        Actions.GameStart += () => { 
            // Only activate timer in Advanced mode
            timerActive = PlayerPrefs.GetString("Mode") == "Advanced Mode";
            if (!timerActive)
            {
                // Hide timer UI in Classic mode
                timeText.gameObject.SetActive(false);
            }
        };
        Actions.GameStop += () => { timerActive = false; };
    }

    private void Start()
    {
        DisplayTime(timeValue);
    }

    void Update()
    {
        if (timerActive)
        {
            if (timeValue > 0)
            {
                timeValue -= Time.deltaTime;
                DisplayTime(timeValue);
            }
            else
            {
                timeValue = 0;
                DisplayTime(timeValue);
                Actions.GameStop?.Invoke();
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        } 
        else if (timeToDisplay > 0)
        {
            timeToDisplay += 1;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
