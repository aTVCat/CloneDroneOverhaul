using UnityEngine;

namespace CDOverhaul
{
    public class MaterialInfo
    {
        public Material Material;

        public string Name => Material.name;
        public string TextureName => Material.mainTexture ? Material.mainTexture.name : string.Empty;
        public Color Color => Material.color;

        public Vector3 Scale;

        public bool Equals(MaterialInfo other)
        {
            return (other.Name, other.TextureName, other.Color, other.Scale) == (Name, TextureName, Color, Scale);
        }

        public MaterialInfo(Material material, Vector3 scale)
        {
            Material = material;
            Scale = scale;
        }
    }
}
