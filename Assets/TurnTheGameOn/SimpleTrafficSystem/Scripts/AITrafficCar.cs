namespace TurnTheGameOn.SimpleTrafficSystem
{
    using UnityEngine;

    public class AITrafficCar : MonoBehaviour
    {
        public int assignedIndex { get; private set; }
        public float topSpeed = 25f;
        public Transform frontSensorTransform;
        public Transform leftSensorTransform;
        public Transform rightSensorTransform;
        public AITrafficCarWheels[] _wheels;
        public Material brakeMaterial;
        public Light headLight;

        #region Get Array Data
        public float AccelerationInput()
        {
            return AITrafficController.Instance.GetAccelerationInput(assignedIndex);
        }

        public float SteeringInput()
        {
            return AITrafficController.Instance.GetSteeringInput(assignedIndex);
        }
        #endregion

        public void RegisterCar(AITrafficWaypointRoute route)
        {
            if (brakeMaterial == null)
            {
                MeshRenderer mesh = GetComponent<MeshRenderer>();
                brakeMaterial = mesh.material;
            }
            assignedIndex = AITrafficController.Instance.RegisterCarAI(this, route);
        }

        #region Waypoint Trigger Methods
        public void OnReachedWaypoint(AITrafficWaypoint.AITrafficCarOnReachWaypointInfo onReachWaypointSettings)
        {
            if (onReachWaypointSettings.parentRoute == AITrafficController.Instance.waypointRouteList[assignedIndex])
            {
                onReachWaypointSettings.OnReachWaypointEvent.Invoke();
                AITrafficController.Instance.Set_SpeedLimitArray(assignedIndex, onReachWaypointSettings.speedLimit);
                AITrafficController.Instance.Set_RouteProgressArray(assignedIndex, onReachWaypointSettings.waypointIndexnumber - 1);
                AITrafficController.Instance.Set_WaypointDataListCountArray(assignedIndex);
                if (onReachWaypointSettings.newRoutePoints.Length > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, onReachWaypointSettings.newRoutePoints.Length);
                    if (randomIndex == onReachWaypointSettings.newRoutePoints.Length) randomIndex -= 1;
                    AITrafficController.Instance.Set_WaypointRoute(assignedIndex, onReachWaypointSettings.newRoutePoints[randomIndex].onReachWaypointSettings.parentRoute);
                    AITrafficController.Instance.Set_RouteInfo(assignedIndex, onReachWaypointSettings.newRoutePoints[randomIndex].onReachWaypointSettings.parentRoute.routeInfo);
                    AITrafficController.Instance.Set_RouteProgressArray(assignedIndex, onReachWaypointSettings.newRoutePoints[randomIndex].onReachWaypointSettings.waypointIndexnumber - 1);
                    AITrafficController.Instance.Set_CurrentRoutePointIndexArray
                        (
                        assignedIndex,
                        onReachWaypointSettings.newRoutePoints[randomIndex].onReachWaypointSettings.waypointIndexnumber,
                        onReachWaypointSettings.newRoutePoints[randomIndex].onReachWaypointSettings.waypoint
                        );
                }
                else if (onReachWaypointSettings.waypointIndexnumber < onReachWaypointSettings.parentRoute.waypointDataList.Count)
                {
                    AITrafficController.Instance.Set_CurrentRoutePointIndexArray
                        (
                        assignedIndex,
                        onReachWaypointSettings.waypointIndexnumber,
                        onReachWaypointSettings.waypoint
                        );
                }
                AITrafficController.Instance.Set_RoutePointPositionArray(assignedIndex);
                if (onReachWaypointSettings.stopDriving) StopDriving();
            }
        }

        public void ChangeToRouteWaypoint(AITrafficWaypoint.AITrafficCarOnReachWaypointInfo onReachWaypointSettings)
        {
            onReachWaypointSettings.OnReachWaypointEvent.Invoke();
            AITrafficController.Instance.Set_SpeedLimitArray(assignedIndex, onReachWaypointSettings.speedLimit);
            AITrafficController.Instance.Set_WaypointDataListCountArray(assignedIndex);
            AITrafficController.Instance.Set_WaypointRoute(assignedIndex, onReachWaypointSettings.parentRoute);
            AITrafficController.Instance.Set_RouteInfo(assignedIndex, onReachWaypointSettings.parentRoute.routeInfo);
            AITrafficController.Instance.Set_RouteProgressArray(assignedIndex, onReachWaypointSettings.waypointIndexnumber - 1);
            AITrafficController.Instance.Set_CurrentRoutePointIndexArray
                (
                assignedIndex,
                onReachWaypointSettings.waypointIndexnumber,
                onReachWaypointSettings.waypoint
                );

            AITrafficController.Instance.Set_RoutePointPositionArray(assignedIndex);
        }
        #endregion

        #region Utility Methods
        [ContextMenu("StartDriving")]
        public void StartDriving()
        {
            AITrafficController.Instance.Set_IsDrivingArray(assignedIndex, true);
        }
        [ContextMenu("StopDriving")]
        public void StopDriving()
        {
            AITrafficController.Instance.Set_IsDrivingArray(assignedIndex, false);
        }
        [ContextMenu("MoveCarToPool")]
        public void MoveCarToPool()
        {
            AITrafficController.Instance.MoveCarToPool(assignedIndex);
        }
        #endregion

        #region Callbacks
        void OnBecameInvisible()
        {
            AITrafficController.Instance.SetVisibleState(assignedIndex, false);
        }

        void OnBecameVisible()
        {
            AITrafficController.Instance.SetVisibleState(assignedIndex, true);
        }
        #endregion
    }
}