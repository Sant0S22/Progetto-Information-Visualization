namespace TurnTheGameOn.SimpleTrafficSystem
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;

    public class Editor_SignalConnectorWindow : EditorWindow
    {
        public static Editor_SignalConnectorWindow editorWindow;

        Vector3 offset;
        Vector3 _position;
        float size;
        float pickSize;
        GUIStyle style;
        Transform sceneViewCameraTransform;
        Vector3 pointTransformPosition;
        Vector3 screenPoint;
        bool onScreen;

        [MenuItem("Tools/Simple Traffic System/Signal Connector Window")]
        public static void ShowWindow()
        {
            Editor_SignalConnectorWindow window = (Editor_SignalConnectorWindow)GetWindow(typeof(Editor_SignalConnectorWindow));
            window.minSize = new Vector2(300, 200);
            window.ClearData(true);
            window.titleContent.text = "Signal Connector";
            window.Show();
        }

        public AITrafficWaypointRoute[] routesToEdit;
        public AITrafficLight[] lightsToEdit;
        public AITrafficLight fromLight;
        public AITrafficWaypointRoute toRoute;
        public int fromLightIndex = -1;
        public int toRouteIndex = -1;

        bool showDebug = true;
        Vector2 scrollPos = new Vector2();

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MaxWidth(1000), GUILayout.MaxHeight(1000));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Lights & Routes"))
            {
                routesToEdit = FindObjectsOfType<AITrafficWaypointRoute>();
                lightsToEdit = FindObjectsOfType<AITrafficLight>();
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

                SerializedProperty lightsToEditProperty = serialObj.FindProperty("lightsToEdit");
                EditorGUILayout.PropertyField(lightsToEditProperty, true);

                SerializedProperty fromLightProperty = serialObj.FindProperty("fromLight");
                EditorGUILayout.PropertyField(fromLightProperty, true);

                SerializedProperty toRouteProperty = serialObj.FindProperty("toRoute");
                EditorGUILayout.PropertyField(toRouteProperty, true);

                SerializedProperty fromLightIndexProperty = serialObj.FindProperty("fromLightIndex");
                EditorGUILayout.PropertyField(fromLightIndexProperty, true);

                SerializedProperty toRouteIndexProperty = serialObj.FindProperty("toRouteIndex");
                EditorGUILayout.PropertyField(toRouteIndexProperty, true);

                GUI.enabled = true;
            }

            if (routesToEdit.Length > 0)
            {
                if (GUILayout.Button("Unload Lights & Routes"))
                {
                    ClearData(true);
                    EditorUtility.SetDirty(this);
                    Repaint();
                }

                GUI.enabled = false;

                SerializedProperty fromLightProperty = serialObj.FindProperty("fromLight");
                if (fromLightIndex == -1)
                    EditorGUILayout.HelpBox("Select 'F' handle to assign 'From Light Point'", MessageType.Info);
                EditorGUILayout.PropertyField(fromLightProperty, true);

                SerializedProperty toRouteProperty = serialObj.FindProperty("toRoute");
                if (toRouteIndex == -1)
                    EditorGUILayout.HelpBox("Select 'T' handle to assign 'To Route Point'", MessageType.Info);
                EditorGUILayout.PropertyField(toRouteProperty, true);

                GUI.enabled = true;

                if (GUILayout.Button("Connect Light to Route"))
                {
                    lightsToEdit[fromLightIndex].waypointRoute = routesToEdit[toRouteIndex];
                    EditorUtility.SetDirty(lightsToEdit[fromLightIndex].waypointRoute);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    ClearData(false);
                }
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        void ClearData(bool clearRoutes)
        {
            if (clearRoutes)
            {
                routesToEdit = new AITrafficWaypointRoute[0];
                lightsToEdit = new AITrafficLight[0];
            }
            toRouteIndex = -1;
            fromLightIndex = -1;
            toRouteIndex = -1;
            toRouteIndex = -1;
            fromLight = null;
            toRoute = null;
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

            #region To Routes Handle
            for (int i = 0; i < this.routesToEdit.Length; i++)
            {
                if (this.routesToEdit[i].waypointDataList.Count > 0)
                {
                    int index = this.routesToEdit[i].waypointDataList.Count - 1;
                    pointTransformPosition = this.routesToEdit[i].waypointDataList[index]._transform.position;
                    screenPoint = Camera.current.WorldToViewportPoint(pointTransformPosition);
                    onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

                    if (onScreen)
                    {
                        offset = new Vector3(0, 0.5f, 0);
                        _position = pointTransformPosition + offset * 2f;
                        size = 0.4f;
                        pickSize = size * 1;
                        style = new GUIStyle();
                        style.normal.textColor = (this.toRouteIndex == i) ? Color.yellow : Color.grey;
                        style.alignment = TextAnchor.MiddleCenter;
                        style.fontStyle = FontStyle.Bold;
                        style.fontSize = 12;
                        Handles.color = Color.grey;
                        if (Handles.Button(_position, Quaternion.LookRotation(sceneViewCameraTransform.forward, sceneViewCameraTransform.up), size, pickSize, Handles.RectangleHandleCap))
                        {
                            this.toRouteIndex = i;
                            this.toRoute = this.routesToEdit[i].waypointDataList[index]._waypoint.onReachWaypointSettings.parentRoute;
                            Repaint();
                        }
                        offset = new Vector3(0, 0.53f, 0);
                        _position = pointTransformPosition + offset * 2f;
                        Handles.Label
                        (
                            _position,
                            "T",
                            style
                        );
                    }
                }
            }
            #endregion

            #region FROM Handle
            for (int i = 0; i < this.lightsToEdit.Length; i++)
            {
                pointTransformPosition = this.lightsToEdit[i].transform.position;

                offset = new Vector3(0, 0.5f, 0);
                _position = pointTransformPosition + offset * 2f;
                size = 0.4f;
                pickSize = size * 1;
                style = new GUIStyle();
                style.normal.textColor = (this.fromLightIndex == i) ? Color.yellow : Color.grey;
                style.alignment = TextAnchor.MiddleCenter;
                style.fontStyle = FontStyle.Bold;
                style.fontSize = 12;
                Handles.color = Color.grey;


                if (Handles.Button(_position, Quaternion.LookRotation(sceneViewCameraTransform.forward, sceneViewCameraTransform.up), size, pickSize, Handles.RectangleHandleCap))
                {
                    this.fromLightIndex = i;
                    this.fromLight = this.lightsToEdit[i];
                    Repaint();
                }

                offset = new Vector3(0, 0.53f, 0);
                _position = pointTransformPosition + offset * 2f;
                Handles.Label
                (
                    _position,
                    "F",
                    style
                );
            }
            #endregion


            for (int i = 0; i < this.lightsToEdit.Length; i++)
            {
                if (this.lightsToEdit[i].waypointRoute != null)
                {
                    int index = this.lightsToEdit[i].waypointRoute.waypointDataList.Count - 1;
                    Handles.color = Color.red;
                    Handles.DrawLine(this.lightsToEdit[i].transform.position, this.lightsToEdit[i].waypointRoute.waypointDataList[index]._transform.position);
                }
            }

            SceneView.RepaintAll();
        }

    }
}