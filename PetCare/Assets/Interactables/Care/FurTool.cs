using Impulses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurTool : DogInteractive
{
    public bool isDirtLayer;
    [Range(0, 255)]
    public System.Byte value;

    protected override void OnDogHit(RaycastHit hitData)
    {
        transform.position = hitData.point;
        m_FurSimulation.DrawAtFur(hitData.textureCoord, 0.02f, isDirtLayer, value);
    }
    
}
