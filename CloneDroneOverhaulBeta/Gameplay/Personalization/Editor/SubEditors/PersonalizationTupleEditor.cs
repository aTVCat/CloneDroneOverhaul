using ModLibrary;
using OverhaulAPI.SharedMonoBehaviours;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationTupleEditor : PersonalizationEditorUIElement
    {
        [ObjectReference("Container")]
        private readonly Transform m_Container;

        private bool m_HasInitialized;

        public ITuple EditingTuple
        {
            get;
            set;
        }

        public PersonalizationEditorPropertyCategoryDisplay CategoryDisplay
        {
            get;
            set;
        }

        protected override bool AssignVariablesAutomatically() => false;

        public void Show(ITuple tuple, PersonalizationEditorPropertyCategoryDisplay categoryDisplay)
        {
            if (!m_HasInitialized)
            {
                OverhaulUIController.AssignValues(this);
                OverhaulUIController.AssignActionToButton(GetComponent<ModdedObject>(), "BackButton", Hide);
                _ = base.gameObject.AddComponent<OverhaulDraggablePanel>();
                m_HasInitialized = true;
            }

            EditingTuple = tuple;
            CategoryDisplay = categoryDisplay;

            base.gameObject.SetActive(true);
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
        }

        public void Populate(bool isPositionNode = false)
        {
            TransformUtils.DestroyAllChildren(m_Container);
            if (isPositionNode)
            {
                PersonalizationEditorStringFieldDisplay stringFieldDisplay = Instantiate(CategoryDisplay.ShortStringFieldDisplay, m_Container);
                stringFieldDisplay.HasDifferentControl = true;
                stringFieldDisplay.TargetObject = EditingTuple.GetPrivateField<string>("m_Item1");
                stringFieldDisplay.gameObject.SetActive(true);
                stringFieldDisplay.Initialize(null, null);
                stringFieldDisplay.SetLabelText("ID");
                stringFieldDisplay.SetOnValueChangeAction(delegate (string newValue)
                {
                    EditingTuple.SetPrivateField("m_Item1", newValue);
                });

                PersonalizationEditorVector3FieldDisplay vectorFieldDisplay = Instantiate(CategoryDisplay.Vector3FieldDisplay, m_Container);
                vectorFieldDisplay.HasDifferentControl = true;
                vectorFieldDisplay.TargetObject = EditingTuple.GetPrivateField<Vector3>("m_Item2");
                vectorFieldDisplay.gameObject.SetActive(true);
                vectorFieldDisplay.Initialize(null, null);
                vectorFieldDisplay.SetLabelText("Offset");
                vectorFieldDisplay.SetOnValueChangeAction(delegate (Vector3 newValue)
                {
                    EditingTuple.SetPrivateField("m_Item2", newValue);
                });
            }
        }
    }
}
