namespace TurnTheGameOn.SimpleTrafficSystem
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;

    public class Editor_RouteConnectorWindow : EditorWindow
    {
        public static Editor_RouteConnectorWindow editorWindow;

        Vector3 offset;
        Vector3 _position;
        float size;
        float pickSize;
        GUIStyle style;
        Transform sceneViewCameraTransform;
        Vector3 pointTransformPosition;
        Vector3 screenPoint;
        bool onScreen;

        [MenuItem("Tools/Simple Traffic System/Route Connector Window")]
        public static void ShowWindow()
        {
            Editor_RouteConnectorWindow window = (Editor_RouteConnectorWindow)GetWindow(typeof(Editor_RouteConnectorWindow));
            window.minSize = new Vector2(300, 200);
            window.ClearData(true);
            window.titleContent.text = "Route Connector";
            window.Show();
        }

        public AITrafficWaypointRoute[] routesToEdit;
        public AITrafficWaypoint fromPoint;
        public AITrafficWaypoint toPoint;
        public int fromRouteIndex = -1;
        public int fromPointIndex = -1;
        public int toRouteIndex = -1;
        public int toPointIndex = -1;

        bool showDebug = true;
        Vector2 scrollPos = new Vector2();

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MaxWidth(1000), GUILayout.MaxHeight(1000));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Routes"))
            {
                routesToEdit = FindObjectsOfType<AITrafficWaypointRoute>();
            }

            if (routesToEdit.Length > 0)
            {
                showDebug = EditorGUILayout.Toggle("Debug", showDebug);
            }
            EditorGUILayout.EndHorizontal();

            SerializedObject serialObj = new SerializedObject(this);
            if (showDebug)
            {
                GUI.enabled = false;

                SerializedProperty routesToEditProperty = serialObj.FindProperty("routesToEdit");
                EditorGUILayout.PropertyField(routesToEditProperty, true);

                SerializedProperty fromRouteIndexProperty = serialObj.FindProperty("fromRouteIndex");
                EditorGUILayout.PropertyField(fromRouteIndexProperty, true);

                SerializedProperty fromPointIndexProperty = serialObj.FindProperty("fromPointIndex");
                EditorGUILayout.PropertyField(fromPointIndexProperty, true);

                SerializedProperty toRouteIndexProperty = serialObj.FindProperty("toRouteIndex");
                EditorGUILayout.PropertyField(toRouteIndexProperty, true);

                SerializedProperty toPointIndexProperty = serialObj.FindProperty("toPointIndex");
                EditorGUILayout.PropertyField(toPointIndexProperty, true);

                GUI.enabled = true;
            }

            if (routesToEdit.Length > 0)
            {
                if (GUILayout.Button("Unload Routes"))
                {
                    ClearData(true);
                    EditorUtility.SetDirty(this);
                    Repaint();
                }

                GUI.enabled = false;

                SerializedProperty fromPointProperty = serialObj.FindProperty("fromPoint");
                if (fromRouteIndex == -1)
                    EditorGUILayout.HelpBox("Select 'F' handle to assign 'From Point'", MessageType.Info);
                EditorGUILayout.PropertyField(fromPointProperty, true);

                SerializedProperty toPointProperty = serialObj.FindProperty("toPoint");
                if (toRouteIndex == -1)
                    EditorGUILayout.HelpBox("Select 'T' handle to assign 'To Point'", MessageType.Info);
                EditorGUILayout.PropertyField(toPointProperty, true);

                GUI.enabled = true;
                if (fromPoint != null && toPoint != null)
                {
                    if (GUILayout.Button("Connect Route Points"))
                    {
                        AITrafficWaypoint[] currentArray = new AITrafficWaypoint[fromPoint.onReachWaypointSettings.newRoutePoints.Length + 1];
                        for (int i = 0; i < fromPoint.onReachWaypointSettings.newRoutePoints.Length; i++)
                        {
                            currentArray[i] = fromPoint.onReachWaypointSettings.newRoutePoints[i];
                        }
                        currentArray[currentArray.Length - 1] = toPoint;
                        fromPoint.onReachWaypointSettings.newRoutePoints = currentArray;

                        EditorUtility.SetDirty(fromPoint);
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                        ClearData(false);
                    }
                }
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        void ClearData(bool clearRoutes)
        {
            if (clearRoutes) routesToEdit = new AITrafficWaypointRoute[0];
            fromRouteIndex = -1;
            fromPointIndex = -1;
            toRouteIndex = -1;
            toPointIndex = -1;
            fromPoint = null;
            toPoint = null;
            showDebug = false;
        }

        void OnFocus()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI;
            SceneView.duringSceneGui += this.OnSceneGUI;
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI;
        }

        void OnSceneGUI(SceneView sceneView)
        {
            sceneViewCameraTransform = Camera.current.transform;

            for (int i = 0; i < this.routesToEdit.Length; i++)
            {
                for (int j = 0; j < this.routesToEdit[i].waypointDataList.Count; j++)
                {
                    pointTransformPosition = this.routesToEdit[i].waypointDataList[j]._transform.position;

                    screenPoint = Camera.current.WorldToViewportPoint(pointTransformPosition);
                    onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

                    if (onScreen)
                    {
                        #region FROM Handle
                        offset = new Vector3(0, 2f, 0);
                        _position = pointTransformPosition + offset * 2f;
                        size = 0.4f;
                        pickSize = size * 1;
                        style = new GUIStyle();
                        style.normal.textColor = (this.fromRouteIndex == i) && (this.fromPointIndex == j) ? Color.yellow : Color.grey;
                        style.alignment = TextAnchor.MiddleCenter;
                        style.fontStyle = FontStyle.Bold;
                        style.fontSize = 12;
                        Handles.color = Color.grey;

                        if (Handles.Button(_position, Quaternion.LookRotation(sceneViewCameraTransform.forward, sceneViewCameraTransform.up), size, pickSize, Handles.RectangleHandleCap))
                        {
                            this.fromRouteIndex = i;
                            this.fromPointIndex = j;
                            this.fromPoint = this.routesToEdit[i].waypointDataList[j]._waypoint;
                            Repaint();
                        }

                        offset = new Vector3(0, 2.03f, 0);
                        _position = pointTransformPosition + offset * 2f;
                        Handles.Label
                        (
                            _position,
                            "F",
                            style
                        );
                        #endregion

                        #region TO Handle
                        offset = new Vector3(0, 1, 0);
                        _position = pointTransformPosition + offset * 2f;
                        style.normal.textColor = (this.toRouteIndex == i) && (this.toPointIndex == j) ? Color.yellow : Color.grey;

                        if (Handles.Button(_position, Quaternion.LookRotation(sceneViewCameraTransform.forward, sceneViewCameraTransform.up), size, pickSize, Handles.RectangleHandleCap))
                        {
                            this.toRouteIndex = i;
                            this.toPointIndex = j;
                            this.toPoint = this.routesToEdit[i].waypointDataList[j]._waypoint;
                            Repaint();
                        }

                        offset = new Vector3(0, 1.03f, 0);
                        _position = pointTransformPosition + offset * 2f;
                        Handles.Label
                        (
                            _position,
                            "T",
                            style
                        );
                        #endregion
                    }
                }
            }

            SceneView.RepaintAll();
        }

    }
}