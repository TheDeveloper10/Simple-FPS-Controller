using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    public static Volume postProcessing;

    public static MotionBlur mBlur;
    public static float defaultMBlurIntensity, currentValueMBlur, aimedIntensityMBlur;

    public static Vignette vignette;
    public static float defaultVignetteIntensity, currentValueVignette, aimedIntensityVignette;

    public static float currentTimeScale = 1;

    // The audio that is affected by the slowmotion
    [Header("Audio")]
    public AudioSource background;
    public AudioSource shot;
    public AudioSource noAmmo;

    public static void SetupPostProcessing()
    {
        postProcessing.profile.TryGet<MotionBlur>(out mBlur);
        currentValueMBlur = aimedIntensityMBlur = defaultMBlurIntensity = mBlur.intensity.value;

        postProcessing.profile.TryGet<Vignette>(out vignette);
        currentValueVignette = aimedIntensityVignette = defaultVignetteIntensity = vignette.intensity.value;
    }

    [HideInInspector] public float timeScaleAim = 1;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Stop Time
            timeScaleAim = .4f;
            vignette.intensity.value = .35f;
            mBlur.intensity.value = .17f;
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            timeScaleAim = 1.005f;
            vignette.intensity.value = defaultVignetteIntensity;
            mBlur.intensity.value = defaultMBlurIntensity;
        }

        if((currentValueVignette - aimedIntensityVignette) * (currentValueVignette - aimedIntensityVignette) > .001f)
        {
            currentValueVignette = Mathf.Lerp(currentValueVignette, aimedIntensityVignette, Time.deltaTime * 2);
            vignette.intensity.value = currentValueVignette;
        }

        if ((currentValueMBlur - aimedIntensityMBlur) * (currentValueMBlur - aimedIntensityMBlur) > .001f)
        {
            currentValueMBlur = Mathf.Lerp(currentValueMBlur, aimedIntensityMBlur, Time.deltaTime * 2);
            mBlur.intensity.value = currentValueMBlur;
        }

        if ((currentTimeScale - timeScaleAim) * (currentTimeScale - timeScaleAim) > .00001f)
        {
            currentTimeScale = Mathf.Lerp(currentTimeScale, timeScaleAim, Time.deltaTime * 7);
            background.pitch = shot.pitch = noAmmo.pitch = currentTimeScale;

            Time.timeScale = currentTimeScale;
        }
    }
}
