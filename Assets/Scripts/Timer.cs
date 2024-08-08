using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    [SerializeField]
    private float totalTime = 10;
    private float currentTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        while (currentTime < totalTime)
        {
            currentTime += Time.deltaTime;
            Debug.Log(currentTime);
            yield return null;
        }
        //stop
        Debug.Log("stop");
    }
}
