using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoDestroyOnLoad : MonoBehaviour
{
    void Start()
    {
        DDOL_manager.AddToDDOL(gameObject);
    }
}
