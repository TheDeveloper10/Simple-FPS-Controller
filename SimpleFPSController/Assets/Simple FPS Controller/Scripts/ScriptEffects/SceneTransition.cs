using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public PostProcessVolume ppVolume;
    private ColorGrading colorGrading;

    [Header("Transition Settings")]
    public float sceneTransitionTime = 4.0f;

    public static SceneTransition mainSceneTransition;

    private void Awake()
    {
        mainSceneTransition = this;
        ppVolume.profile.TryGetSettings<ColorGrading>(out colorGrading);
        StartCoroutine(ShowScene());
    }

    IEnumerator ShowScene()
    {
        float t = sceneTransitionTime / 2.0f;
        do
        {
            t -= Time.deltaTime;
            colorGrading.postExposure.value = -t * 10;
            yield return new WaitForEndOfFrame();
        } while (t > 0.0f);
        colorGrading.postExposure.value = 0;
    }

    IEnumerator HideScene()
    {
        float t = 0.0f, nT = sceneTransitionTime / 2.0f;
        do
        {
            t += Time.deltaTime;
            colorGrading.postExposure.value = -t * 10;
            yield return new WaitForEndOfFrame();
        } while (t < nT);
        colorGrading.postExposure.value = 10;

        SceneManager.LoadScene(newSceneIndex_, LoadSceneMode.Single);
    }
    
    private int newSceneIndex_ = 0;
    /// <summary>
    /// This method switches between scenes. You can point to which scene
    /// you want it to switch by setting value to newSceneIndex.
    /// If the newSceneIndex is equal to -1 it means that you want the active scene.
    /// </summary>
    public void ChangeScene(int newSceneIndex)
    {
        if(newSceneIndex == -1)
            newSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if(newSceneIndex < 0 || newSceneIndex > SceneManager.sceneCountInBuildSettings)
            return;

        newSceneIndex_ = newSceneIndex;
        StartCoroutine(HideScene());
    }
}
