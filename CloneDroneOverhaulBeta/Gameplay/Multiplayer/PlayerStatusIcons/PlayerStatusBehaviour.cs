using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay.Multiplayer
{
    public class PlayerStatusBehaviour : OverhaulCharacterExpansion
    {
        private static PlayerStatusBehaviour m_LocalBehaviour;
        private OverhaulModdedPlayerInfo m_Info;

        private GameObject m_WorldCanvasHolder;
        private PlayerStatusWorldCanvas m_WorldCanvas;
        public bool IsCanvasCreated => base.enabled && m_WorldCanvas != null && m_WorldCanvasHolder != null;

        public int TargetStatus;

        public override void Start()
        {
            base.Start();

            if (!GameModeManager.IsMultiplayer())
            {
                base.enabled = false;
                return;
            }

            m_Info = OverhaulModdedPlayerInfo.GetPlayerInfo(Owner);
            if (m_Info != null && m_Info.Equals(OverhaulModdedPlayerInfo.GetLocalPlayerInfo()))
            {
                m_LocalBehaviour = this;
            }

            _ = OverhaulEventsController.AddEventListener<Hashtable>(OverhaulModdedPlayerInfo.InfoReceivedEventString, onGetData);
            CreateCanvas();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            OverhaulEventsController.RemoveEventListener<Hashtable>(OverhaulModdedPlayerInfo.InfoReceivedEventString, onGetData);
            m_Info = null;
            m_WorldCanvas = null;
            m_WorldCanvasHolder = null;
        }

        protected override void OnDeath()
        {
            if (m_WorldCanvas != null)
            {
                m_WorldCanvas.DestroyGameObject();
            }
            if (m_WorldCanvasHolder != null)
            {
                Destroy(m_WorldCanvasHolder);
            }
            Destroy(this);
        }

        public void SetStatus(PlayerStatusType type)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            if (m_Info == null) m_Info = OverhaulModdedPlayerInfo.GetPlayerInfo(Owner);

            int intValue = (int)type;
            if (m_Info != null && m_Info.HasReceivedData)
            {
                TargetStatus = intValue;
                m_Info.RefreshData();
            }
        }

        public void CreateCanvas()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (m_WorldCanvas != null)
            {
                m_WorldCanvas.DestroyGameObject();
            }
            if (m_WorldCanvasHolder != null)
            {
                Destroy(m_WorldCanvasHolder);
            }

            m_WorldCanvas = null;
            m_WorldCanvasHolder = null;

            GameObject holder = new GameObject("StatusCanvasHolder");
            holder.transform.SetParent(base.transform, false);
            holder.transform.localPosition = new Vector3(0, 5, 0);
            m_WorldCanvasHolder = holder;

            GameObject prefab = OverhaulAssetsController.GetAsset("PlayStatus", OverhaulAssetPart.Part2);
            GameObject instantiatedPrefab = Instantiate(prefab, m_WorldCanvasHolder.transform);
            instantiatedPrefab.transform.localScale = Vector3.one * 0.02f;
            instantiatedPrefab.transform.localPosition = Vector3.zero;
            m_WorldCanvas = instantiatedPrefab.AddComponent<PlayerStatusWorldCanvas>();
            m_WorldCanvas.Initialize();
        }

        private void onGetData(Hashtable table)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (table == null || Owner == null)
            {
                return;
            }

            string playFabID = Owner.GetPlayFabID();
            if (string.IsNullOrEmpty(playFabID) || !playFabID.Equals(table["ID"]))
            {
                return;
            }

            int status = 0;
            if (!IsOwnerMainPlayer() && table.Contains("State.Status"))
            {
                try
                {
                    status = (int)(long)table["State.Status"];
                }
                catch
                {
                    status = 0;
                }
            }

            if (IsCanvasCreated)
            {
                m_WorldCanvas.SetText(GetTextForStatus(status));
            }
        }

        public string GetTextForStatus(int value)
        {
            return GetTextForStatusStatic(value);
        }

        public static string GetTextForStatusStatic(int value)
        {
            switch (value)
            {
                case 1:
                    return "Selecting skins";
                default:
                    return string.Empty;
            }
        }

        public static void SetOwnStatus(PlayerStatusType type)
        {
            if (m_LocalBehaviour == null)
            {
                return;
            }
            m_LocalBehaviour.SetStatus(type);
        }

        public static int GetOwnStatus()
        {
            return m_LocalBehaviour == null ? -1 : m_LocalBehaviour.TargetStatus;
        }
    }
}