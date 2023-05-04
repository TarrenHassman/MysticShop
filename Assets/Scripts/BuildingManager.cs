using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingManager : MonoBehaviour
{
  public GameObject[] objects;
  private GameObject pendingObj;
  private readonly BuildingActions building;
  private Vector3 pos;
  private RaycastHit hit;
  
  [SerializeField] private LayerMask layerMask;

  protected void Awake(){
    BuildingActions building = new BuildingActions();
  }

  protected void OnEnable(){
    building.Building.FindAction("Click").performed += PlaceObject;
    building.Building.Enable();
  }

  protected void OnDisable(){
    building.Building.FindAction("Click").performed += PlaceObject;
    building.Building.Disable();
  }
    private void PlaceObject(InputAction.CallbackContext context){
      pendingObj = null;
    }
    void Update()
    {
        if(pendingObj != null){
            pendingObj.transform.position = pos;
        }
        

    }


    void FixedUpdate()
    {
     if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000, layerMask)){
        pos = hit.point;
      }   

    }

    private void SelectObject(int index){
        pendingObj = Instantiate(objects[index], pos, transform.rotation);
    }

}
