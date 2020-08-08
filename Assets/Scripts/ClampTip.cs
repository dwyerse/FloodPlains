using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
public class ClampTip : MonoBehaviour
{   
    public Camera cam;
    public GameObject text;
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = cam.WorldToScreenPoint(transform.position);
        text.transform.position = pos;
        
    }
}
