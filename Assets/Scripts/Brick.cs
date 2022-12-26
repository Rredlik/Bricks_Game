using System.Collections.Generic;
using UnityEngine;

public enum BrickClass
{
    Default, ModelPart
}

[System.Serializable]
public struct RendererMaterialsData
{
    public Material defaultMaterial, fadingMaterial;
}

public class Brick : MonoBehaviour
{
    public GridVector size;
    [HideInInspector] public BrickClass brickClass;

    public MeshRenderer meshRenderer;
    public Renderer[] _renderers;
    //public Renderer material;
    public Material[] blocksMaterials;

    [HideInInspector] public Transform positionPoint;
    [HideInInspector] public GridCell ownerCell;
    [HideInInspector] public List<GridCell> cells;
    [HideInInspector] public int orderIndex;

    RendererMaterialsData[] renderersMaterials;

    public bool Silhouette { get; private set; }

    void Awake()
    {
        Silhouette = false;

        renderersMaterials = new RendererMaterialsData[_renderers.Length];
        for (int i = 0; i < renderersMaterials.Length; i++)
        {
            Renderer _renderer = _renderers[i];
            renderersMaterials[i] = new RendererMaterialsData
            {
                defaultMaterial = _renderer.material,
                fadingMaterial = CreateFadingMaterial(_renderer.material)
            };
        }
    }

    public void SetMaterial()
    {
        //Renderer material = GetComponent();
        
    }

    public void CreatePositionPoint(float rotationAngle)
    {
        Vector3 size = meshRenderer.bounds.size;
        float halfSizeX = size.x / 2f;
        float halfSizeZ = size.z / 2f;
        Vector3 pos;
        if(rotationAngle == 0f)
        {
            pos = new Vector3(-halfSizeX, 0f, -halfSizeZ);
        }
        else if (rotationAngle == 90f)
        {
            pos = new Vector3(halfSizeX, 0f, -halfSizeZ);
        }
        else if (rotationAngle == 180f)
        {
            pos = new Vector3(halfSizeX, 0f, halfSizeZ);
        }
        else
        {
            pos = new Vector3(-halfSizeX, 0f, halfSizeZ);
        }
        positionPoint = new GameObject("PositionPoint").transform;
        positionPoint.SetParent(transform);
        positionPoint.rotation = Quaternion.identity;
        positionPoint.localPosition = pos;
    }

    public void Remove()
    {
        if(cells.Count > 0)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i] != ownerCell)
                {
                    cells[i].RemoveBrick(this, false);
                }
            }

            cells.Clear();
        }

        ownerCell.RemoveBrick(this);
    }

    public GridCell FindCell(Vector3 point)
    {
        if(cells == null || cells.Count == 0)
        {
            return null;
        }

        for (int i = 0; i < cells.Count; i++)
        {
            GridCell cell = cells[i];
            if (cell.ContainsPoint(point))
            {
                return cell;
            }
        }

        return null;
    }

    public void SetAsSilhouette()
    {
        Silhouette = true;
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material = renderersMaterials[i].fadingMaterial;
        }
    }

    public void SetDefault()
    {
        Silhouette = false;
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material = renderersMaterials[i].defaultMaterial;
        }
    }

    Material CreateFadingMaterial(Material source)
    {
        Material newMaterial = new Material(source);
        newMaterial.SetFloat("_Mode", 2);
        newMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        newMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        newMaterial.SetInt("_ZWrite", 0);
        newMaterial.DisableKeyword("_ALPHATEST_ON");
        newMaterial.EnableKeyword("_ALPHABLEND_ON");
        newMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        newMaterial.DisableKeyword("_EMISSION");
        newMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
        newMaterial.SetColor("_EmissionColor", Color.black);
        newMaterial.renderQueue = 3000;
        SetMaterialAlpha(newMaterial, 0.5f);
        return newMaterial;
    }

    void SetMaterialAlpha(Material _material, float a)
    {
        Color color = _material.GetColor("_Color");
        color.a = a;
        _material.SetColor("_Color", color);
    }

    [ContextMenu("Get Renderers")]
    void GetRenderers()
    {
        _renderers = GetComponentsInChildren<Renderer>();
    }

    private void Reset()
    {
        GetRenderers();
    }
}