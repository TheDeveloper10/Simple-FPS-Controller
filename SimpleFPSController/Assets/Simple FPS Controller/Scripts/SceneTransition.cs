using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Transform hidden, shown;
    private Vector3 aimedPosition;
    private void Start()
    {
        aimedPosition = hidden.position;
        changeScenes = false;

        Diamond.st = this;
    }
    
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, aimedPosition, Time.deltaTime * 3);
        if (changeScenes)
        {
            if (GrapplingHook.DistanceSquared(transform.position, aimedPosition) < .01f)
            {
                SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
                changeScenes = false;
            }
        }
    }

    public void HideTransition()
    {
        aimedPosition = hidden.position;
        changeScenes = false;
    }

    private int nextScene;
    private bool changeScenes = false;
    public void ShowTransition(int nextScene_)
    {
        aimedPosition = shown.position;
        nextScene = nextScene_;
        changeScenes = true;
    }
}
