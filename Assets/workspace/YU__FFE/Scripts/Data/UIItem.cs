using UnityEngine;
using UnityEngine.Serialization;

namespace workspace.YU__FFE.Scripts.Data {
    namespace workspace.YU__FFE.Scripts.Data {
        [System.Serializable]
        public class UIItem {
            [FormerlySerializedAs("UIKey")]
            public string uiKey;
            [FormerlySerializedAs("Prefab")]
            public GameObject prefab;
        }
    }

}
