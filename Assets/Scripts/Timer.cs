using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField]
    private float totalTime = 10;
    private float currentTime = 0;

    public delegate void OnTimeEnd();
    public event OnTimeEnd onTimeEnd;
    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine(StartTimer());
    }

    IEnumerator StartTimerCo()
    {
        currentTime = 0;
        while (currentTime < totalTime)
        {
            currentTime += Time.deltaTime;
            float displayTime = totalTime - currentTime;
            float minutes = Mathf.FloorToInt(displayTime / 60);
            float seconds = Mathf.FloorToInt(displayTime % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            yield return null;
        }
        //stop
        Debug.Log("stop");
        onTimeEnd?.Invoke();
    }

    public void StartTimer()
    {
        StopAllCoroutines();
        StartCoroutine(StartTimerCo());
    }
}
