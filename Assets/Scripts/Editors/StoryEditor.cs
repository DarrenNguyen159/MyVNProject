using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.IO;

namespace DN.VN
{
    public class StoryEditor : EditorWindow
    {
        private List<StoryNode> nodes;// danh sách các StoryNode
        private List<Connection> connections;
        private VisualElement rootNode;

        private GUIStyle nodeStyle;

        private GUIStyle inPointStyle;
        private GUIStyle outPointStyle;

        private ConnectionPoint selectedInPoint;
        private ConnectionPoint selectedOutPoint;

        private Vector2 drag;

        private Vector2 offset = new Vector2(0, 0);
        public static Vector2 initialPosition = new Vector2(500, 300);

        private string openFilePath = "";
        private string openFileName = "";

        // init position
        Rect controlPanelRect;

        [MenuItem("Window/Story Editor")]
        private static void OpenWindow()
        {
            StoryEditor window = GetWindow<StoryEditor>();
            window.titleContent = new GUIContent("Story Editor");
        }

        void Init()
        {
            if (nodes == null)
            {
                nodes = new List<StoryNode>();
            }
            StartNode startNode = new StartNode(initialPosition, 100, 100, OnClickOutPoint);
            nodes.Add(startNode);

            EndNode endNode = new EndNode(new Vector2(initialPosition.x + 200, initialPosition.y), 100, 100, OnClickOutPoint);
            nodes.Add(endNode);

            controlPanelRect = new Rect(new Vector2(0, 0), new Vector2(300, 500));
        }

        private void OnGUI()
        {
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

            offset = new Vector2(100, 0);
            DrawNodes();
            DrawConnections();

            DrawConnectionLine(Event.current);

            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            DrawControlPanel();


            if (GUI.changed) Repaint();
        }

        private void DrawControlPanel()
        {
            if (openFileName.Length == 0)
            {
                openFileName = "untitled";
            }
            GUIStyle panelStyle = new GUIStyle("ProgressBarBack");
            GUI.Box(controlPanelRect, "", panelStyle);
            GUILayout.Space(20);
            GUILayout.BeginScrollView(new Vector2(0, 0), panelStyle);
            openFileName = GUILayout.TextField(openFileName);

            if (GUILayout.Button("OPEN"))
            {
                string path = EditorUtility.OpenFilePanel("Open file", openFilePath, "json");
                if (path.Length > 0)
                {
                    openFileName = Path.GetFileName(path);
                    openFilePath = path.Substring(0, path.Length - openFileName.Length);

                    GUI.changed = true;

                    string jsonString = File.ReadAllText(path);

                    JSONParserTemplate json = new JSONParserTemplate(jsonString);
                    json.Parse();

                    string storyName = json.GetString("storyName");
                    openFileName = storyName;
                }
            }
            if (GUILayout.Button("EXPORT"))
            {
                Export();
            }
            GUI.EndScrollView();
        }

        private void Export()
        {
            string fileName = openFileName;
            string path = EditorUtility.SaveFilePanel("Save file", "", openFileName, "json");
            string saveFileName = Path.GetFileName(path);
            if (path.Length > 0)
            {
                Story story = new Story();
                story.storyName = fileName;
                story.author = "DN";
                JSONTemplate json = new JSONTemplate();
                json.AddKeyValue("storyName", story.storyName);
                json.AddKeyValue("author", story.author);
                JSONTemplate nodeJSON = new JSONTemplate();
                int currentId = 0;
                foreach (StoryNode node in nodes)
                {
                    nodeJSON.AddChildJSON("" + currentId, node.GetJSONObject());
                    currentId += 1;
                }
                json.AddChildJSON("nodes", nodeJSON);
                File.WriteAllText(path, json.GetJSON());
            }
        }

        private void DrawConnectionLine(Event e)
        {
            if (selectedInPoint != null && selectedOutPoint == null)
            {
                Handles.DrawBezier(
                    selectedInPoint.rect.center,
                    e.mousePosition,
                    selectedInPoint.rect.center + Vector2.left * 50f,
                    e.mousePosition - Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );

                GUI.changed = true;
            }

            if (selectedOutPoint != null && selectedInPoint == null)
            {
                Handles.DrawBezier(
                    selectedOutPoint.rect.center,
                    e.mousePosition,
                    selectedOutPoint.rect.center - Vector2.left * 50f,
                    e.mousePosition + Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );

                GUI.changed = true;
            }
        }

        private void DrawConnections()
        {
            if (connections != null)
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    connections[i].Draw();
                }
            }
        }

        private void OnDrag(Vector2 delta)
        {
            drag = delta;
            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].Drag(drag);
                }
            }
            GUI.changed = true;
        }

        private void DrawNodes()
        {
            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].Draw();
                }
            }
        }

        private void OnEnable()
        {
            Init();

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
            inPointStyle = new GUIStyle();
            inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            inPointStyle.border = new RectOffset(4, 4, 12, 12);

            outPointStyle = new GUIStyle();
            outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
            outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
            outPointStyle.border = new RectOffset(4, 4, 12, 12);
        }

        private void ProcessEvents(Event e)
        {
            drag = Vector2.zero;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1)  // nhấp chuột phải
                    {
                        ProcessContextMenu(e.mousePosition);
                    }
                    break;
#if UNITY_EDITOR_OSX
                case EventType.ScrollWheel:
                    OnDrag(e.delta * -5);
                    break;
#else
                case EventType.MouseDrag:
                    if (e.button == )
                    {
                        OnDrag(e.delta);
                    }
                    break;
#endif
            }
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            // Add các options của Menu
            genericMenu.AddItem(new GUIContent("Add Story Node"), false, () => OnClickAddNode(mousePosition));

            genericMenu.ShowAsContext();
        }

        private void OnClickAddNode(Vector2 mousePosition)
        {
            if (nodes == null)
            {
                nodes = new List<StoryNode>();
            }
            StoryNode node = new StoryNode(mousePosition, 200, 50, OnClickInPoint, OnClickOutPoint);
            nodes.Add(node);
        }

        private void ProcessNodeEvents(Event e)
        { 
            if (nodes != null)
            {
                for (int i = nodes.Count - 1; i >= 0; i--)
                {
                    bool guiChanged = nodes[i].ProcessEvents(e);

                    if (guiChanged)
                    {
                        GUI.changed = true;
                    }
                }
            }
        }

        private void OnClickInPoint(ConnectionPoint inPoint)
        {
            selectedInPoint = inPoint;

            if (selectedOutPoint != null)
            {
                if (selectedOutPoint.node != selectedInPoint.node)
                {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else
                {
                    ClearConnectionSelection();
                }
            }
        }

        private void OnClickOutPoint(ConnectionPoint outPoint)
        {
            selectedOutPoint = outPoint;

            if (selectedInPoint != null)
            {
                if (selectedOutPoint.node != selectedInPoint.node)
                {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else
                {
                    ClearConnectionSelection();
                }
            }
        }

        private void OnClickRemoveConnection(Connection connection)
        {
            connections.Remove(connection);
        }

        private void CreateConnection()
        {
            if (connections == null)
            {
                connections = new List<Connection>();
            }

            connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
        }

        private void ClearConnectionSelection()
        {
            selectedInPoint = null;
            selectedOutPoint = null;
        }

        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            offset += drag * 0.5f;
            Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }
    }
}
