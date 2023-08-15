using System.Reflection;
using System.Runtime.CompilerServices;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorTupleFieldDisplay : PersonalizationEditorFieldDisplay
    {
        public override void Initialize(FieldInfo fieldToEdit, object targetObject)
        {
            base.Initialize(fieldToEdit, targetObject);
            OverhaulUIVer2.AssignActionToButton(base.GetComponent<ModdedObject>(), "EditButton", EditTuple);
        }

        public override void InitializeAsCollectionObject(PersonalizationEditorListEntryDisplay listFieldDisplay, string displayName, object targetObject)
        {
            base.InitializeAsCollectionObject(listFieldDisplay, displayName, targetObject);
            OverhaulUIVer2.AssignActionToButton(base.GetComponent<ModdedObject>(), "EditButton", EditTuple);
        }

        public void EditTuple()
        {
            if (IsCollectionObject)
            {
                EditorUI.TupleEditor.Show(TargetObject as ITuple, base.ListFieldDisplay.FieldDisplay.Category);
                EditorUI.TupleEditor.Populate(base.ListFieldDisplay.FieldDisplay.IsPositionNodesList);
                return;
            }

            EditorUI.TupleEditor.Show(FieldValue as ITuple, Category);
            EditorUI.TupleEditor.Populate(base.ListFieldDisplay.FieldDisplay.IsPositionNodesList);
        }
    }
}
