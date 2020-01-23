using System;
using UnityEngine;

/// <summary>
/// Warper object is responsible for warping/edge blend
/// 
/// Author: Christoffer A Træen
/// </summary>
public class Dewarp {
    /// <summary>
    /// Holds the positions for the Dewarp mesh
    /// </summary>
    [Serializable]
    public class DewarpMeshPosition {
        [Header ("Mesh vertecies(Do not add extra verts)")]
        public Vector3[] verts;

        /// <summary>
        /// Holds generated vertecies: filled if verts are empty
        /// </summary>
        public Vector3[] generatedVerts;

    };

    public readonly int xSize = 7;
    public readonly int ySize = 7;

    /// <summary>
    /// The game object name for the dewarp mesh
    /// </summary>
    private readonly string objectName = "Dewarp Mesh For:";

    /// <summary>
    /// Holds the gameobject reference for the dewarp mesh
    /// </summary>
    private GameObject dewarpObject;

    /// <summary>
    /// Holds the reference to the dewarp mesh
    /// </summary>
    private Mesh warpMesh;

    /// <summary>
    /// Holds the reference to the dewarp mesh filter
    /// </summary>
    private MeshFilter warpMeshFilter;

    /// <summary>
    /// Render materioal
    /// </summary>
    private Material renderMaterial;

    /// <summary>
    /// Holds the dewarp positions
    /// </summary>
    public DewarpMeshPosition dewarpPositions;

    public Dewarp (string name, Material postProcessMaterial, DewarpMeshPosition vertices, RenderTexture cameraTexture) {
        this.dewarpPositions = vertices;

        dewarpObject = new GameObject (objectName + name);
        dewarpObject.layer = 8;

        this.warpMeshFilter = dewarpObject.AddComponent<MeshFilter> ();

        this.warpMeshFilter.mesh = Generate ();
        dewarpObject.layer = 8; //post processing layer is 8

        //create material for left warpMesh
        this.renderMaterial = new Material (postProcessMaterial);
        this.renderMaterial.name = $"Material for {name}";

        //assign the render texture to the material and the material to the warpMesh
        renderMaterial.mainTexture = cameraTexture;
        dewarpObject.AddComponent<MeshRenderer> ().material = renderMaterial;

    }

    /// <summary>
    /// Generates the dewarp mesh.
    /// The mesh has a total of <c>xSize*ySize</c> vertices.
    /// Normals, tangtents, UVs and triangles is generated.
    /// 
    /// Vertices starts from bottom left.
    /// </summary>
    /// <returns>returns the generated mesh</returns>
    private Mesh Generate () {
        this.warpMesh = new Mesh ();
        this.warpMesh.name = "Warp grid";

        this.dewarpPositions.generatedVerts = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[this.dewarpPositions.generatedVerts.Length];
        Vector4[] tangents = new Vector4[this.dewarpPositions.generatedVerts.Length];
        Vector4 tangent = new Vector4 (1f, 0f, 0f, -1f);

        decimal xx = xSize;
        decimal yy = ySize;

        decimal ymodifier = (2 / yy);
        decimal lastY = -1;

        decimal xmodifier = (2 / xx);
        decimal lastX = -1;

        for (int i = 0, y = 0; y <= ySize; y++) {
            for (int x = 0; x <= xSize; x++, i++) {
                if (this.dewarpPositions.verts.Length == this.dewarpPositions.generatedVerts.Length) {
                    this.dewarpPositions.generatedVerts[i] = new Vector3 ((float) this.dewarpPositions.verts[i].x, (float) this.dewarpPositions.verts[i].y);
                } else {
                    this.dewarpPositions.generatedVerts[i] = new Vector3 ((float) lastX, (float) lastY);
                }
                uv[i] = new Vector2 ((float) x / xSize, (float) y / ySize);
                tangents[i] = tangent;
                lastX += (decimal) xmodifier;

            }
            lastY += (decimal) ymodifier;
            lastX = -1;
        }

        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
            for (int x = 0; x < xSize; x++, ti += 6, vi++) {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        this.warpMesh.vertices = this.dewarpPositions.generatedVerts;
        this.warpMesh.triangles = triangles;
        this.warpMesh.uv = uv;
        this.warpMesh.tangents = tangents;
        this.warpMesh.RecalculateNormals ();
        this.warpMesh.RecalculateTangents ();
        if (this.dewarpPositions.verts.Length == 0) {
            this.dewarpPositions.verts = this.dewarpPositions.generatedVerts;
        }

        return this.warpMesh;

    }

    /// <summary>
    /// Returns the GameObject reference for the dewarp mesh
    /// </summary>
    /// <returns>GameObject reference for the dewarp mesh</returns>
    public GameObject GetDewarpGameObject () {
        return this.dewarpObject;
    }

    /// <summary>
    /// Returns the Dewarp mesh reference
    /// </summary>
    /// <returns>reference of the warp mesh</returns>
    public Mesh GetDewarpMesh () {
        return this.warpMesh;
    }

    /// <summary>
    /// Returns the Dewarp mesh filter reference
    /// </summary>
    /// <returns>reference of the warp mesh filter </returns>
    public MeshFilter GetDewarpMeshFilter () {
        return this.warpMeshFilter;
    }

}