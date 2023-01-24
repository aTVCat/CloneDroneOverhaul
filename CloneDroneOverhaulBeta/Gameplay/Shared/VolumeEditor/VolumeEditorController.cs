using PicaVoxel;
using UnityEngine;

namespace CDOverhaul.Shared
{
    public class VolumeEditorController : ModController
    {
        public bool IsActivated { get; private set; }
        public Volume EditingVolume { get; private set; }

        private Camera _camera;

        public bool IsFullyActivated => IsActivated && EditingVolume != null && _camera != null;

        public EVolumeEditorTool Tool { get; private set; }

        public override void Initialize()
        {
            HasAddedEventListeners = true;
            IsInitialized = true;
        }

        public void EditVolume(in Volume volume)
        {
            if (volume == null)
            {
                IsActivated = false;
                EditingVolume = null;
                return;
            }

            _camera = Camera.main;
            if (_camera == null)
            {
                return;
            }

            IsActivated = true;
            EditingVolume = volume;

            ObjectStateListener.AddStateListener(EditingVolume.gameObject).AddOnDestroyTrigger(onVolumeDestroy);
        }

        /// <summary>
        /// This may happen for some reason
        /// </summary>
        private void onVolumeDestroy()
        {
            EditVolume(null);
        }

        private void Update()
        {
            if (!IsActivated || !IsFullyActivated)
            {
                return;
            }
            doRaycast();
        }

        private void doRaycast()
        {
            Ray newRay = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] raycastArray = Physics.RaycastAll(newRay, float.PositiveInfinity);
            Vector3 vector = Vector3.one * float.PositiveInfinity;

            foreach (RaycastHit hit in raycastArray)
            {
                Volume vol = hit.collider.gameObject.GetComponentInParents<Volume>();
                if (vol == EditingVolume)
                {
                    Vector3 vector2 = hit.point;
                    while ((vol.GetVoxelAtWorldPosition(vector2) == null || vol.GetVoxelAtWorldPosition(vector2).Value.State != VoxelState.Active) && Vector3.Distance(vector2, hit.point) <= 4f)
                    {
                        vector2 += _camera.transform.forward * 0.05f;
                    }
                    if (Vector3.Distance(_camera.transform.position, vector) > Vector3.Distance(_camera.transform.position, vector2))
                    {
                        vector = vector2;
                    }

                    break;
                }
            }

            Voxel? voxel = EditingVolume.GetVoxelAtWorldPosition(vector);

            if (Tool != EVolumeEditorTool.None && voxel != null && Input.GetMouseButtonDown(0))
            {
                if (Tool == EVolumeEditorTool.AddVoxel)
                {
                    Voxel newVox2 = voxel.Value;
                    newVox2.State = VoxelState.Active;
                    SetVoxelAtPosition(vector, newVox2);
                    return;
                }

                Voxel newVox = voxel.Value;
                newVox.State = VoxelState.Inactive;
                SetVoxelAtPosition(vector, newVox);
            }
        }

        public void SetVoxelAtPosition(Vector3 vector, Voxel vox)
        {
            if (IsFullyActivated)
            {
                if (Tool == EVolumeEditorTool.AddVoxel)
                {
                    Vector3 v = _camera.transform.forward * -0.05f;
                    EditingVolume.SetVoxelAtWorldPosition(vector + v, vox);
                    return;
                }
                EditingVolume.SetVoxelAtWorldPosition(vector, vox);
            }
        }

        public void SelectToolNone()
        {
            Tool = EVolumeEditorTool.None;
        }

        public void SelectToolAdd()
        {
            Tool = EVolumeEditorTool.AddVoxel;
        }

        public void SelectToolDelete()
        {
            Tool = EVolumeEditorTool.RemoveVoxel;
        }
    }
}