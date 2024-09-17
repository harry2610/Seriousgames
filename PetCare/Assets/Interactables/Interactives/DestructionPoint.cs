using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionPoint : MonoBehaviour
{
    private MeshRenderer m_MeshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        m_MeshRenderer = GetComponentInChildren<MeshRenderer>();
        m_MeshRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Destruct()
    {
        m_MeshRenderer.enabled = true;
    }
}
