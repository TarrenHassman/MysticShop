using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
  List<ItemObject> items;
  private bool isVisible = false;
  CanvasGroup inventoryPanel;
  private void Awake()
  {
    items = new List<ItemObject>();
        inventoryPanel = GameObject.Find("Main").gameObject.GetComponent<CanvasGroup>();
  }

  public void Start(){
        inventoryPanel.alpha = 0;
        inventoryPanel.interactable = false;
  }
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.I))
    {
      isVisible = !isVisible;
      inventoryPanel.alpha = isVisible ? 1 : 0;
      inventoryPanel.interactable = isVisible;
    }
  }
}