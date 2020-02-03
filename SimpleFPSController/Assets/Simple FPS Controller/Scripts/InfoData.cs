using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoData : MonoBehaviour
{
    private static Vector3 zeroVector = Vector3.zero;

    private Vector3 AimedPosition = zeroVector;
    public Transform HiddenPosition, ShownPosition;
    
    private void Update()
    {
        if(AimedPosition != zeroVector)
            transform.position = Vector3.Lerp(transform.position, AimedPosition, Time.deltaTime * 4);
    }

    public void ShowID()
    {
        AimedPosition = ShownPosition.position;
    }

    public void HideID()
    {
        AimedPosition = HiddenPosition.position;
    }
}
