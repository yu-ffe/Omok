using UnityEngine;

namespace Game {
    public class DestroySelfAfterAnimation : MonoBehaviour
    {
        public float delay = 20f;

        void OnEnable()
        {
            Destroy(gameObject, delay);
        }
    }
}
