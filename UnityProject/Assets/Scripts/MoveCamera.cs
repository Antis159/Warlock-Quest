using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float cameraSpeed = 4f;
    private PlayerControl Player;
    private Vector3 offset = new Vector3(0,0,-10f);
    public bool combat;
    public Cinemachine.CinemachineConfiner cameraChild;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        if (cameraChild == null)
            cameraChild = transform.GetChild(0).gameObject.GetComponent<Cinemachine.CinemachineConfiner>();
    }
    void LateUpdate()
    {
        if(Player != null)
        {
            if(!Player.inCombat)
            {
                transform.position = Vector3.MoveTowards(transform.position, Player.transform.position + offset, cameraSpeed * Time.deltaTime);
            }
        }
        if(combat)
        {
            StartCoroutine(MoveCameraOnCombat());
            combat = false;
        }

        if (cameraChild.m_BoundingShape2D == null && GameObject.FindGameObjectWithTag("BoundingShape"))
            cameraChild.m_BoundingShape2D = GameObject.FindGameObjectWithTag("BoundingShape").GetComponent<PolygonCollider2D>();
    }

    IEnumerator MoveCameraOnCombat()
    {
        Vector3 cameraPos = (Player.GetLastEnemy().transform.position - Player.transform.position)/2 + transform.position;
        cameraPos.z = offset.z;
        while (transform.position != cameraPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, cameraPos, cameraSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
