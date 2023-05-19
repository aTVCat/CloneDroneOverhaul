// I took it here: https://assetstore.unity.com/packages/tools/particles-effects/quick-outline-115488

//
//  Outline.cs
//  QuickOutline
//
//  Created by Chris Nolet on 3/30/18.
//  Copyright © 2018 Chris Nolet. All rights reserved.
//

using CDOverhaul;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CDOverhaul.Credits
{
    public class ThreeDOutline : MonoBehaviour
    {
        private static readonly HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

        public Color OutlineColor
        {
            get => outlineColor;
            set
            {
                outlineColor = value;
                UpdateMaterialProperties();
            }
        }

        public float OutlineWidth
        {
            get => outlineWidth;
            set
            {
                outlineWidth = value;
                UpdateMaterialProperties();
            }
        }

        private Color outlineColor = Color.white;

        private float outlineWidth = 10f;

        private readonly List<Mesh> bakeKeys = new List<Mesh>();

        private readonly List<List<Vector3>> bakeValues = new List<List<Vector3>>();

        private Renderer[] renderers;
        private Material outlineMaskMaterial;
        private Material outlineFillMaterial;

        private void Awake()
        {
            // Cache renderers
            renderers = GetComponentsInChildren<Renderer>();

            // Instantiate outline materials
            outlineMaskMaterial = Instantiate(AssetsController.GetAsset<Material>("OutlineMask", OverhaulAssetsPart.Part2));
            outlineFillMaterial = Instantiate(AssetsController.GetAsset<Material>("OutlineFill", OverhaulAssetsPart.Part2));

            outlineMaskMaterial.name = "OutlineMask (Instance)";
            outlineFillMaterial.name = "OutlineFill (Instance)";

            // Retrieve or generate smooth normals
            LoadSmoothNormals();
        }

        private void OnDestroy()
        {
            Destroy(outlineFillMaterial);
            Destroy(outlineMaskMaterial);
        }
        
        private void OnEnable()
        {
            foreach (Renderer renderer in renderers)
            {
                List<Material> materials = renderer.sharedMaterials.ToList();
                if(!materials.Contains(outlineFillMaterial) && !materials.Contains(outlineFillMaterial))
                {
                    materials.Add(outlineMaskMaterial);
                    materials.Add(outlineFillMaterial);
                }
                renderer.materials = materials.ToArray();
            }
        }

        private void OnDisable()
        {
            foreach (Renderer renderer in renderers)
            {
                // Remove outline shaders
                List<Material> materials = renderer.sharedMaterials.ToList();
                _ = materials.Remove(outlineMaskMaterial);
                _ = materials.Remove(outlineFillMaterial);
                renderer.materials = materials.ToArray();
            }
        }

        private void LoadSmoothNormals()
        {
            // Retrieve or generate smooth normals
            foreach (MeshFilter meshFilter in GetComponentsInChildren<MeshFilter>())
            {
                // Skip if smooth normals have already been adopted
                if (!registeredMeshes.Add(meshFilter.sharedMesh))
                {
                    continue;
                }

                // Retrieve or generate smooth normals
                int index = bakeKeys.IndexOf(meshFilter.sharedMesh);
                List<Vector3> smoothNormals = (index >= 0) ? bakeValues[index] : SmoothNormals(meshFilter.sharedMesh);

                // Store smooth normals in UV3
                meshFilter.sharedMesh.SetUVs(3, smoothNormals);

                // Combine submeshes
                Renderer renderer = meshFilter.GetComponent<Renderer>();

                if (renderer != null)
                {
                    CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
                }
            }

            // Clear UV3 on skinned mesh renderers
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                // Skip if UV3 has already been reset
                if (!registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
                {
                    continue;
                }

                // Clear UV3
                skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];

                // Combine submeshes
                CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
            }
        }

        private List<Vector3> SmoothNormals(Mesh mesh)
        {
            // Group vertices by location
            IEnumerable<IGrouping<Vector3, KeyValuePair<Vector3, int>>> groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

            // Copy normals to a new list
            List<Vector3> smoothNormals = new List<Vector3>(mesh.normals);

            // Average normals for grouped vertices
            foreach (IGrouping<Vector3, KeyValuePair<Vector3, int>> group in groups)
            {
                // Skip single vertices
                if (group.Count() == 1)
                {
                    continue;
                }

                // Calculate the average normal
                Vector3 smoothNormal = Vector3.zero;

                foreach (KeyValuePair<Vector3, int> pair in group)
                {
                    smoothNormal += smoothNormals[pair.Value];
                }

                smoothNormal.Normalize();

                // Assign smooth normal to each vertex
                foreach (KeyValuePair<Vector3, int> pair in group)
                {
                    smoothNormals[pair.Value] = smoothNormal;
                }
            }

            return smoothNormals;
        }

        private void CombineSubmeshes(Mesh mesh, Material[] materials)
        {

            // Skip meshes with a single submesh
            if (mesh.subMeshCount == 1)
            {
                return;
            }

            // Skip if submesh count exceeds material count
            if (mesh.subMeshCount > materials.Length)
            {
                return;
            }

            // Append combined submesh
            mesh.subMeshCount++;
            mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
        }

        private void UpdateMaterialProperties()
        {
            // Apply properties according to mode
            outlineFillMaterial.SetColor("_OutlineColor", outlineColor);
            outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
            outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
            outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
        }
    }
}
