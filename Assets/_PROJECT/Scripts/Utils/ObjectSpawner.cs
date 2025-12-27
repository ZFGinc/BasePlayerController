using UnityEngine;

namespace ZFGinc.Utils
{
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Transform _target;

        public void Spawn()
        {
            Instantiate(_prefab, _target.position, Quaternion.identity);
        }

        public void Spawn(GameObject prefab)
        {
            Instantiate(prefab, _target.position, Quaternion.identity);
        }
    }
}