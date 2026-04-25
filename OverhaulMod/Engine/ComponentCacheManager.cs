using OverhaulMod.Content.Personalization;
using OverhaulMod.Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class ComponentCacheManager : Singleton<ComponentCacheManager>
    {
        private CacheForGetComponent<CameraModeController> _cacheForCameraModeControllerComponent;

        private CacheForGetComponent<CharacterExtension> _cacheForCharacterExtensionComponent;

        private CacheForGetComponent<PersonalizationController> _cacheForPersonalizationControllerComponent;

        private CacheForGetComponent<PersonalizationAccessoryReferences> _cacheForPersonalizationAccessoryReferencesComponent;

        private CacheForGetComponent<SwordHitArea> _cacheForSwordHitArea;

        public override void Awake()
        {
            base.Awake();

            _cacheForCameraModeControllerComponent = new CacheForGetComponent<CameraModeController>();
            _cacheForCharacterExtensionComponent = new CacheForGetComponent<CharacterExtension>();
            _cacheForPersonalizationControllerComponent = new CacheForGetComponent<PersonalizationController>();
            _cacheForPersonalizationAccessoryReferencesComponent = new CacheForGetComponent<PersonalizationAccessoryReferences>();
            _cacheForSwordHitArea = new CacheForGetComponent<SwordHitArea>();
        }

        public void ClearCache()
        {
            _cacheForCameraModeControllerComponent.Clear();
            _cacheForCharacterExtensionComponent.Clear();
            _cacheForPersonalizationControllerComponent.Clear();
            _cacheForPersonalizationAccessoryReferencesComponent.Clear();
            _cacheForSwordHitArea.Clear();
        }

        public CameraModeController GetCameraModeController(Transform transform)
        {
            return _cacheForCameraModeControllerComponent.GetScript(transform);
        }

        public CharacterExtension GetCharacterExtension(Transform transform)
        {
            return _cacheForCharacterExtensionComponent.GetScript(transform);
        }

        public PersonalizationController GetPersonalizationController(Transform transform)
        {
            return _cacheForPersonalizationControllerComponent.GetScript(transform);
        }

        public PersonalizationAccessoryReferences GetPersonalizationAccessoryReferences(Transform transform)
        {
            return _cacheForPersonalizationAccessoryReferencesComponent.GetScript(transform);
        }

        public SwordHitArea GetSwordHitArea(Transform transform)
        {
            return _cacheForSwordHitArea.GetScript(transform);
        }

        protected class CacheForGetComponent<T>
        {
            private readonly Dictionary<Transform, T> _transformToObject;

            public CacheForGetComponent()
            {
                _transformToObject = new Dictionary<Transform, T>();
            }

            public T GetScript(Transform transform)
            {
                if (_transformToObject.ContainsKey(transform))
                {
                    T component1 = _transformToObject[transform];
                    if (component1 == null)
                    {
                        component1 = transform.GetComponent<T>();
                        _transformToObject[transform] = component1;
                    }
                    return component1;
                }

                T component = transform.GetComponent<T>();
                _transformToObject.Add(transform, component);
                return component;
            }

            public void Clear()
            {
                _transformToObject.Clear();
            }
        }
    }
}
