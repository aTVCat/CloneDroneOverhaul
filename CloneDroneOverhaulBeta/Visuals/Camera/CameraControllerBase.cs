using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class CameraControllerBase : OverhaulBehaviour
    {
        protected LevelEditorCinematicCamera CinematicCamera;

        private Camera m_CameraReference;
        public Camera CameraReference
        {
            get
            {
                if (!m_CameraReference)
                    m_CameraReference = base.GetComponent<Camera>();

                return m_CameraReference;
            }
        }

        public FirstPersonMover CameraOwner
        {
            get;
            protected set;
        }

        public bool IsCinematicCameraEnabled => CinematicCamera && CinematicCamera._camera;

        public override void Start()
        {
            OverhaulEventsController.AddEventListener<LevelEditorCinematicCamera>(GlobalEvents.CinematicCameraTurnedOn, onCinematicCameraEnabled, true);
            OverhaulEventsController.AddEventListener<Character>(GlobalEvents.CharacterKilled, onCharacterDied, true);

            if (base.transform.parent && base.transform.parent.parent)
            {
                CinematicCamera = base.transform.parent.parent.GetComponent<LevelEditorCinematicCamera>();
            }
        }

        protected override void OnDisposed()
        {
            OverhaulEventsController.RemoveEventListener<LevelEditorCinematicCamera>(GlobalEvents.CinematicCameraTurnedOn, onCinematicCameraEnabled, true);
            OverhaulEventsController.RemoveEventListener<Character>(GlobalEvents.CharacterKilled, onCharacterDied, true);
            OverhaulDisposable.AssignNullToAllVars(this);
        }

        public void SetOwner(FirstPersonMover newOwner)
        {
            CameraOwner = newOwner;
        }

        private void onCinematicCameraEnabled(LevelEditorCinematicCamera cinematicCamera)
        {
            CinematicCamera = cinematicCamera;
        }

        private void onCharacterDied(Character character)
        {
            if (!character)
                return;

            if (!CameraOwner || Equals(CameraOwner.GetInstanceID(), character.GetInstanceID()))
                DestroyBehaviour();
        }

        public static bool IsMainPlayerCamera(Camera camera)
        {
            return camera && camera.gameObject.activeInHierarchy && camera.GetComponent<PlayerCameraMover>();
        }

        public static Character GetCameraOwner(Camera camera)
        {
            return camera ? camera.GetComponentInParent<Character>() : null;
        }
    }
}
