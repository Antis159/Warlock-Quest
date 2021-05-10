using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSummonParticles : MonoBehaviour
{
    private float timer;
    private void Start()
    {
        transform.position -= new Vector3(0, -1f, 0);
    }
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= .8f)
            Destroy(gameObject);
    }
}
