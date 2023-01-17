namespace TurnTheGameOn.SimpleTrafficSystem
{
    using UnityEditor;
    using UnityEngine;

    public class Editor_UnityMenuBar : Editor
    {
        [MenuItem("Tools/Simple Traffic System/Route Creator")]
        private static void AITrafficWaypointRouteCreator()
        {
            GameObject _objectToSpawn = Instantiate(Resources.Load("AITrafficWaypointRouteCreator")) as GameObject;
            _objectToSpawn.name = "AITrafficWaypointRouteCreator";
            GameObject[] newSelection = new GameObject[1];
            newSelection[0] = _objectToSpawn;
            Selection.objects = newSelection;
        }

        [MenuItem("Tools/Simple Traffic System/AI Traffic Controller")]
        private static void SpawnAITrafficController()
        {
            GameObject _objectToSpawn = Instantiate(Resources.Load("AITrafficController")) as GameObject;
            _objectToSpawn.name = "AITrafficController";
            GameObject[] newSelection = new GameObject[1];
            newSelection[0] = _objectToSpawn;
            Selection.objects = newSelection;
        }

        [MenuItem("Tools/Simple Traffic System/AI Traffic Waypoint Route %&r")]
        private static void SpawnWaypointRoute()
        {
            GameObject _objectToSpawn = Instantiate(Resources.Load("AITrafficWaypointRoute")) as GameObject;
            _objectToSpawn.name = "AITrafficWaypointRoute";
            GameObject[] newSelection = new GameObject[1];
            newSelection[0] = _objectToSpawn;
            Selection.objects = newSelection;
        }

        [MenuItem("Tools/Simple Traffic System/AI Traffic Light Manager %&l")]
        private static void SpawnTrafficLightManager()
        {
            GameObject _objectToSpawn = Instantiate(Resources.Load("AITrafficLightManager")) as GameObject;
            _objectToSpawn.name = "AITrafficLightManager";
            GameObject[] newSelection = new GameObject[1];
            newSelection[0] = _objectToSpawn;
            Selection.objects = newSelection;
        }
    }
}