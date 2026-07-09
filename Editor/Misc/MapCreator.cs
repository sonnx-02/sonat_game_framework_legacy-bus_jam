using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorPrefs;
using Event = UnityEngine.Event;


public class MapCreator : EditorWindow
{
    [MenuItem("Tools/DuongPham/" + nameof(MapCreator))]
    public static void ShowWindow()
    {
        GetWindow(typeof(MapCreator));
    }

    [@MenuItem("GameObject/Rotate 90 Y _g")]
    public static void Rotate90Y()
    {
        var go = Selection.activeGameObject;
        if (go != null)
        {
            if(go.GetComponent<RectTransform>() == null)
                go.transform.eulerAngles = new Vector3(0,(Mathf.RoundToInt(go.transform.eulerAngles.y/90f) +1) *90,0);
        }
    }

    [@MenuItem("GameObject/Rotate 90 Y alternative ^g")]
    public static void Rotate90Y2()
    {
        var go = Selection.activeGameObject;
        if (go != null)
        {
            if(go.GetComponent<RectTransform>() == null)
                go.transform.eulerAngles = new Vector3(0,(Mathf.RoundToInt(go.transform.eulerAngles.y/90f) +1) *90,0);
        }
    }

    
    private Vector2 _scrollPosition;

    private string path => "Assets/maps.json";

    int _currentColor = 0;
    private EditorAssets _editorAssets;
    Rect _lastRect;

    int _currentLevel = 0;
    private MapListPointCollection _levelCollection;

    private Vector3Int _defaultSize;

    public Vector3Int DefaultSize
    {
        get
        {
            if (_defaultSize.x <= 0 || _defaultSize.y <= 0)
            {
                if (HasKey(nameof(DefaultSize)))
                    _defaultSize = JsonUtility.FromJson<Vector3Int>(GetString(nameof(DefaultSize)));
                else
                    _defaultSize = new Vector3Int(10, 10, 10);
            }

            return _defaultSize;
        }
        set
        {
            _defaultSize = value;
            SetString(nameof(DefaultSize), JsonUtility.ToJson(_defaultSize));
        }
    }

    void Load()
    {
        _currentLevel = Mathf.Clamp(_currentLevel, 0, _levelCollection.maps.Count);
        if (_levelCollection.maps.Count == _currentLevel)
        {
            _tempLevel.map.customParameter.intValue = _defaultSize.x;
            _tempLevel.map.customParameter.intValue2 = _defaultSize.y;
            _tempLevel.map.customParameter.intValue3 = _defaultSize.z;
            Repaint();
            return;
        }

        _tempLevel = _levelCollection.maps[_currentLevel].Clone();
        Repaint();
    }

    private MapListPoint _tempLevel;

    public void OnGUI()
    {
        if (ReferenceEquals(_editorAssets, null))
            _editorAssets = Resources.Load<EditorAssets>("EditorAssets");

        Event current = Event.current;
        var color = GUI.backgroundColor;
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);


        StartHorizon(() =>
        {
            _currentLevel =
                EditorGUILayout.IntField(
                    $"level (total:{(_levelCollection != null ? _levelCollection.maps.Count : 0)})", _currentLevel);
            if (GUILayout.Button("<<"))
            {
                _currentLevel--;
                Load();
            }

            if (GUILayout.Button(">>"))
            {
                _currentLevel++;
                Load();
            }

            if (GUILayout.Button("load from path"))
            {
                var data = DuongExtensions.LoadTextAtPath(path);
                _levelCollection = JsonUtility.FromJson<MapListPointCollection>(data);
                if (_levelCollection == null)
                    _levelCollection = new MapListPointCollection();
                Load();
                Debug.Log(_levelCollection);
            }

            if (GUILayout.Button("save file"))
            {
                File.WriteAllText(path, JsonUtility.ToJson(_levelCollection));
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.LogError("save to path:" + path);
            }

            if (GUILayout.Button("save level"))
            {
                if (_levelCollection.maps.Count == _currentLevel)
                    _levelCollection.maps.Add(_tempLevel.Clone());
                else
                    _levelCollection.maps[_currentLevel] = _tempLevel.Clone();
            }
        });

        StartHorizon(() =>
        {
            if (GUILayout.Button("Clear"))
            {
                _tempLevel = new MapListPoint();
                _tempLevel.map.customParameter.intValue = _defaultSize.x;
                _tempLevel.map.customParameter.intValue2 = _defaultSize.y;
                _tempLevel.map.customParameter.intValue3 = _defaultSize.z;
            }
        });

        var df = DefaultSize;
        df = EditorGUILayout.Vector3IntField("default", df, GUILayout.Width(300));
        if (DefaultSize != df)
            DefaultSize = df;


        EditorGUILayout.LabelField("Select :", _level2Title, _baseLayout);
        _lastRect = GUILayoutUtility.GetLastRect();
        int count1 = 0;
        for (int i = 0; i < _editorAssets.colors.Count; i++)
        {
            Vector2 rootPos = new Vector2(50 + count1 * (_editorAssets.tileSize.x + _editorAssets.spacing.x),
                _lastRect.yMax + 10);
            Rect rect = new Rect(rootPos, _editorAssets.tileSize);

            if (i < _editorAssets.numberTextures.Length)
            {
                DrawTexture(rect, _editorAssets.numberTextures[i], _editorAssets.colors[i]);
            }
            else
            {
                DrawBgTile(rect, _editorAssets.colors[i]);
            }

            //Draw Indicator
            if (i == _currentColor)
            {
                rootPos.y += _editorAssets.tileSize.y + 2;

                var size = new Vector2(_editorAssets.tileSize.x, _editorAssets.tileSize.y / 4);
                rect = new Rect(rootPos, size);
                DrawBgTile(rect, Color.black);
            }

            //Catch Click
            if (current.type == EventType.MouseDown && rect.Contains(current.mousePosition))
            {
                _currentColor = i;
                this.Repaint();
            }

            count1++;
        }

        GUILayout.Space(_editorAssets.tileSize.y + _editorAssets.spacing.y + 20);
        GUI.color = color;

        EditorGUILayout.LabelField(path);
        if (_tempLevel != null)
        {
            _tempLevel.map.customParameter.vector3 = EditorGUILayout.Vector3IntField("Size",
                _tempLevel.map.customParameter.vector3, GUILayout.Width(300));

            _lastRect = GUILayoutUtility.GetLastRect();
            var col = _tempLevel.map.customParameter.vector3.x;
            var row = _tempLevel.map.customParameter.vector3.y;
            for (int i = 0; i < col; i++)
            for (int j = 0; j < row; j++)
            {
                var point = new Point(i, j);
                Vector2 rootPos = new Vector2(50 + point.col * (_editorAssets.tileSize.x + _editorAssets.spacing.x),
                    _lastRect.yMax + 10 + (row - 1 - point.row) *
                    (_editorAssets.tileSize.y + _editorAssets.spacing.y));
                Rect rect = new Rect(rootPos, _editorAssets.tileSize);
                DrawBgTile(rect, _editorAssets.blankColor);
                // Mouse down
                if (current.type == EventType.MouseDown && rect.Contains(current.mousePosition))
                {
                    if (current.button == 0)
                    {
                        var find = _tempLevel.map.points.Find(x => x == point);
                        if (find == null)
                            _tempLevel.map.points.Add(new PointParameter(point.col, point.row, _currentColor, 0));
                        else
                            find.value = _currentColor;
                    }

                    if (current.button == 1)
                        _tempLevel.map.points.RemoveAll(x => x == point);
                    Repaint();
                }
            }


            foreach (var point in _tempLevel.map.points)
            {
                Vector2 rootPos = new Vector2(50 + point.col * (_editorAssets.tileSize.x + _editorAssets.spacing.x),
                    _lastRect.yMax + 10 + (row - 1 - point.row) *
                    (_editorAssets.tileSize.y + _editorAssets.spacing.y));
                Rect rect = new Rect(rootPos, _editorAssets.tileSize);

                DrawTexture(rect, _editorAssets.numberTextures[point.value], _editorAssets.colors[point.value]);
            }

            GUILayout.Space(row * (_editorAssets.tileSize.y + _editorAssets.spacing.y) + 20);
        }


        GUI.color = color;
        EditorGUILayout.LabelField("spawn");

        if (_tempLevel != null)
        {
            if (_tempLevel.spawns.Count < 2)
            {
                _tempLevel.spawns = new List<PointParameter>()
                {
                    new PointParameter(0, -1, 0, 0), new PointParameter(0, -1, 0, 0)
                };
            }

            for (int k = 0; k < _tempLevel.spawns.Count; k++)
            {
                _lastRect = GUILayoutUtility.GetLastRect();
                for (int i = 0; i < 2; i++)
                {
                    var point = new Point(i, 0);

                    Vector2 rootPos = new Vector2(50 + point.col * (_editorAssets.tileSize.x + _editorAssets.spacing.x),
                        _lastRect.yMax + 10 + (point.row) *
                        (_editorAssets.tileSize.y + _editorAssets.spacing.y));
                    Rect rect = new Rect(rootPos, _editorAssets.tileSize);

                    DrawBgTile(rect, _editorAssets.blankColor);

                    // Mouse down
                    if (current.type == EventType.MouseDown && rect.Contains(current.mousePosition))
                    {
                        if (current.button == 0)
                        {
                            if (i == 0)
                            {
                                _tempLevel.spawns[k] =
                                    new PointParameter(_currentColor, _tempLevel.spawns[k].row, 0, 0);
                            }
                            else
                            {
                                _tempLevel.spawns[k] =
                                    new PointParameter(_tempLevel.spawns[k].col, _currentColor, 0, 0);
                            }
                        }

                        if (current.button == 1)
                        {
                            if (i == 0)
                            {
                                _tempLevel.spawns[k] = new PointParameter(-1, _tempLevel.spawns[k].row, 0, 0);
                            }
                            else
                            {
                                _tempLevel.spawns[k] = new PointParameter(_tempLevel.spawns[k].row, -1, 0, 0);
                            }
                        }

                        Repaint();
                    }
                }


                for (var i = 0; i < 2; i++)
                {
                    var point = new Point(i, 0);
                    Vector2 rootPos = new Vector2(50 + point.col * (_editorAssets.tileSize.x + _editorAssets.spacing.x),
                        _lastRect.yMax + 10 + (point.row) *
                        (_editorAssets.tileSize.y + _editorAssets.spacing.y));
                    Rect rect = new Rect(rootPos, _editorAssets.tileSize);

                    var c = i == 0 ? _tempLevel.spawns[k].col : _tempLevel.spawns[k].row;

                    if (c >= 0)
                        DrawTexture(rect, _editorAssets.numberTextures[c], _editorAssets.colors[c]);
                    else
                        DrawBgTile(rect, _editorAssets.blankColor);
                }

                GUILayout.Space(1 * (_editorAssets.tileSize.y + _editorAssets.spacing.y));
            }
        }

        //// __________

        GUI.color = color;
        GUILayout.Space(15);
        if (GUILayout.Button("Add"))
        {
            _tempLevel.spawns.Add(new PointParameter(0, -1, 0, 0));
        }

        StartHorizon(() =>
        {
            if (GUILayout.Button("Start Level"))
            {
                RootView.rootView.gameController.StartLevelDirty(_tempLevel);
            }

            if (GUILayout.Button("Delete Data"))
            {
                PlayerPrefs.DeleteAll();
                ReactivePlayerData.DeleteFile();
            }
        });

        GUILayout.Space(15);
        EditorGUILayout.EndScrollView();
    }

    private Texture2D Bgtexture2D => StaticTexture.SquareTexture;

    void DrawBgTile(Rect rect, Color bgColor)
    {
        GUI.color = bgColor;
        GUI.DrawTexture(rect, Bgtexture2D, ScaleMode.StretchToFill);
    }

    void DrawTexture(Rect rect, Texture2D text, Color bgColor)
    {
        GUI.color = bgColor;
        GUI.DrawTexture(rect, text, ScaleMode.StretchToFill);
    }

    void StartHorizon(Action action)
    {
        //*
        EditorGUILayout.BeginHorizontal();
        action.Invoke();
        EditorGUILayout.EndHorizontal();
        //*
    }

    GUIStyle _level1 => new GUIStyle()
    {
        padding = new RectOffset(10, 0, 0, 0),
        fontStyle = FontStyle.Bold,
    };

    GUIStyle _level2Title => new GUIStyle()
    {
        padding = new RectOffset(20, 0, 5, 0),
        fontStyle = FontStyle.Bold
    };

    GUIStyle _level2 => new GUIStyle()
    {
        padding = new RectOffset(30, 0, 5, 0),
        //fontStyle = FontStyle.Bold
    };

    GUILayoutOption[] _baseLayout => new GUILayoutOption[] {GUILayout.Width(100f)};


    private static string FilePath => string.Format("{0}/{1}", Application.persistentDataPath, "PlayerData.json");
}