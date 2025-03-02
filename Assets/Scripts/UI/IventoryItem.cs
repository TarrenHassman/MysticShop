using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [Header("UI")]
    public Image image;
    [HideInInspector]
    public Transform parentAfterDrag;
  public void OnBeginDrag(PointerEventData eventData)
  {
    image.raycastTarget = false;
    parentAfterDrag = transform.parent;
    transform.SetParent(transform.root);

  }

  public void OnDrag(PointerEventData eventData)
  {
    transform.position = Input.mousePosition;
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    image.raycastTarget = true;
  }

}
