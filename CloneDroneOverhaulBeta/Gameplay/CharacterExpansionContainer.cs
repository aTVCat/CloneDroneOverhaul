using System.Collections.Generic;

namespace CDOverhaul.Gameplay
{
    public class CharacterExpansionContainer : OverhaulBehaviour
    {
        public static readonly Dictionary<int, CharacterExpansionContainer> CachedContainers = new Dictionary<int, CharacterExpansionContainer>();

        public List<OverhaulCharacterExpansion> Expansions = new List<OverhaulCharacterExpansion>();
        public int MyInstanceId;

        protected override void OnDisposed()
        {
            _ = CachedContainers.Remove(MyInstanceId);
            Expansions.Clear();
            Expansions = null;
        }
    }
}
