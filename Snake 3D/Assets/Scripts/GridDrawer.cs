using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridHeight = 10;

    [Header("Line Settings")]
    public float lineWidth = 0.03f;
    public Color lineColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    [Header("Border Settings")]
    public float borderWidth = 0.06f;
    public Color borderColor = new Color(0.8f, 0.8f, 0.8f, 1f);

    // Kareler 0,1,2...9 merkezli, yani kenarlar -0.5 ile 9.5 arasinda
    private float offset = 0.5f;

    void Start()
    {
        DrawGrid();
        DrawBorder();
    }

    void DrawGrid()
    {
        GameObject gridParent = new GameObject("GridLines");
        gridParent.transform.parent = transform;

        Material lineMat = CreateLineMaterial(lineColor);

        float minX = -offset;
        float maxX = gridWidth - 1 + offset;
        float minZ = -offset;
        float maxZ = gridHeight - 1 + offset;

        // Dikey cizgiler
        for (int x = 0; x <= gridWidth - 1; x++)
        {
            float posX = x + offset;
            CreateLine(gridParent.transform, lineMat,
                new Vector3(posX, 0.01f, minZ),
                new Vector3(posX, 0.01f, maxZ),
                lineWidth, "LineV_" + x);
        }

        // Yatay cizgiler
        for (int z = 0; z <= gridHeight - 1; z++)
        {
            float posZ = z + offset;
            CreateLine(gridParent.transform, lineMat,
                new Vector3(minX, 0.01f, posZ),
                new Vector3(maxX, 0.01f, posZ),
                lineWidth, "LineH_" + z);
        }
    }

    void DrawBorder()
    {
        GameObject borderParent = new GameObject("BorderLines");
        borderParent.transform.parent = transform;

        Material borderMat = CreateLineMaterial(borderColor);

        float minX = -offset;
        float maxX = gridWidth - 1 + offset;
        float minZ = -offset;
        float maxZ = gridHeight - 1 + offset;
        float y = 0.02f;

        // 4 kenar cizgisi
        CreateLine(borderParent.transform, borderMat,
            new Vector3(minX, y, minZ), new Vector3(maxX, y, minZ),
            borderWidth, "BorderBottom");

        CreateLine(borderParent.transform, borderMat,
            new Vector3(minX, y, maxZ), new Vector3(maxX, y, maxZ),
            borderWidth, "BorderTop");

        CreateLine(borderParent.transform, borderMat,
            new Vector3(minX, y, minZ), new Vector3(minX, y, maxZ),
            borderWidth, "BorderLeft");

        CreateLine(borderParent.transform, borderMat,
            new Vector3(maxX, y, minZ), new Vector3(maxX, y, maxZ),
            borderWidth, "BorderRight");
    }

    void CreateLine(Transform parent, Material mat, Vector3 start, Vector3 end, float width, string name)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.parent = parent;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = mat;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.useWorldSpace = true;
        lr.receiveShadows = false;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.numCornerVertices = 0;
        lr.numCapVertices = 2;
    }

    Material CreateLineMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
            shader = Shader.Find("Unlit/Color");

        Material mat = new Material(shader);
        mat.color = color;
        return mat;
    }
}