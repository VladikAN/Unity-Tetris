using UnityEngine;
using System;

/// <summary>
/// Component will render 2d array into bricks.
/// </summary>
public class BricksRenderComponent : MonoBehaviour
{
    /// <summary>
    /// This object is used for render
    /// </summary>
    public GameObject Template;

    public float TemplateSize;
    public uint Width;
    public uint Height;
    public float OffsetX = 0;
    public float OffsetY = 0;

    /// <summary>
    /// Store set of objects. This objects will be used for render.
    /// </summary>
    private GameObject[,] _objectsPool;

    /// <summary>
    /// On game start action
    /// </summary>
    public void Start()
    {
        // Validate input parameters
        Validate();

        // Fill objects pool by Height * Width objects
        _objectsPool = new GameObject[Height, Width];
        for (var i = 0; i < Height; i++)
        {
            for (var j = 0; j < Width; j++)
            {
                var obj = Instantiate<GameObject>(Template, transform, false);
                obj.name = string.Format("Brick {0}x{1}", i, j);
                obj.SetActive(false);
                obj.transform.localPosition = new Vector3(OffsetX + j * TemplateSize, -OffsetY + (-i * TemplateSize));

                _objectsPool[i, j] = obj;
            }
        }
    }

    /// <summary>
    /// Activate bricks from pool according to passed map and coord
    /// </summary>
    public void Render(byte[,] map, uint offsetX, uint offsetY, bool revert = false)
    {
        if (revert)
        {
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    _objectsPool[i, j].SetActive(false);
                }
            }
        }

        for (var i = 0; i < map.GetLength(0); i++)
        {
            for (var j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == 1)
                {
                    _objectsPool[i + offsetY, j + offsetX].SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Activate bricks from pool according to passed map
    /// </summary>
    public void Render(byte[,] map)
    {
        Render(map, 0, 0, true);
    }

    /// <summary>
    /// Draw gizmos to see render in editor mode
    /// </summary>
    public void OnDrawGizmosSelected()
    {
        // Calculate edge points and draw gizmos
        var x = transform.position.x;
        var y = transform.position.y;
        var point1 = new Vector2(x + OffsetX, y - OffsetY);
        var point2 = new Vector2(x + OffsetX + TemplateSize * Width, y - OffsetY);
        var point3 = new Vector2(x + OffsetX + TemplateSize * Width, y - OffsetY - (Height * TemplateSize));
        var point4 = new Vector2(x + OffsetX, y - OffsetY - (Height * TemplateSize));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(point1, point2);
        Gizmos.DrawLine(point2, point3);
        Gizmos.DrawLine(point3, point4);
        Gizmos.DrawLine(point4, point1);
    }

    private void Validate()
    {
        if (Template == null)
        {
            throw new Exception("Template is not specified for Bricks Render");
        }

        if (TemplateSize == 0)
        {
            throw new Exception("Template Size is not specified for Bricks Render");
        }

        if (Width == 0)
        {
            throw new Exception("Width is not specified for Bricks Render");
        }

        if (Height == 0)
        {
            throw new Exception("Height is not specified for Bricks Render");
        }
    }
}