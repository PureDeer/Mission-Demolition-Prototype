using UnityEngine;

namespace IGDPD.MissionDemolitionPrototype
{
    public class Slingshot : MonoBehaviour
    {
        [Header("Set in Inspector")]
        public GameObject prefabProjectile;

        public float velocityFactor = 8f;

        [Header("Set Dynamically")]
        public GameObject launchPoint;

        public Vector3 launchPos;
        public Quaternion launchRotation;
        public GameObject projectile;
        public bool aimingMode;

        private Rigidbody _projectileRigid;

        private void Awake()
        {
            var launchPointTrans = transform.Find("LaunchPoint");
            launchPoint = launchPointTrans.gameObject;
            launchPoint.SetActive(false);
            launchPos = launchPointTrans.position;
            launchRotation = launchPointTrans.rotation;
        }

        private void OnMouseEnter()
        {
            launchPoint.SetActive(true);
        }

        private void OnMouseExit()
        {
            launchPoint.SetActive(false);
        }

        private void OnMouseDown()
        {
            aimingMode = true;
            projectile = Instantiate(prefabProjectile, launchPos, launchRotation);
            _projectileRigid = projectile.GetComponent<Rigidbody>();
            _projectileRigid.isKinematic = true;
        }

        private void Update()
        {
            if (!aimingMode) return;
            // 将鼠标在当前窗口的位置转为3d坐标
            var mousePos2D = Input.mousePosition;
            mousePos2D.z = -Camera.main.transform.position.z;
            var mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

            // 计算发射点到鼠标位置的坐标差，是一个向量，代表着发射的相反方向
            var launchDirection = mousePos3D - launchPos;
            // 限制子弹在弹弓在的碰撞器的半径内
            var maxMagnitude = GetComponent<SphereCollider>().radius;
            if (launchDirection.magnitude > maxMagnitude)
            {
                launchDirection.Normalize();
                launchDirection *= maxMagnitude;
            }

            // 在被限制在半径之内的情况下，将子弹移动到发射点与鼠标位置的连线上
            var projPos = launchPos + launchDirection;
            projectile.transform.position = projPos;

            if (Input.GetMouseButtonUp(0))
            {
                aimingMode = false;
                _projectileRigid.isKinematic = false;
                // 将发射方向和速率赋给子弹的刚体速度
                _projectileRigid.velocity = -launchDirection * velocityFactor;
                // 摄像机追踪子弹
                FollowCam.poi = projectile;
                projectile = null;
            }
        }
    }
}