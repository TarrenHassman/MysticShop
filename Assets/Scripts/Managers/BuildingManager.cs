using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class BuildingManager : MonoBehaviour
{
    public GameObject[] objects;
    private GameObject pendingObj;
    private BuildingActions building;
    private Vector3 pos;
    private RaycastHit hit;
    private Camera main;
  
    [SerializeField] private LayerMask layerMask;

    protected void Awake(){
        building = new BuildingActions();
        main = Camera.main;
    }

    protected void OnEnable()
    {
        building.Building.Click.performed += PlaceObject;
        building.Building.Enable();
    }

    protected void OnDisable(){
        building.Building.Click.performed += PlaceObject;
        building.Building.Disable();
    }
    private void PlaceObject(InputAction.CallbackContext context){
        pendingObj = null;
    }
    void Update()
    {
        if (pendingObj != null)
        {
            pendingObj.transform.position = pos;
        }


    }
    void FixedUpdate()
    {
     if(Physics.Raycast(main.ScreenPointToRay(Input.mousePosition), out hit, 1000, layerMask)){
        pos = hit.point;
      }   

    }

    public void SelectObject(int index){
        pendingObj = Instantiate(objects[index], pos, transform.rotation);
    }

}
