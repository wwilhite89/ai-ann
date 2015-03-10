using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A console to display Unity's debug logs in-game.
/// </summary>
public class ConsoleDebug : MonoBehaviour
{
    struct Log
    {
        public string message;
        public string stackTrace;
        public LogType type;
    }

    List<Log> logs = new List<Log>();
    Vector2 scrollPosition;

    // Visual elements:

    static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
	{
		{ LogType.Assert, Color.white },
		{ LogType.Error, Color.red },
		{ LogType.Exception, Color.red },
		{ LogType.Log, Color.white },
		{ LogType.Warning, Color.yellow },
	};

    const int margin = 20;

    Rect windowRect = new Rect(margin, margin, Screen.width/4, Screen.height - (margin * 2));
    Rect titleBarRect = new Rect(0, 0, 10000, 20);
    GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    GUIContent details = new GUIContent("Position", "Position and heading");
    BasePlayerScript player;

    void Start() {
        this.player = this.gameObject.GetComponent<BasePlayerScript>();
    }

    void Update()
    {
        float heading;

        if (player != null)
        {
            Vector3 pos = player.GetPosition(out heading);

            details.text = "Position: " + pos + "\nHeading: " + heading + " degrees";
        }
        else
        {
            details.text = "not working";
        }   
    }

    void OnEnable()
    {
        Application.RegisterLogCallback(HandleLog);
    }

    void OnDisable()
    {
        Application.RegisterLogCallback(null);
    }

    void OnGUI()
    {
        
        windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
    }

    /// <summary>
    /// A window that displayss the recorded logs.
    /// </summary>
    /// <param name="windowID">Window ID.</param>
    void ConsoleWindow(int windowID)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Iterate through the recorded logs.
        for (int i = 0; i < logs.Count; i++)
        {
            var log = logs[i];

            GUI.contentColor = logTypeColors[log.type];
            GUILayout.Label(log.message);
        }

        GUILayout.EndScrollView();

        GUI.contentColor = Color.white;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button(clearLabel))
        {
            logs.Clear();
        }

        GUILayout.TextArea(details.text, GUILayout.ExpandWidth(false));

        GUILayout.EndHorizontal();

        // Allow the window to be dragged by its title bar.
        GUI.DragWindow(titleBarRect);
    }

    /// <summary>
    /// Records a log from the log callback.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="stackTrace">Trace of where the message came from.</param>
    /// <param name="type">Type of message (error, exception, warning, assert).</param>
    void HandleLog(string message, string stackTrace, LogType type)
    {
        logs.Add(new Log()
        {
            message = message,
            stackTrace = stackTrace,
            type = type,
        });
    }
}