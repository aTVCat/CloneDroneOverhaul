using CDOverhaul;
using CDOverhaul.Gameplay;
using System.Collections.Generic;

public class PersonalizationItemsData : OverhaulDisposable
{
    public List<PersonalizationItem> Items;

    protected override void OnDisposed()
    {
        Items.Clear();
        Items = null;
    }
}
