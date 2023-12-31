using System.Collections.Generic;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationItem
    {
        public string Name, Author, Description;

        public bool IsVerified; // is it custom?

        public List<string> ExclusiveFor;

        public bool IsExclusive()
        {
            return ExclusiveFor != null && ExclusiveFor.Count != 0;
        }
    }
}
