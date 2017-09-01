using System;
using UnityEngine;

public class InfoBlockComponent : MonoBehaviour
{
    /// <summary>
    /// Link to text mesh
    /// </summary>
    public TextMesh Mesh;
    
    /// <summary>
    /// Validate parameters
    /// </summary>
    public void OnValidate()
    {
        if (Mesh == null) throw new Exception("Mesh is not set for InfoBlockComponent");
    }

    public void PrintValue(int value)
    {
        Mesh.text = value >= 999999 ? "999999" : value.ToString("D6");
    }
}