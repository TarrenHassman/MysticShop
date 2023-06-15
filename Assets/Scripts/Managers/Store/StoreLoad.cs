using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StoreLoad : NetworkBehaviour
{
    public CanvasGroup inventoryPanel;
    public CanvasGroup listingPanel;
    public CanvasGroup radial;
    static public StoreLoad Instance;
    void Awake()
    {
        Instance = this;
        inventoryPanel = GameObject.Find("Main").gameObject.GetComponent<CanvasGroup>();
        listingPanel = GameObject.Find("Info").gameObject.GetComponent<CanvasGroup>();
        radial = GameObject.Find("Radial").gameObject.GetComponent<CanvasGroup>();

    }
    private void Start()
    {
        inventoryPanel.alpha = 0;
        inventoryPanel.interactable = false;
        listingPanel.alpha = 0;
        listingPanel.interactable = false;
        radial.alpha = 0;
        radial.interactable = false;
    }

    public void swapInventoryVisibility()
    {
        inventoryPanel.alpha = inventoryPanel.interactable ? 0 : 1;
        inventoryPanel.interactable = inventoryPanel.interactable ? false : true;
    }
        public void swapRadialVisibility()
    {
        radial.alpha = inventoryPanel.interactable ? 0 : 1;
        radial.interactable = inventoryPanel.interactable ? false : true;
    }
    public void swapListingVisibility()
    {
        listingPanel.alpha = listingPanel.interactable ? 0 : 1;
        listingPanel.interactable = listingPanel.interactable ? false : true;
    }
}
