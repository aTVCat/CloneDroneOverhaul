using ModLibrary;
using UnityEngine;

namespace CloneDroneOverhaul.Modules
{
    public class PointLightDust : MonoBehaviour
    {
        public Transform Target;
        private Transform Dust;

        private void Start()
        {
            Dust = Instantiate<Transform>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustLight").transform);
        }

        private void FixedUpdate()
        {
            Dust.transform.position = Target.position;
        }

        private void OnDestroy()
        {
            if (Dust != null)
            {
                Destroy(Dust.gameObject);
            }
        }

        private void OnDisable()
        {
            if (Dust != null)
            {
                Dust.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (Dust != null)
            {
                Dust.gameObject.SetActive(true);
            }
        }
    }

    public enum ShadowResolution
    {
        Low,
        Default,
        High,
        ExtremlyHigh
    }

    public enum ShadowBias
    {
        Minimum,
        Low,
        Default,
    }

    public enum ShadowDistance
    {
        VeryLow,
        Low,
        Default,
        High,
        VeryHigh,
        ExtremlyHigh
    }

    public enum LightLimit
    {
        Low,
        Default,
        High,
        ExtremlyHigh
    }

    public enum AntialiasingLevel
    {
        Off,
        X2,
        X4,
        X8
    }
}
