using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BodyEnum", menuName = "Data/Enum/Body")]
public class BodyEnumData : EnumData
{
    public enum eType
    {
        Static,
        Kinematic,
        Dynamic
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
