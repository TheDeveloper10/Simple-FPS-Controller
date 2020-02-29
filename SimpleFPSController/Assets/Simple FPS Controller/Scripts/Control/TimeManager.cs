using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager mainTimeManager;

    public static float currentTimeScale = 1;

    // The audio that is affected by the slowmotion
    [Header("Audio")]
    public AudioSource background;
    public AudioSource shot;
    public AudioSource noAmmo;

    private void Start(){
        mainTimeManager = this;
    }

    /// <summary>
    /// Sets the time scale smoothly
    /// </summary>
    /// <param name="newTimeScale"> The new time scale </param>
    /// <param name="finalTime"> The time that will take to transform the time scale
    /// to the new one </param>
    public void SetTimeScale(float newTimeScale = 0.0f, float finalTime = 5.0f){
        if(newTimeScale < 0.0f || finalTime <= 0.0f || changingTime)
            return;
        StartCoroutine(TransformTimeScale(newTimeScale, finalTime));
    }
    [HideInInspector] public bool changingTime = false;

    private IEnumerator TransformTimeScale(float newTimeScale = 0.0f, float finalTime = 5.0f){
        changingTime = true;
        float percent = 0.0f, 
            startingTimeScale = currentTimeScale,
            dTime = Time.fixedDeltaTime,
            elapsedTime = 0.0f;
        WaitForEndOfFrame wfeof = new WaitForEndOfFrame();

        do{
            elapsedTime += dTime;
            percent = elapsedTime / finalTime;

            currentTimeScale = Mathf.Lerp(startingTimeScale, newTimeScale, percent);
            background.pitch = shot.pitch = noAmmo.pitch = currentTimeScale;
            Time.timeScale = currentTimeScale;

            yield return wfeof;
        } while(elapsedTime <= finalTime);
        
        background.pitch = shot.pitch = noAmmo.pitch = currentTimeScale;
        Time.timeScale = currentTimeScale = newTimeScale;
        changingTime = false;
    }
}
