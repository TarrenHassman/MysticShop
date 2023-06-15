using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Unity.Netcode;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;

public class BuildingManager : NetworkBehaviour
{
    private GameObject pendingObj;
    private GameObject selectedObj;
    private BuildingActions building;
    private Vector3 pos;
    private RaycastHit hit;
    private Camera main;
    private float maxDistance = 1000;
    private int index;
    [SerializeField] private LayerMask layerMask;
    public float gridSize;
    public bool isGrid;
    public bool isRotating;
    public float change;
    private CanvasGroup radial;

    //Menu
    public RadialMenu MainMenuPrefab;
    protected RadialController MainMenu;

    //DEV FLAG
    private bool justClosedMenu = false;
    public bool menuOpen = false;


    protected void Awake(){
        building = new BuildingActions();
        main = Camera.main;
        gridSize = .5f;
        MainMenu = GameObject.Find("Radial").gameObject.GetComponent<RadialController>();
        radial = GameObject.Find("Radial").gameObject.GetComponent<CanvasGroup>();
        MainMenu.AddAction("Move", MoveSelected);
        MainMenu.AddAction("Delete", DeleteSelected);
        MainMenu.AddAction("List", MakeListing);
    }

    private void Start()
    {
    }

    protected void OnEnable()
    {
        building.Building.Click.performed += Click;
        building.Building.ChangeObject.performed += ChangeObject;
        building.Building.GridToggle.performed += ToggleGrid;
        building.Building.Rotate.performed += RotateObject;
        building.Building.Rotate.canceled += StopRotateObject;
        building.Building.Enable();
    }

    protected void OnDisable(){
        building.Building.Click.performed -= Click;
        building.Building.ChangeObject.performed -= ChangeObject;
        building.Building.GridToggle.performed -= ToggleGrid;
        building.Building.Rotate.performed -= RotateObject;
        building.Building.Disable();
    }

    void Update()
    {
        if (pendingObj != null)
        {
            pendingObj.transform.position = isGrid ? 
                new Vector3(
                    roundNearestPoint(pos.x),
                    roundNearestPoint(pos.y),
                    roundNearestPoint(pos.z))
                : pos;

            pendingObj.transform.rotation = isRotating ? 
                Quaternion.Euler(
                    pendingObj.transform.rotation.eulerAngles.x,
                    pendingObj.transform.rotation.eulerAngles.y + (change * Time.deltaTime * 20),
                    pendingObj.transform.rotation.eulerAngles.z)
                : pendingObj.transform.rotation;
        }
    }
    void FixedUpdate()
    {
     if(Physics.Raycast(main.ScreenPointToRay(Input.mousePosition), out hit, maxDistance, layerMask)){
        pos = hit.point;
    }   
    }
    private void ChangeObject(InputAction.CallbackContext context){
        float c = context.ReadValue<float>();
        index = (int)(index == 0 && c == -1 ? PrefabManager.instance.objects.Length-1 : index == PrefabManager.instance.objects.Length -1 && c == 1 ? 0 : index + c);
        if (pendingObj != null)
        {
            Destroy(pendingObj);
        }
        GameObject o = PrefabManager.instance.objects[index];
        o.name = Guid.NewGuid().ToString();
        pendingObj = Instantiate(PrefabManager.instance.objects[index], pos, transform.rotation);

    }
    private void RotateObject(InputAction.CallbackContext context){
        isRotating = true;
        change = context.ReadValue<float>();
    }
    private void Click(InputAction.CallbackContext context){
        if(!justClosedMenu && !menuOpen){
            if(pendingObj != null){
            PlaceObject();
            if(selectedObj != null){
                UnselectPlaced();
            }
            }else{
                if(Physics.Raycast(main.ScreenPointToRay(Input.mousePosition), out hit, maxDistance, layerMask)){
                    if(hit.collider.gameObject.CompareTag("Untagged") || hit.collider.gameObject.CompareTag("Listable")){
                        SelectPlaced(hit.collider.gameObject);
                    }
                }
            }
        }
        justClosedMenu = false;
    }
    private void PlaceObject(){
            AddToMap(pendingObj);
            pendingObj.layer = 3;
            pendingObj = null;

    }
    private void StopRotateObject(InputAction.CallbackContext context){
        isRotating = false;
    }
    private void MoveSelected(GameObject[] entities){
        if(selectedObj != null){
            selectedObj.layer = 0;
            pendingObj = selectedObj;
            justClosedMenu  = true;
        } 
    }
    private void DeleteSelected(GameObject[] entities){
        if(selectedObj != null){
            GameObject o = selectedObj;
            UnselectPlaced();
            Destroy(o);
        }
    }
    //TODO: Add menuOpen check
    private void MakeListing(GameObject[] entities){
        SaleInfo listing = selectedObj.GetComponent<SaleInfo>();
        if(listing != null){
            listing.OpenMenu();
            menuOpen = true;
            justClosedMenu = true;
        }
    }
    private void SelectPlaced(GameObject obj){
        Debug.Log(obj.name);
        if(obj != selectedObj){
                Outline outline = obj.GetComponent<Outline>();
                if(outline == null){
                    outline = obj.AddComponent<Outline>();
                }else{
                    outline.enabled = true;
                }
                selectedObj = obj;
                MainMenu.OpenMenu();
        }else{
            UnselectPlaced();
        }
    }

    private void UnselectPlaced(){
        selectedObj.GetComponent<Outline>().enabled = false;
        selectedObj = null;
    }

    float roundNearestPoint(float pos){
        float xDif = pos % gridSize;
        pos = xDif > (gridSize/2) ? pos+gridSize-xDif : pos-xDif;
        return pos;
    }
    //Implement snap to 90 degrees

    void ToggleGrid(InputAction.CallbackContext context){
        isGrid = !isGrid;
    }

    private void AddToMap(GameObject o)
    {
        SavableEntity e = Map.instance.convertToSavable(o);
        if(Map.instance.objects.ContainsKey(e.Id)){
            Map.instance.objects[e.Id] = e;
        }else{
            Map.instance.objects.Add(e.Id, e);
        }
    }


}
