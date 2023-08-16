using System;
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

                UnityEngine.Object @object = OverhaulAssetsController.GetAsset<UnityEngine.Object>(asset.AssetName, asset.AssetBundle, asset.FixMaterials);
                if(@object is Texture2D && asset.FieldReference.FieldType == typeof(Sprite))
                {
                    @object = (@object as Texture2D).ToSprite();
                }

                asset.FieldReference.SetValue(null, @object);
            }

            if (assetsToLoadAsync.IsNullOrEmpty())
                return;

            foreach (OverhaulAssetAttribute asset in assetsToLoadAsync)
                _ = StaticCoroutineRunner.StartStaticCoroutine(loadBundleThenAssetCoroutine(asset));
        }

        private static IEnumerator loadBundleThenAssetCoroutine(OverhaulAssetAttribute asset)
        {
            if (!OverhaulAssetsController.IsLoadingAssetBundle(asset.AssetBundle) && !OverhaulAssetsController.HasLoadedAssetBundle(asset.AssetBundle))
                _ = OverhaulAssetsController.LoadAssetBundleAsync(asset.AssetBundle, delegate { }, false);
            yield return new WaitUntil(() => OverhaulAssetsController.HasLoadedAssetBundle(asset.AssetBundle));

            bool hasLoadedAsset = false;
            OverhaulAssetsController.AssetLoadHandler handler = null;
            _ = OverhaulAssetsController.GetAssetAsync(asset.AssetBundle, asset.AssetName, delegate (OverhaulAssetsController.AssetLoadHandler h)
            {
                hasLoadedAsset = true;
                handler = h;
            });
            yield return new WaitUntil(() => hasLoadedAsset);
            UnityEngine.Object @object = (handler.Request as AssetBundleRequest).asset;
            if (@object is Texture2D && asset.FieldReference.FieldType == typeof(Sprite))
            {
                @object = (@object as Texture2D).ToSprite();
            }
            asset.FieldReference.SetValue(null, @object);
            yield break;
        }

        [OverhaulAsset(OverhaulAssetsController.ModAssetBundle_Fonts, "triggering_fanfares", false, false)]
        public static Font TriggeringFanFaresFont;

        [OverhaulAsset(OverhaulAssetsController.ModAssetBundle_Part2, "HQ-Question-256x256", false, false)]
        public static Sprite HQQuestionSprite;
    }
}
