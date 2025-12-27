using UnityEngine;
using UnityEngine.Events;

namespace ZFGinc.Utils
{
    public class OnTriggerEnterInvoker : MonoBehaviour
    {
        public UnityEvent OnEnter;

        public void Triggered()
        {
            OnEnter?.Invoke();
        }
    }
}