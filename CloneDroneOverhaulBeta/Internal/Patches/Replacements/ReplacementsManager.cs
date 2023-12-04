using System.Collections;
using UnityEngine;

namespace CDOverhaul.Patches
{
    public class ReplacementsManager : OverhaulManager<ReplacementsManager>
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void OnSceneReloaded()
        {
            base.OnSceneReloaded();
            AddReplacements();
        }

        protected override void OnAssetsLoaded()
        {
            base.OnAssetsLoaded();
            AddReplacements();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            RemoveReplacements();
        }

        public void AddReplacements(bool force = false)
        {
            if (!force)
            {
                _ = StartCoroutine(addReplacementsCoroutine());
                return;
            }
            ReplacementBase.AddAll();
        }

        private IEnumerator addReplacementsCoroutine()
        {
            yield return new WaitUntil(() => !OverhaulLocalizationManager.Error);
            yield return null;
            ReplacementBase.AddAll();
            yield break;
        }

        public void RemoveReplacements()
        {
            ReplacementBase.RemoveAll();
        }
    }
}
