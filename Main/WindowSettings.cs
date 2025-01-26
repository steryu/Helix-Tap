using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Window Settings for pc to make it phone sized */

public class WindowSettings : MonoBehaviour
{
    void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        Screen.SetResolution(520, 1000, false);
#endif
    }
}
