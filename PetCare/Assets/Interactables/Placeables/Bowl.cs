using System.Collections;
using System.Collections.Generic;
using UI.Items;
using UnityEngine;
using UnityEngine.Serialization;

public enum BowlType
{
    Water, Food, None
}
public class Bowl : MonoBehaviour
{
    // Type of Bowl
    public BowlType bowlType;
    // Prefab of spawned object
    public GameObject spawnedBowlContent;
    // Starting height offset of bowl content
    [Range(-0.5f, 0.5f)]
    public float contentHeightOffset;
    // Divisor of rate at which the content increases in height
    [Range(100, 5000)]
    public float fillValueHeightAdjustment;
    // Actual content gameobject
    private GameObject m_BowlContent;
    private float m_FillValue;
    // Effect of the food on the dogs condition per fill percent
    public ConditionEffect[] conditionEffects;
    public float FillValue
    {
        get => m_FillValue;

        set
        {
            // Cap fill at 100
            if (value > 100.0f) value = 100.0f;
            
            // Adjust content height
            m_BowlContent.transform.position = transform.position + Vector3.up * ((FillValue / fillValueHeightAdjustment) + contentHeightOffset);
            m_FillValue = value;
        }
    }

    public bool IsEmpty()
    {
        return FillValue <= 0;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // Spawn bowl content
        m_BowlContent = Instantiate(spawnedBowlContent, new Vector3(0, -4, 0), new Quaternion(0, 0, 90, 1));
        m_FillValue = 0f;
    }
    
}
