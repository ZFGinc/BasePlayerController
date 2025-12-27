using UnityEngine;

namespace ZFGinc.Utils
{
    public class OnTriggerListener : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out OnTriggerEnterInvoker invoker))
            {
                invoker.Triggered();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out OnTriggerExitInvoker invoker))
            {
                invoker.Triggered();
            }
        }
    }
}