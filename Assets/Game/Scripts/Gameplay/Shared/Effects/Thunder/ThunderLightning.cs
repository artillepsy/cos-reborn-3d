
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Gameplay.Shared.Effects.Thunder
{
    public class ThunderLightning : MonoBehaviour
    {
        [SerializeField] private Light _directionalLight;
        
        [Header("Lightning")]
        [Space]
        [SerializeField] private float _lightningDelaySecMin = 10;
        [SerializeField] private float _lightningDelaySecMax = 30;
        [Space]
        [SerializeField] private uint _lightningTimesPerCastMin = 1;
        [SerializeField] private uint _lightningTimesPerCastMax = 4;
        [Space]
        [SerializeField] private float _lightningDelaySecPerCastMin = 0.1f;
        [SerializeField] private float _lightningDelaySecPerCastMax = 0.3f;
        [Space]
        [SerializeField] private float _lightningCastTimeSecMin = 0.1f;
        [SerializeField] private float _lightningCastTimeSecMax = 0.5f;
        [Space]
        [SerializeField] private float _lightningIntensityMin = 0.1f;
        [SerializeField] private float _lightningIntensityMax = 1f;
        [Space]
        [SerializeField] private float _lightningIntensityShadeMin = 0.5f;
        [SerializeField] private float _lightningIntensityShadeMax = 1f;

        private bool _isLightning = false;
        private bool _isDelay = false;
        
        private void Update()
        {
            if (_isDelay || _isLightning)
            {
                return;
            }
            StartCoroutine(LightningDelayCo());
            StartCoroutine(ExecuteLightningCo());
        }

        private IEnumerator LightningDelayCo()
        {
            _isDelay = true;
            yield return new WaitForSeconds(Random.Range(_lightningDelaySecMin, _lightningDelaySecMax));
            _isDelay = false;
        }

        private IEnumerator ExecuteLightningCo()
        {
            _isLightning = true;
            var lightningNumber = Random.Range((int)_lightningTimesPerCastMin, (int)_lightningTimesPerCastMax + 1);
            for (var i = 0; i < lightningNumber; i++)
            {
                _directionalLight.intensity = Random.Range(_lightningIntensityMin, _lightningIntensityMax);
                yield return 
                    new WaitForSeconds(Random.Range(_lightningCastTimeSecMin, _lightningCastTimeSecMax));
                _directionalLight.intensity -= Random.Range(_lightningIntensityShadeMin, _lightningIntensityShadeMax);
                if (_directionalLight.intensity < 0)
                {
                    _directionalLight.intensity = 0;
                }
                yield return 
                    new WaitForSeconds(Random.Range(_lightningDelaySecPerCastMin, _lightningDelaySecPerCastMax));
            }
            _directionalLight.intensity = 0;
            _isLightning = false;
        } 
        
    }
}
