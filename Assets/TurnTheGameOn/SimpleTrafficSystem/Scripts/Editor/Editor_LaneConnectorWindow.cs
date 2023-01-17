namespace TurnTheGameOn.SimpleTrafficSystem
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;

    public class Editor_LaneConnectorWindow : EditorWindow
    {
        public static Editor_LaneConnectorWindow editorWindow;

        Vector3 offset;
        Vector3 _position;
        float size;
        float pickSize;
        GUIStyle style;
        Transform sceneViewCameraTransform;
        Vector3 pointTransformPosition;
        Vector3 screenPoint;
        bool onScreen;

        [MenuItem("Tools/Simple Traffic System/Lane Connector Window")]
        public static void ShowWindow()
        {
            Editor_LaneConnectorWindow window = (Editor_LaneConnectorWindow)GetWindow(typeof(Editor_LaneConnectorWindow));
            window.minSize = new Vector2(300, 200);
            window.ClearData(true);
            window.titleContent.text = "Lane Connector";
            window.Show();
        }

        public AITrafficWaypointRoute[] routesToEdit;
        public AITrafficWaypointRoute routeA;
        public AITrafficWaypointRoute routeB;
        public int routeIndexA = -1;
        public int routeIndexB = -1;

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

                SerializedProperty routeAProperty = serialObj.FindProperty("routeA");
                EditorGUILayout.PropertyField(routeAProperty, true);

                SerializedProperty routeBProperty = serialObj.FindProperty("routeB");
                EditorGUILayout.PropertyField(routeBProperty, true);

                SerializedProperty routeIndexAProperty = serialObj.FindProperty("routeIndexA");
                EditorGUILayout.PropertyField(routeIndexAProperty, true);

                SerializedProperty routeIndexBProperty = serialObj.FindProperty("routeIndexB");
                EditorGUILayout.PropertyField(routeIndexBProperty, true);

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

                SerializedProperty routeAProperty = serialObj.FindProperty("routeA");
                if (routeIndexA == -1)
                    EditorGUILayout.HelpBox("Select 'A' handle to assign 'Route A'", MessageType.Info);
                EditorGUILayout.PropertyField(routeAProperty, true);

                SerializedProperty routeBProperty = serialObj.FindProperty("routeB");
                if (routeIndexB == -1)
                    EditorGUILayout.HelpBox("Select 'B' handle to assign 'Route B'", MessageType.Info);
                EditorGUILayout.PropertyField(routeBProperty, true);

                GUI.enabled = true;

                if (routeIndexA != -1 && routeIndexB != -1)
                {
                    if (GUILayout.Button("Setup Lane Change Points"))
                    {
                        LaneChangeHelper.AssignLaneChangePoints(routeA, routeB);
                        EditorUtility.SetDirty(routeA);
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        LaneChangeHelper.AssignLaneChangePoints(routeB, routeA);
                        EditorUtility.SetDirty(routeB);
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
            if (clearRoutes)
            {
                routesToEdit = new AITrafficWaypointRoute[0];
            }
            routeIndexA = -1;
            routeIndexB = -1;
            routeA = null;
            routeB = null;
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
                if (this.routesToEdit[i].waypointDataList.Count > 0)
                {
                    int index = this.routesToEdit[i].waypointDataList.Count - 1;
                    pointTransformPosition = this.routesToEdit[i].waypointDataList[index]._transform.position;
                    screenPoint = Camera.current.WorldToViewportPoint(pointTransformPosition);
                    onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

                    if (onScreen)
                    {
                        // if index is selected
                        // if 1 or more indexes are not assigned
                        if ((routeIndexA == i || routeIndexB == i) || (routeIndexA == -1 || routeIndexB == -1))
                        {
                            offset = new Vector3(0, 0.5f, 0);
                            _position = pointTransformPosition + offset * 2f;
                            size = 0.4f;
                            pickSize = size * 1;
                            style = new GUIStyle();
                            style.normal.textColor = (this.routeIndexA == i || this.routeIndexB == i) ? Color.cyan : Color.white;
                            style.alignment = TextAnchor.MiddleCenter;
                            style.fontStyle = FontStyle.Bold;
                            style.fontSize = 12;
                            Handles.color = (this.routeIndexA == i || this.routeIndexB == i) ? Color.cyan : Color.white;
                            if (Handles.Button(_position, Quaternion.LookRotation(sceneViewCameraTransform.forward, sceneViewCameraTransform.up), size, pickSize, Handles.RectangleHandleCap))
                            {
                                bool updatedSelection = false;
                                /// Check if selection is already assigned to A
                                if (this.routeIndexA == i)
                                {
                                    this.routeA = null;
                                    routeIndexA = -1;
                                    updatedSelection = true;
                                }
                                /// Check if selection is already assigned to B
                                if (this.routeIndexB == i)
                                {
                                    this.routeB = null;
                                    routeIndexB = -1;
                                    updatedSelection = true;
                                }
                                /// Assign selection
                                if (updatedSelection == false)
                                {
                                    if (this.routeIndexA == -1)
                                    {
                                        this.routeIndexA = i;
                                        this.routeA = this.routesToEdit[i].waypointDataList[index]._waypoint.onReachWaypointSettings.parentRoute;
                                    }
                                    else if (this.routeIndexB == -1)
                                    {
                                        this.routeIndexB = i;
                                        this.routeB = this.routesToEdit[i].waypointDataList[index]._waypoint.onReachWaypointSettings.parentRoute;
                                    }
                                }
                                Repaint();
                            }
                            offset = new Vector3(0, 0.53f, 0);
                            _position = pointTransformPosition + offset * 2f;
                            Handles.Label
                            (
                                _position,
                                "R",
                                style
                            );
                        }
                        
                    }
                }
            }
            /// Draw Line
            if (this.routeIndexA != -1 && this.routeIndexB != -1)
            {
                Handles.color = Color.cyan;
                int indexA = this.routesToEdit[routeIndexA].waypointDataList.Count - 1;
                int indexB = this.routesToEdit[routeIndexB].waypointDataList.Count - 1;
                Vector3 positionA = this.routesToEdit[routeIndexA].waypointDataList[indexA]._transform.position + offset * 2f;
                Vector3 positionB = this.routesToEdit[routeIndexB].waypointDataList[indexB]._transform.position + offset * 2f;
                Handles.DrawLine(positionA, positionB);
            }
            SceneView.RepaintAll();
        }

    }
}