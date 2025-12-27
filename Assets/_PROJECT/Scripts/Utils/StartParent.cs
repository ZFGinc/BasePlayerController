using UnityEngine;

namespace ZFGinc.Utils
{
    public class StartParent : MonoBehaviour
    {
        public Transform NewParent = null;

        private void Start()
        {
            transform.parent = NewParent;
        }
    }
}