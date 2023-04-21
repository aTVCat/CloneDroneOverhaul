using System.Collections.Generic;

namespace CDOverhaul
{
    public static class OverhaulStaticEditor<T>
    {
        public static readonly List<T> SelectedItems = new List<T>();

        public static void ClearList()
        {
            if (SelectedItems.IsNullOrEmpty())
            {
                return;
            }

            SelectedItems.Clear();
        }

        public static void SelectItem(T item)
        {
            if (SelectedItems.Contains(item))
            {
                return;
            }

            SelectedItems.Add(item);
        }

        public static void DeselectItem(T item)
        {
            if (!SelectedItems.Contains(item))
            {
                return;
            }

            SelectedItems.Remove(item);
        }

        public static bool HasSelectedItem(T item)
        {
            return SelectedItems.Contains(item);
        }
    }
}
