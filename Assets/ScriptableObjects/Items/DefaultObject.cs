
using UnityEngine;

[CreateAssetMenu(fileName = "New Wearable Object", menuName = "Inventory System/Items/Wearable")]
public class WearableObject : ItemObject
{
  public void Awake()
  {
    type = ItemType.Wearable;
  }
}