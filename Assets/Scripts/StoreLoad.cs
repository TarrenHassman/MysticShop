using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CanvasGroup temp = GameObject.Find("Inventory").gameObject.GetComponent<CanvasGroup>();
        temp.alpha = 0;
        temp.interactable = false;
        Debug.Log("StoreLoad");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
