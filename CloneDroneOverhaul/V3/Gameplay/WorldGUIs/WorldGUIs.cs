using CloneDroneOverhaul.V3;
using CloneDroneOverhaul.V3.Base;
using CloneDroneOverhaul.V3.Utilities;
using ModLibrary;
using UnityEngine;

namespace CloneDroneOverhaul.V3.Gameplay
{
    public class WorldGUIs : V3_ModControllerBase
    {
        private Transform MindTransfers;
        private Transform MindTransferIcon;
        private Transform MindTransferContainer;

        private Transform MindTransferToFollow;

        private float YOffset;
        private Bounds _combinedBounds;
        private RobotShortInformation CurrentPlayerInfo;
        private RobotShortInformation OldPlayerInfo;

        private bool _isShowingMindtransfers;

        private void Start()
        {
            MindTransfers = UnityEngine.GameObject.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "MindTransfersLeft"), base.transform).transform;
            MindTransfers.gameObject.SetActive(false);
            MindTransferContainer = MindTransfers.GetComponent<ModdedObject>().GetObjectFromList<RectTransform>(1);
            MindTransferIcon = MindTransfers.GetComponent<ModdedObject>().GetObjectFromList<RectTransform>(0);
            MindTransfers.gameObject.AddComponent<FaceTowardsCamera>();
        }

        public override void OnEvent(in string eventName, in object[] args)
        {
            if (eventName == "onPlayerSet")
            {
                CurrentPlayerInfo = args[0] as RobotShortInformation;
                OldPlayerInfo = args[1] as RobotShortInformation;
                OverhaulMain.Timer.CompleteNextFrame(OnTransfer);
            }
        }

        private void OnTransfer()
        {
            if (GameModeManager.ConsciousnessTransferToKillerEnabled() && !CurrentPlayerInfo.IsNull && !_isShowingMindtransfers)
            {
                FirstPersonMover mover = (FirstPersonMover)CurrentPlayerInfo.Instance;

                Transform head = TransformUtils.FindChildRecursive(CurrentPlayerInfo.Instance.transform, "Head");
                MindTransferToFollow = head;

                MindTransferContainer.transform.localScale = Vector3.zero;
                iTween.ScaleTo(MindTransferContainer.gameObject, Vector3.one, 0.5f);

                YOffset = 1.4f;
                _combinedBounds = new Bounds();

                if (head == null || mover.CharacterType == EnemyType.EmperorCombat || mover.CharacterType == EnemyType.EmperorNonCombat)
                {
                    CloneDroneOverhaul.V3.HUD.UIMindTransfersLeftForEdgeCases.GetInstance<CloneDroneOverhaul.V3.HUD.UIMindTransfersLeftForEdgeCases>().RefreshMindTransfers();
                    MindTransferToFollow = null;
                    return;
                }

                RefreshMindTransfers();

                Renderer[] renderers = head.gameObject.GetComponentsInChildren<Renderer>();
                if (renderers.Length != 0)
                {
                    _combinedBounds = renderers[0].bounds;
                    int i = 1;
                    int len = renderers.Length;
                    while (i < len)
                    {
                        _combinedBounds.Encapsulate(renderers[i].bounds);
                        int num = i;
                        i = num + 1;
                    }
                }
                YOffset = Mathf.Clamp(_combinedBounds.center.y + 1.25f, 1.4f, 2.65f);
            }
        }

        private void RefreshMindTransfers()
        {
            _isShowingMindtransfers = true;

            TransformUtils.DestroyAllChildren(MindTransferContainer);
            MindTransferIcon.gameObject.Instantiate(MindTransferIcon.gameObject, MindTransferContainer, GameDataManager.Instance.GetNumConsciousnessTransfersLeft(), true);

            DelegateScheduler.Instance.Schedule(delegate
            {
                iTween.ScaleTo(MindTransferContainer.gameObject, Vector3.zero, 0.25f);

                DelegateScheduler.Instance.Schedule(delegate
                {
                    _isShowingMindtransfers = false;
                }, 0.3f);

            }, 5);
        }

        private void LateUpdate()
        {
            bool condition1 = GameModeManager.ConsciousnessTransferToKillerEnabled();
            bool work = MindTransferToFollow != null && condition1;

            if (work)
            {
                MindTransfers.position = new Vector3(MindTransferToFollow.position.x, MindTransferToFollow.position.y + YOffset, MindTransferToFollow.position.z);
            }

            if (condition1 && Input.GetKeyDown(KeyCode.Tab))
            {
                OnTransfer();
            }

            MindTransfers.gameObject.SetActive(work);
        }
    }
}
