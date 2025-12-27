using UnityEngine;
using UnityEngine.Events;

namespace ZFGinc.Utils
{
    public class OnTriggerExitInvoker : MonoBehaviour
    {
        public UnityEvent OnExit;

        public void Triggered()
        {
            OnExit?.Invoke();
        }
    }
}