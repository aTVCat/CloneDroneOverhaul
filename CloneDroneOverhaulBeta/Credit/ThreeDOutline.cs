// I took it from here: https://assetstore.unity.com/packages/tools/particles-effects/quick-outline-115488

//
//  Outline.cs
//  QuickOutline
//
//  Created by Chris Nolet on 3/30/18.
//  Copyright © 2018 Chris Nolet. All rights reserved.
//

using CDOverhaul.Gameplay.QualityOfLife;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CDOverhaul.Credits
{
    public class ThreeDOutline : MonoBehaviour
    {
        private static readonly HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

        private readonly List<Mesh> bakeKeys = new List<Mesh>();

        private readonly List<List<Vector3>> bakeValues = new List<List<Vector3>>();

        private Renderer[] renderers;
        private static Material outlineMaskMaterial;
        private static Material outlineFillMaterial;

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            if (outlineFillMaterial == null || outlineMaskMaterial == null)
            {
                outlineMaskMaterial = OverhaulAssetsController.GetAsset<Material>("OutlineMask", OverhaulAssetPart.Part2);
                outlineFillMaterial = OverhaulAssetsController.GetAsset<Material>("OutlineFill", OverhaulAssetPart.Part2);
            }
            LoadSmoothNormals();
        }

        private void OnEnable()
        {
            foreach (Renderer renderer in renderers)
            {
                List<Material> materials = renderer.sharedMaterials.ToList();
                if (!materials.Contains(outlineFillMaterial) && !materials.Contains(outlineFillMaterial))
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
                if (meshFilter == null || meshFilter.sharedMesh == null || !registeredMeshes.Add(meshFilter.sharedMesh))
                    continue;

                // Retrieve or generate smooth normals
                int index = bakeKeys.IndexOf(meshFilter.sharedMesh);
                List<Vector3> smoothNormals = (index >= 0) ? bakeValues[index] : SmoothNormals(meshFilter.sharedMesh);

                // Store smooth normals in UV3
                meshFilter.sharedMesh.SetUVs(3, smoothNormals);

                // Combine submeshes
                Renderer renderer = meshFilter.GetComponent<Renderer>();

                if (renderer != null)
                    CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
            }

            // Clear UV3 on skinned mesh renderers
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                // Skip if UV3 has already been reset
                if (!registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
                    continue;

                // Clear UV3
                skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];

                // Combine submeshes
                CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
            }
        }

        private List<Vector3> SmoothNormals(Mesh mesh)
        {
            if (mesh == null)
                return new List<Vector3>();

            // Copy normals to a new list
            List<Vector3> smoothNormals = new List<Vector3>(mesh.normals);

            try
            {
                // Group vertices by location
                IEnumerable<IGrouping<Vector3, KeyValuePair<Vector3, int>>> groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

                // Average normals for grouped vertices
                foreach (IGrouping<Vector3, KeyValuePair<Vector3, int>> group in groups)
                {
                    // Skip single vertices
                    if (group == null || group.Count() == 1)
                        continue;

                    // Calculate the average normal
                    Vector3 smoothNormal = Vector3.zero;

                    foreach (KeyValuePair<Vector3, int> pair in group)
                        smoothNormal += smoothNormals[pair.Value];

                    smoothNormal.Normalize();

                    // Assign smooth normal to each vertex
                    foreach (KeyValuePair<Vector3, int> pair in group)
                        smoothNormals[pair.Value] = smoothNormal;
                }
            }
            catch
            {

            }

            return smoothNormals;
        }

        private void CombineSubmeshes(Mesh mesh, Material[] materials)
        {
            // Skip meshes with a single submesh
            if (mesh.subMeshCount == 1)
                return;

            // Skip if submesh count exceeds material count
            if (mesh.subMeshCount > materials.Length)
                return;

            // Append combined submesh
            mesh.subMeshCount++;
            mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
        }

        public static void UpdateMaterialProperties()
        {
            if (outlineMaskMaterial != null)
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);

            if (outlineFillMaterial != null)
            {
                outlineFillMaterial.SetColor("_OutlineColor", LevelEditorSelectionSettingsPanel.OutlineColor);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_OutlineWidth", LevelEditorSelectionSettingsPanel.OutlineWidth);
            }
        }
    }
}
