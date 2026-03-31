using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;

namespace OverhaulMod.Patches.Behaviours
{
    internal class ChunkUpdateDelayPatchBehaviour : GamePatchBehaviour
    {
        private const float ABSENT_MULTIPLIER = 0f;
        private const float MINIMAL_MULTIPLIER = 0.4f;
        private const float OPTIMAL_MULTIPLIER = 1f;

        [ModSetting(ModSettingsConstants.CHUNK_UPDATE_DELAY, ChunkUpdateDelay.Absent)]
        public static ChunkUpdateDelay UpdateDelay;

        private List<MinWaitBetweenChunkUpdatesForSize> _chunkUpdatesRate;

        private List<MinWaitBetweenChunkUpdatesForSize> _defaultChunkUpdatesRate;

        private bool _isInitialized;

        public override void Patch()
        {
            _chunkUpdatesRate = AdaptivePerformanceManager.Instance.MinWaitBetweenChunkUpdatesForSizes;
            _defaultChunkUpdatesRate = new List<MinWaitBetweenChunkUpdatesForSize>();
            for (int i = 0; i < _chunkUpdatesRate.Count; i++)
            {
                _defaultChunkUpdatesRate.Add(new MinWaitBetweenChunkUpdatesForSize()
                {
                    FrameVolume = _chunkUpdatesRate[i].FrameVolume,
                    MinWait = _chunkUpdatesRate[i].MinWait,
                });
            }
            _isInitialized = true;

            Refresh();
        }

        public override void Unpatch()
        {
            if (!_isInitialized) return;

            for (int i = 0; i < _chunkUpdatesRate.Count; i++)
            {
                _chunkUpdatesRate[i].MinWait = _defaultChunkUpdatesRate[i].MinWait;
            }
        }

        public void Refresh()
        {
            if (!_isInitialized) return;

            float multiplier = getMultiplierForSetting(UpdateDelay);
            for (int i = 0; i < _chunkUpdatesRate.Count; i++)
            {
                _chunkUpdatesRate[i].MinWait = _defaultChunkUpdatesRate[i].MinWait * multiplier;
            }
        }

        private float getMultiplierForSetting(ChunkUpdateDelay chunkUpdateDelay)
        {
            switch (chunkUpdateDelay)
            {
                case ChunkUpdateDelay.Absent:
                    return ABSENT_MULTIPLIER;
                case ChunkUpdateDelay.Minimal:
                    return MINIMAL_MULTIPLIER;
            }
            return OPTIMAL_MULTIPLIER;
        }
    }
}