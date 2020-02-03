using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLevel : MonoBehaviour
{
    public static InfoShow _is_;
    public int currentLevel = 0;
    

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
            _is_.ShowInfo(currentLevel);
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
            _is_.HideInfo(currentLevel);
    }
}
