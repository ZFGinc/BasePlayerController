using NaughtyAttributes;
using System.Collections;
using UnityEngine;

namespace ZFGinc.Utils
{
    public class LightFlicker : MonoBehaviour
    {
        public bool StartOnStart = true;
        public string Pattern = "100110110110010110100101100101010";
        [Range(0.1f, 1f)] public float Interval = 0.3f;
        [Space]
        public Light Light;

        protected Coroutine coroutine;

        protected void Start()
        {
            if (StartOnStart) Restart();
        }

        protected virtual IEnumerator Flicker()
        {
            while (true)
            {
                foreach (char c in Pattern)
                {
                    Light.enabled = c == '1';
                    yield return new WaitForSeconds(Interval);
                }
            }
        }

        [Button]
        public void Restart()
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(Flicker());
        }

        [Button]
        public void Stop()
        {
            if (coroutine != null) StopCoroutine(coroutine);
        }
    }
}