using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FirstPersonNetworkController : NetworkBehaviour
{
    Vector2 look;
    Transform cam;
    
    private void Start()
    {
        cam = transform.GetComponentInChildren<Camera>().transform;
    }
    
    // public override void OnNetworkSpawn()
    // {
    //     if (IsLocalPlayer)
    //     {
    //         cam.gameObject.SetActive(true);
    //     }
    // }


    private void Update()
    {

        look.x += Input.GetAxis("Mouse X");
        look.y += Input.GetAxis("Mouse Y");
        look.y = Mathf.Clamp(look.y, -90, 90);
        cam.localRotation = Quaternion.Euler(-look.y*2, 0, 0);
        transform.localRotation = Quaternion.Euler(0, look.x*2, 0);

    }

}

