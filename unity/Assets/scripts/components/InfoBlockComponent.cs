using System;
using UnityEngine;

public class InfoBlockComponent : MonoBehaviour
{
    /// <summary>
    /// Link to text mesh
    /// </summary>
    public TextMesh Mesh;

    public void Start()
    {
        // Validate parameters
        if (Mesh == null)
        {
            throw new Exception("Mesh is not set for InfoBlockComponent");
        }
    }

    public void PrintValue(int value)
    {
        Mesh.text = value >= 99999 ? "99999" : value.ToString("D5");
    }
}