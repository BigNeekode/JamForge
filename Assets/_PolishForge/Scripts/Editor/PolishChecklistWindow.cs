using UnityEditor;
using UnityEngine;

namespace PolishForge.Editor
{
    public class PolishChecklistWindow : EditorWindow
    {
        private static readonly string[] Items =
        {
            "Every button has hover/click feedback.",
            "Player damage has visual feedback.",
            "Player damage has audio feedback.",
            "Enemy damage has visual feedback.",
            "Enemy death has VFX or animation.",
            "Pickups have sound and visual feedback.",
            "Camera shake is not too strong.",
            "Screen flash can be reduced or disabled.",
            "Hit stop does not break pause.",
            "Game over has clear feedback.",
            "Victory has clear feedback.",
            "Score changes are readable.",
            "Low health state is visible/audible.",
            "Audio is not clipping.",
            "There is a restart option.",
            "There is a main menu option.",
            "Build has been tested.",
            "WebGL build has been tested if used."
        };

        private Vector2 scroll;

        [MenuItem("Tools/PolishForge/Polish Checklist")]
        public static void Open()
        {
            GetWindow<PolishChecklistWindow>("Polish Checklist");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("PolishForge Checklist", EditorStyles.boldLabel);
            scroll = EditorGUILayout.BeginScrollView(scroll);

            foreach (string item in Items)
            {
                string key = $"PolishForge.Checklist.{item}";
                bool value = EditorPrefs.GetBool(key, false);
                bool next = EditorGUILayout.ToggleLeft(item, value);
                if (next != value)
                    EditorPrefs.SetBool(key, next);
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Clear Checklist"))
            {
                foreach (string item in Items)
                    EditorPrefs.DeleteKey($"PolishForge.Checklist.{item}");
            }
        }
    }
}
