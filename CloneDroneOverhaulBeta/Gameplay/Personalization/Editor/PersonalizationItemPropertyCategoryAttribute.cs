using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Gameplay
{
    public class PersonalizationItemPropertyCategoryAttribute : Attribute
    {
        public string Category;

        public PersonalizationItemPropertyCategoryAttribute(string name)
        {
            Category = name;
        }
    }
}
