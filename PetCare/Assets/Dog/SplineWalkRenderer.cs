using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineWalkRenderer : MonoBehaviour
{
    public Mesh waypointMesh;
    [Range(1f, 50f)]
    public float waypointsPerMeter;
    [Range(0f, 0.05f)]
    public float waypointsRadius;
    private Walk walk;
    private Material _greenMaterial;
    private Material _blueMaterial;

    // Start is called before the first frame update
    void Start()
    {
        this.walk = this.transform.GetComponent<Walk>();
        var litShader = Shader.Find("Universal Render Pipeline/Lit");
        _greenMaterial = new Material(litShader);
        _greenMaterial.enableInstancing = true;
        _greenMaterial.SetColor("_BaseColor", Color.green);
        _blueMaterial = new Material(litShader);
        _blueMaterial.enableInstancing = true;
        _blueMaterial.SetColor("_BaseColor", Color.blue);
    }

    // Update is called once per frame
    void Update()
    {
        this.walk = this.transform.GetComponent<Walk>();
        if (walk == null)
            return;

        int count = (int)(walk.path.GetLength() * waypointsPerMeter);
        int countToCurrentPos = (int)(walk.move.position * waypointsPerMeter);
        Matrix4x4[] instData = new Matrix4x4[count];
        for (int i = 0; i < count; ++i)
        {
            var pos = walk.path.EvaluatePosition((i / waypointsPerMeter) / walk.path.GetLength());
            instData[i] = Matrix4x4.Translate(pos) * Matrix4x4.Scale(new Vector3(waypointsRadius, 0.01f, waypointsRadius));
        }
        if (countToCurrentPos > 0)
        {
            RenderParams rpGreen = new RenderParams(_greenMaterial);
            Graphics.RenderMeshInstanced(rpGreen, this.waypointMesh, 0, instData, countToCurrentPos, 0);
        }
        if (count - countToCurrentPos > 0)
        {
            RenderParams rpBlue = new RenderParams(_blueMaterial);
            Graphics.RenderMeshInstanced(rpBlue, this.waypointMesh, 0, instData, count - countToCurrentPos, countToCurrentPos);
        }
    }
}
