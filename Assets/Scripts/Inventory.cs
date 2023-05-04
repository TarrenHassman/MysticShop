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
  }

  public void Start(){
        inventoryPanel = GameObject.Find("Inventory").GetComponent<CanvasGroup>();
  }
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.E))
    {
      isVisible = !isVisible;
      inventoryPanel.alpha = isVisible ? 1 : 0;
      inventoryPanel.interactable = isVisible;
    }
  }
}