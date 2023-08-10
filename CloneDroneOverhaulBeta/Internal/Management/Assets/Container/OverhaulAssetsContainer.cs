using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulAssetsContainer
    {
        private static bool s_HasLoaded;

        public static void Initialize()
        {
            if (s_HasLoaded)
                return;

            s_HasLoaded = true;

            List<OverhaulAssetAttribute> assetsToLoadAsync = new List<OverhaulAssetAttribute>();
            List<OverhaulAssetAttribute> fields = typeof(OverhaulAssetsContainer).GetFieldReferencingAttributes<OverhaulAssetAttribute>(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach (OverhaulAssetAttribute asset in fields)
            {
                if (asset.AsyncLoad)
                {
                    assetsToLoadAsync.Add(asset);
                    continue;
                }

                Object @object = OverhaulAssetsController.GetAsset<UnityEngine.Object>(asset.AssetName, asset.AssetBundle, asset.FixMaterials);
                asset.FieldReference.SetValue(null, @object);
            }

            if (assetsToLoadAsync.IsNullOrEmpty())
                return;

            foreach (OverhaulAssetAttribute asset in assetsToLoadAsync)
                StaticCoroutineRunner.StartStaticCoroutine(loadBundleThenAssetCoroutine(asset));
        }

        private static IEnumerator loadBundleThenAssetCoroutine(OverhaulAssetAttribute asset)
        {
            if (!OverhaulAssetsController.IsLoadingAssetBundle(asset.AssetBundle) && !OverhaulAssetsController.HasLoadedAssetBundle(asset.AssetBundle))
                OverhaulAssetsController.LoadAssetBundleAsync(asset.AssetBundle, delegate { }, false);
            yield return new WaitUntil(() => OverhaulAssetsController.HasLoadedAssetBundle(asset.AssetBundle));

            bool hasLoadedAsset = false;
            OverhaulAssetsController.AssetLoadHandler handler = null;
            OverhaulAssetsController.GetAssetAsync(asset.AssetBundle, asset.AssetName, delegate (OverhaulAssetsController.AssetLoadHandler h)
            {
                hasLoadedAsset = true;
                handler = h;
            });
            yield return new WaitUntil(() => hasLoadedAsset);
            asset.FieldReference.SetValue(null, (handler.Request as AssetBundleRequest).asset);
            yield break;
        }
    }
}
