using System.Collections.Generic;
using System.Diagnostics;

namespace CDOverhaul.DevTools
{
    public static class OverhaulProfiler
    {
        private static readonly Dictionary<string, (long, long)> s_ProfilerEntries = new Dictionary<string, (long, long)>();

        public static int GetEntriesCount() => s_ProfilerEntries.Count;
        public static string GetEntry(int index)
        {
            if (s_ProfilerEntries.IsNullOrEmpty())
                return string.Empty;

            int i = 0;
            foreach (string str in s_ProfilerEntries.Keys)
            {
                if (i == index)
                    return str;
                i++;
            }
            return string.Empty;
        }
        public static long GetEntryTicks(string entry) => s_ProfilerEntries[entry].Item1;
        public static long GetEntryMs(string entry) => s_ProfilerEntries[entry].Item2;

        public static void SetTime(string entryId, long ticks, long ms)
        {
            if (!s_ProfilerEntries.ContainsKey(entryId))
            {
                s_ProfilerEntries.Add(entryId, (ticks, ms));
                return;
            }
            s_ProfilerEntries[entryId] = (ticks, ms);
        }

        public static Stopwatch StartTimer() => OverhaulVersion.IsDebugBuild ? Stopwatch.StartNew() : null;
        public static void StopTimer(this Stopwatch stopwatch, string entryId)
        {
            if (stopwatch == null)
                return;

            stopwatch.Stop();
            SetTime(entryId, stopwatch.ElapsedTicks, stopwatch.ElapsedMilliseconds);
        }
    }
}
