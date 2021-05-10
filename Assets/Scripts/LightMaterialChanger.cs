using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightMaterialChanger : MonoBehaviour
{
    private ParticleSystemRenderer[] childParticles;
    public Material lit;
    public Material unlit;


    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "CaveScene")
        {
            GetComponent<SpriteRenderer>().material = unlit;
            if (transform.GetComponentInChildren<ParticleSystemRenderer>())
            {
                childParticles = GetComponentsInChildren<ParticleSystemRenderer>();
                for (int i = 0; i < childParticles.Length; i++)
                {
                    childParticles[i].material = unlit;
                }
            }
        }
        else
        {
            GetComponent<SpriteRenderer>().material = lit;
            if (transform.GetComponentInChildren<ParticleSystemRenderer>())
            {
                childParticles = GetComponentsInChildren<ParticleSystemRenderer>();
                for (int i = 0; i < childParticles.Length; i++)
                {
                    childParticles[i].material = lit;
                }
            }
        }
    }
}
