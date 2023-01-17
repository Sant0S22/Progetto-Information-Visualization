namespace TurnTheGameOn.SimpleTrafficSystem
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;

    public class AITrafficWaypoint : MonoBehaviour
    {
        [System.Serializable]
        public struct AITrafficCarOnReachWaypointInfo
        {
            public AITrafficWaypointRoute parentRoute;
            public AITrafficWaypoint waypoint;
            public int waypointIndexnumber;
            public float speedLimit;
            public bool stopDriving;
            public AITrafficWaypoint[] newRoutePoints;
            public List<AITrafficWaypoint> laneChangePoints;
            public UnityEvent OnReachWaypointEvent;
            [HideInInspector] public Vector3 position;
        }
        public AITrafficCarOnReachWaypointInfo onReachWaypointSettings;

        private void OnEnable()
        {
            onReachWaypointSettings.position = transform.position;
            if (onReachWaypointSettings.waypointIndexnumber < onReachWaypointSettings.parentRoute.waypointDataList.Count)
            {
                onReachWaypointSettings.waypoint = this;
            }
        }

        void OnTriggerEnter(Collider col)
        {
            col.transform.root.SendMessage("OnReachedWaypoint", onReachWaypointSettings, SendMessageOptions.DontRequireReceiver);
            if (onReachWaypointSettings.waypointIndexnumber == onReachWaypointSettings.parentRoute.waypointDataList.Count)
            {
                if (onReachWaypointSettings.newRoutePoints.Length == 0)
                {
                    col.transform.root.SendMessage("StopDriving", SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        public void TriggerNextWaypoint(AITrafficCar _AITrafficCar)
        {
            _AITrafficCar.OnReachedWaypoint(onReachWaypointSettings);
            if (onReachWaypointSettings.waypointIndexnumber == onReachWaypointSettings.parentRoute.waypointDataList.Count)
            {
                if (onReachWaypointSettings.newRoutePoints.Length == 0)
                {
                    _AITrafficCar.StopDriving();
                }
            }
        }

    }
}