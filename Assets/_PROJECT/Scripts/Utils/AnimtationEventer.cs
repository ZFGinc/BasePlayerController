using UnityEngine;
using UnityEngine.Events;

namespace ZFGinc.Utils
{
    public class AnimtationEventer : MonoBehaviour
    {
        public UnityEvent Event;

        public void Invoke()
        {
            Event?.Invoke();
        }
    }
}