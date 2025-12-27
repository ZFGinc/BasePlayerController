using System.Collections.Generic;
using UnityEngine;
using ZFGinc.Player;

namespace ZFGinc.Utils
{
    public class ParticleCollisionDetect : MonoBehaviour
    {
        public ParticleSystem part;
        public List<ParticleCollisionEvent> collisionEvents;

        void Start()
        {
            part = GetComponent<ParticleSystem>();
            collisionEvents = new List<ParticleCollisionEvent>();
        }

        void OnParticleCollision(GameObject other)
        {
            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

            if (!other.TryGetComponent(out FirstPersonCharacter _)) return;

            Rigidbody rb = other.GetComponent<Rigidbody>();
            int i = 0;

            UnityEngine.Debug.LogError("Particle Collision Detected");

            while (i < numCollisionEvents)
            {
                if (rb)
                {
                    Vector3 pos = collisionEvents[i].intersection;
                    Vector3 force = collisionEvents[i].velocity * 10;
                    rb.AddForce(force);
                }
                i++;
            }
        }
    }
}