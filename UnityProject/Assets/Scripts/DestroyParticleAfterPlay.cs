using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleAfterPlay : MonoBehaviour
{
    private ParticleSystem ps;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();

    }

    private void Update()
    {
        if(ps)
        {
            if (!ps.IsAlive())
                DestroyImmediate(gameObject);
        }
    }
}
