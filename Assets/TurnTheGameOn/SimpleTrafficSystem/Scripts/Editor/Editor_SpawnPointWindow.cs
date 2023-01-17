namespace TurnTheGameOn.SimpleTrafficSystem
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;

    public class Editor_SpawnPointWindow : EditorWindow
    {
        public static Editor_SpawnPointWindow editorWindow;

        Vector3 offset;
        Vector3 _position;
        float size;
        float pickSize;
        GUIStyle style;
        Transform sceneViewCameraTransform;
        Vector3 pointTransformPosition;
        Vector3 screenPoint;
        bool onScreen;

        [MenuItem("Tools/Simple Traffic System/Spawn Points Window")]
        public static void ShowWindow()
        {
            Editor_SpawnPointWindow window = (Editor_SpawnPointWindow)GetWindow(typeof(Editor_SpawnPointWindow));
            window.minSize = new Vector2(300, 200);
            window.ClearData(true);
            window.titleContent.text = "Spawn Points";
            window.Show();
        }

        public AITrafficWaypointRoute[] routesToEdit;

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

                if (GUILayout.Button("Align Route Waypoints"))
                {
                    for (int i = 0; i < routesToEdit.Length; i++)
                    {
                        routesToEdit[i].AlignPoints();
                    }
                    EditorUtility.SetDirty(this);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    Repaint();
                }
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        void ClearData(bool clearRoutes)
        {
            if (clearRoutes) routesToEdit = new AITrafficWaypointRoute[0];
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
                        offset = new Vector3(0, 1f, 0);
                        _position = pointTransformPosition + offset * 2f;
                        size = 0.4f;
                        pickSize = size * 1;
                        style = new GUIStyle();
                        style.normal.textColor = Color.white;
                        style.alignment = TextAnchor.MiddleCenter;
                        style.fontStyle = FontStyle.Bold;
                        style.fontSize = 12;
                        Handles.color = Color.grey;

                        if (Handles.Button(_position, Quaternion.LookRotation(sceneViewCameraTransform.forward, sceneViewCameraTransform.up), size, pickSize, Handles.RectangleHandleCap))
                        {
                            GameObject loadedSpawnPoint = Instantiate(Resources.Load("AITrafficSpawnPoint"), this.routesToEdit[i].waypointDataList[j]._transform) as GameObject;

                            AITrafficSpawnPoint trafficSpawnPoint = loadedSpawnPoint.GetComponent<AITrafficSpawnPoint>();

                            trafficSpawnPoint.waypoint = trafficSpawnPoint.transform.parent.GetComponent<AITrafficWaypoint>();

                            GameObject[] newSelection = new GameObject[1];
                            newSelection[0] = loadedSpawnPoint;

                            Selection.objects = newSelection;

                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                            Repaint();
                        }

                        offset = new Vector3(0, 1.03f, 0);
                        _position = pointTransformPosition + offset * 2f;
                        Handles.Label
                        (
                            _position,
                            "S",
                            style
                        );
                    }
                }
            }

            SceneView.RepaintAll();
        }

    }
}