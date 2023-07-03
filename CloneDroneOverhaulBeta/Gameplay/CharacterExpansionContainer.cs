using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Gameplay
{
    public class CharacterExpansionContainer : OverhaulBehaviour
    {
        public static readonly Dictionary<int, CharacterExpansionContainer> CachedContainers = new Dictionary<int, CharacterExpansionContainer>();

        public List<OverhaulCharacterExpansion> Expansions = new List<OverhaulCharacterExpansion>();
        public int MyInstanceId;

        protected override void OnDisposed()
        {
            if (CachedContainers.ContainsKey(MyInstanceId))
            {
                CachedContainers.Remove(MyInstanceId);
            }

            Expansions.Clear();
            Expansions = null;
        }
    }
}
