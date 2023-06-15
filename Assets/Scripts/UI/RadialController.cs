using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class RadialController : MonoBehaviour
{
	public RadialMenu Data;
	public RadialCakePiece Piece;
	public float GapWidthDegree = 1f;

	protected RadialCakePiece[] pieces;
	protected RadialController parent;
	[HideInInspector]
	int activeIndex = 0;
	public Transform center;
	public GameObject[] entities;
	public Dictionary<string, Action<GameObject[]>> actionMap = new Dictionary<string, Action<GameObject[]>>();
	public bool menuOpen = false;

    private void Awake()
    {
		var stepLength = 360f / Data.elements.Length;
		var iconDist = Vector3.Distance(Piece.icon.transform.position, Piece.CakePiece.transform.position);
		pieces = new RadialCakePiece[Data.elements.Length];
		for (int i = 0; i < Data.elements.Length; i++)
		{
			pieces[i] = Instantiate(Piece, transform);
			pieces[i].transform.localPosition = Vector3.zero;
			pieces[i].transform.localRotation = Quaternion.identity;

			//Set Piece location and size
			pieces[i].CakePiece.fillAmount = 1f / Data.elements.Length - GapWidthDegree / 360f;
			pieces[i].CakePiece.transform.localPosition = Vector3.zero;
            pieces[i].CakePiece.transform.localRotation = Quaternion.Euler(0,0, stepLength / 2f +GapWidthDegree /2f + i*stepLength);
			//pieces[i].CakePiece.color = new Color()
			//Set Icon
			var a =((stepLength * i)+180);
			pieces[i].icon.transform.localPosition = pieces[i].CakePiece.transform.localPosition
				+ Quaternion.AngleAxis(a, Vector3.forward) * Vector3.up * iconDist;

			pieces[i].icon.sprite = Data.elements[i].Sprite;
        }			
    }


		 // private void Input(){
    //         _input = GetComponent<StarterAssetsInputs>();
    //         // if there is an input and camera position is not fixed
    //         if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
    //         {
    //             //Don't multiply mouse input by Time.deltaTime;
    //             float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

    //             _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
    //             _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
    //         }

    //         // clamp our rotations so our values are limited 360 degrees
    //         _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
    //         _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

    //         // Cinemachine will follow this target
    //         CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
    //             _cinemachineTargetYaw, 0.0f);
    // }


	public void AddAction(string name, Action<GameObject[]> action){
		//TODO: Add check for duplicate names
		actionMap.Add(name, action);
		foreach(RadialMenuItem item in Data.elements){
			if(item.Name == name){
				item.Action = action;
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		//Add Controller support
		//For controller you would grab the input for the stick you want to use, 
		//then you can use math.atan2 to turn it into an angle,
		//convert that into a button index so when they press A you check that,
		//make sure stick input has a minimum magnitude and you are good to go.
		var e = Data.elements.Length;
		var stepLength = 360f / Data.elements.Length;
		Vector2 delta = center.position - Input.mousePosition;
		float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
		angle = NormalizeAngle(angle-(e *10f +(GapWidthDegree*2)+(25-(5*e))));
		activeIndex = (int)(angle / stepLength);
		for(var i =0; i<Data.elements.Length;i++){
			if(activeIndex == i){
				pieces[i].CakePiece.color = new Color(1, 1, 1, 1);
			}else{
				pieces[i].CakePiece.color = new Color(1, 1, 1, 0.2f);
			}
		}
		if (Input.GetMouseButtonDown(0))
		{
			if(menuOpen){
				onClick();
			}
		}
	}

	public void onClick(){
		if(Data.elements[activeIndex].NextRing != null){
			var next = Instantiate(gameObject, transform.parent).GetComponent<RadialController>();
			next.parent = this;
			for(var j=0; j < next.transform.childCount; j++){
				Destroy(next.transform.GetChild(j).gameObject);
			}
			next.Data = Data.elements[activeIndex].NextRing;
		}
		else{
			Data.elements[activeIndex].Action?.Invoke(entities);
		}
		if(StoreLoad.Instance.radial.interactable){
			gameObject.SetActive(false);
			menuOpen = false;
		}
	}

	public void OpenMenu(){
		StoreLoad.Instance.swapRadialVisibility();
		menuOpen = true;
		gameObject.SetActive(true);
	}

	private float NormalizeAngle(float a) => (a + 360) % 360;
}

