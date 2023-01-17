namespace TurnTheGameOn.SimpleTrafficSystem
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEditor.SceneManagement;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(AITrafficWaypointRoute))]
    public class Editor_AITrafficWaypointRoute : Editor
    {
        AITrafficWaypointRoute circuit;
        ReorderableList reorderableList;
        float lineHeight;
        float lineHeightSpace;
        private bool showWaypoints;

        void OnEnable()
        {
            lineHeight = EditorGUIUtility.singleLineHeight;
            lineHeightSpace = lineHeight + 0;

            circuit = (AITrafficWaypointRoute)target;
            reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("waypointDataList"), false, true, false, true);
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);

                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), element.FindPropertyRelative("_name").stringValue);
                EditorGUI.indentLevel = 7;
                GUI.enabled = false;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * 0), rect.width, lineHeight), element.FindPropertyRelative("_transform"));
                GUI.enabled = true;
                EditorGUI.indentLevel = 0;
                reorderableList.elementHeightCallback = (int _index) => lineHeightSpace * 2;
            };
            reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "Waypoints");
            };
            reorderableList.onRemoveCallback = (ReorderableList list) =>
            {
                GameObject go = circuit.waypointDataList[reorderableList.index]._transform.gameObject;
                circuit.waypointDataList.RemoveAt(reorderableList.index);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins);
            EditorStyles.label.wordWrap = true;


            EditorGUILayout.HelpBox("Alt + Left Click in scene view on a Collider to add new points to the route", MessageType.None);
            EditorGUILayout.HelpBox("Alt + Ctrl + Left Click in scene view on a Collider to insert new points to the route", MessageType.None);

            SerializedProperty m_AITrafficGizmoSettings = serializedObject.FindProperty("m_AITrafficGizmoSettings");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_AITrafficGizmoSettings, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            showWaypoints = EditorGUILayout.Foldout(showWaypoints, "Waypoints", true);
            if (showWaypoints)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUI.BeginChangeCheck();
                reorderableList.DoLayoutList();
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndVertical();
            }

            SerializedProperty spawnTrafficVehicles = serializedObject.FindProperty("spawnTrafficVehicles");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(spawnTrafficVehicles, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            SerializedProperty useSpawnPoints = serializedObject.FindProperty("useSpawnPoints");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(useSpawnPoints, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Reverse Waypoints"))
            {
                circuit.ReversePoints();
                EditorUtility.SetDirty(this);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            if (GUILayout.Button("Align Waypoints"))
            {
                circuit.AlignPoints();
                EditorUtility.SetDirty(this);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        void OnSceneGUI()
        {
            AITrafficWaypointRoute waypointManager = (AITrafficWaypointRoute)target;

            for (int i = 0; i < waypointManager.waypointDataList.Count; i++)
            {
                if (waypointManager.waypointDataList[i]._waypoint)
                {
                    GUIStyle style = new GUIStyle();
                    string target = "";
                    style.normal.textColor = Color.green;
                    Handles.Label(waypointManager.waypointDataList[i]._waypoint.transform.position + new Vector3(0, 0.25f, 0),
                    "    Waypoint:   " + waypointManager.waypointDataList[i]._waypoint.onReachWaypointSettings.waypointIndexnumber.ToString() + "\n" +
                    "    SpeedLimit: " + waypointManager.waypointDataList[i]._waypoint.onReachWaypointSettings.speedLimit + "\n" +
                    target,
                    style
                    );
                }
            }

            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0 && e.alt && e.button == 0 && e.control)
            {
                Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(worldRay, out hitInfo))
                {
                    waypointManager.ClickToInsertSpawnNextWaypoint(hitInfo.point);
                }
            }
            else if (e.type == EventType.MouseDown && e.button == 0 && e.alt)
            {
                Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(worldRay, out hitInfo))
                {
                    waypointManager.ClickToSpawnNextWaypoint(hitInfo.point);
                }
            }



        }

    }
}