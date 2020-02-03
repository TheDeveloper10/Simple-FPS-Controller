using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoShow : MonoBehaviour
{
    public InfoData[] infos;
    
    private void Start()
    {
        NewLevel._is_ = this;
    }

    public void ShowInfo(int n)
    {
        infos[n].ShowID();
    }

    public void HideInfo(int n)
    {
        infos[n].HideID();
    }
}
