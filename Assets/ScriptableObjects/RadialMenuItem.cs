using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "RadialMenu", menuName = "RadialMenu/Item", order = 2)]
public class RadialMenuItem : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public RadialMenu NextRing;
    public Action<GameObject[]> Action;

}

//ICON CREDITS
//<a href="https://www.flaticon.com/free-icons/move" title="move icons">Move icons created by Roundicons - Flaticon</a>
//<a href="https://www.flaticon.com/free-icons/delete" title="delete icons">Delete icons created by Kiranshastry - Flaticon</a>
