using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Diamond : MonoBehaviour
{
    public static SceneTransition st;

    public int nextScene = -1;
    private void Update()
    {
        transform.eulerAngles += Vector3.up * 15 * Time.deltaTime;
    }
    
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (nextScene != -1)
            {
                st.ShowTransition(nextScene);
            }
        }
    }
}