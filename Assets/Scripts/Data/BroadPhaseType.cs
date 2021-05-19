using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BroadPhaseType", menuName = "Data/Enum/BroadPhaseType")]
public class BroadPhaseType : EnumData
{
    public enum eType
    {
        None,
        Quadtree,
        BVH
    }

    public eType value;

    public override int index 
    {
        get => (int)value; 
        set => this.value = (eType)value; 
    }

    public override string[] names 
    {
        get => Enum.GetNames(typeof(eType)); 
    }
}
