using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering.PostProcessing;

public class PillEffects : MonoBehaviour
{
    public static PillEffects mainPillEffects;

    public PostProcessVolume ppVolume;
    private ColorGrading colorGrading;
    private DepthOfField depthOfField;
    
    public PillEffect[] pillEffects;
    private bool isUsingDrug = false;

    [Header("UI")]
    public GameObject pill;
    public RectTransform pillBackground;

    private void Start(){
        ppVolume.profile.TryGetSettings<ColorGrading>(out colorGrading);
        ppVolume.profile.TryGetSettings<DepthOfField>(out depthOfField);

        mainPillEffects = this;
    }

    public int UseDrug(int drugIndex)
    {
        if (drugIndex < 0 || drugIndex >= pillEffects.Length || isUsingDrug)
            return -1;

        StartCoroutine(Drug(drugIndex));
        return 1;
    }

    private IEnumerator Drug(int drugIndex){
        if(Random.value < pillEffects[drugIndex].chanceToHealth / 100.0f){
            PlayerStats.playerStats.UpdateHealth(pillEffects[drugIndex].health);
            yield return null;
        }
        else{
            isUsingDrug = true;
            pill.active = true;
            pillBackground.anchoredPosition = new Vector2(32, pillBackground.anchoredPosition.y);

            WaitForEndOfFrame wfeof = new WaitForEndOfFrame();

            float focusDistance = depthOfField.focusDistance.value;

            float elapsedTime = 0.0f, totalTime = 0.0f, finalTime = pillEffects[drugIndex].duration / 4.0f, normalizedTime = 0.0f;
            // Turning on effects
            do{
                elapsedTime += Time.deltaTime;
                totalTime += Time.deltaTime;
                normalizedTime = elapsedTime / finalTime;

                // Applying the visual effects
                colorGrading.hueShift.value = pillEffects[drugIndex].hueShift * normalizedTime;
                colorGrading.saturation.value = pillEffects[drugIndex].saturation * normalizedTime;
                colorGrading.contrast.value = pillEffects[drugIndex].contrast * normalizedTime;

                depthOfField.focusDistance.value = Mathf.LerpUnclamped(focusDistance, pillEffects[drugIndex].focusDistance, normalizedTime);
                
                pillBackground.anchoredPosition = new Vector2((1.0f - (totalTime / pillEffects[drugIndex].duration)) * 32.0f, pillBackground.anchoredPosition.y);
                yield return wfeof;
            } while(elapsedTime <= finalTime);

            // Mid time
            elapsedTime = 0.0f;
            do{
                elapsedTime += Time.deltaTime;
                totalTime += Time.deltaTime;

                pillBackground.anchoredPosition = new Vector2((1.0f - (totalTime / pillEffects[drugIndex].duration)) * 32.0f, pillBackground.anchoredPosition.y);
                yield return wfeof;
            } while(elapsedTime <= pillEffects[drugIndex].duration / 2.0f);

            // Turning off effects
            elapsedTime = finalTime;
            do{
                elapsedTime -= Time.deltaTime;
                normalizedTime = elapsedTime / finalTime;
                totalTime += Time.deltaTime;

                // Applying the visual effects
                colorGrading.hueShift.value = pillEffects[drugIndex].hueShift * normalizedTime;
                colorGrading.saturation.value = pillEffects[drugIndex].saturation * normalizedTime;
                colorGrading.contrast.value = pillEffects[drugIndex].contrast * normalizedTime;

                depthOfField.focusDistance.value = Mathf.LerpUnclamped(focusDistance, pillEffects[drugIndex].focusDistance, normalizedTime);
                
                pillBackground.anchoredPosition = new Vector2((1.0f - (totalTime / pillEffects[drugIndex].duration)) * 32.0f, pillBackground.anchoredPosition.y);
                yield return wfeof;
            } while(elapsedTime > 0.0f);

            isUsingDrug = false;
            pill.active = false;
            pillBackground.anchoredPosition = new Vector2(32, pillBackground.anchoredPosition.y);
        }
    }
}

[System.Serializable]
public class PillEffect
{
    public float duration = 20.0f; // In seconds

    public float chanceToHealth = 50.0f; // The chance for that drug to heal on taking
    public float health = 30.0f; // The health that this drug heals

    public float hueShift = 10.0f; // Visual effect
    public float saturation = 0.0f; // Visual effect
    public float contrast = 0.0f; // Visual effect
    public float focusDistance = 3.5f;
}