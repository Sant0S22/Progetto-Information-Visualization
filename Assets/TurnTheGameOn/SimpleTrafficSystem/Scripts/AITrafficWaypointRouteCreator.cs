namespace TurnTheGameOn.SimpleTrafficSystem
{
    using System.Collections.Generic;
    using UnityEngine;

    [ExecuteInEditMode]
    public class AITrafficWaypointRouteCreator : MonoBehaviour
    {
        public Transform startControlPoint;
        public Transform endControlPoint;
        public List<Transform> controlPointsList;
        public enum Routes
        {
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9,
            Ten = 10
        }
        public Routes routes;
        [System.Serializable]
        public class RouteSettings
        {
            public AITrafficWaypointRoute route;
            public Vector3 offset;
        }
        public List<RouteSettings> routeSettings;

        public int spawnedPoints;
        public bool loopSpline = false;
        [Range(0.001f, 1f)] public float waypointFrequency = 0.1f;
        public bool requiresUpdate;
        public List<Vector3> previousOffset;
        public List<Vector3> defaultOffset;


        public bool isInitialized;

        private void Start()
        {
            if (!isInitialized)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            transform.position = Vector3.zero;
            startControlPoint = new GameObject("startControlPoint").transform;
            endControlPoint = new GameObject("endControlPoint").transform;
            startControlPoint.transform.SetParent(transform, true);
            endControlPoint.transform.SetParent(transform, true);
            controlPointsList = new List<Transform>();
            controlPointsList.Add(startControlPoint);
            controlPointsList.Add(endControlPoint);
            routeSettings = new List<RouteSettings>();
            spawnedPoints = 0;
            isInitialized = true;
        }

    }
}