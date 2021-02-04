using UnityEngine;

namespace IGDPD.MissionDemolitionPrototype
{
    public class FollowCam : MonoBehaviour
    {
        public static GameObject poi;

        [Header("Set Dynamic")]
        public float camZ;

        private void Awake()
        {
            camZ = transform.position.z;
        }

        private void FixedUpdate()
        {
            if (poi == null) return;

            var des = poi.transform.position;
            des.z = camZ;
            transform.position = des;
        }
    }
}