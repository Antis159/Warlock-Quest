using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingControl : MonoBehaviour
{
    public string buildingName;
    public int[] buildCost = new int[4];

    public int[] GetBuildCost()
    {
        return buildCost;
    }
    public string GetBuildingName()
    {
        return buildingName;
    }
}
