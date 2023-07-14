using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return (other.Name, other.TextureName, other.Color, other.Scale) == (this.Name, this.TextureName, this.Color, this.Scale);
        }

        public MaterialInfo(Material material, Vector3 scale)
        {
            Material = material;
            Scale = scale;
        }
    }
}
