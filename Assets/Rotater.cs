using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.rotateAroundLocal(gameObject, Vector3.left, 360f, 3f).setRepeat(-1);

    }

}
