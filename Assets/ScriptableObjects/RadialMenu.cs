using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "RadialMenu", menuName = "RadialMenu/Menu", order = 1)]
public class RadialMenu : ScriptableObject
{
    public RadialMenuItem[] elements;
}