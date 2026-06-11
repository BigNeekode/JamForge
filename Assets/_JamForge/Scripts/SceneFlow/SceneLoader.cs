using UnityEngine;
using UnityEngine.SceneManagement;

namespace JamForge
{
    public enum JamScene
    {
        Boot,
        MainMenu,
        Game,
        GameOver
    }

    public static class SceneLoader
    {
        public static void Load(JamScene scene)
        {
            SceneManager.LoadScene(GetSceneName(scene));
        }

        public static void Load(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public static void ReloadCurrentScene()
        {
            Scene current = SceneManager.GetActiveScene();
            SceneManager.LoadScene(current.name);
        }

        public static void QuitGame()
        {
#if UNITY_EDITOR
            Debug.Log("Quit requested. Ignored inside the Unity Editor.");
#else
            Application.Quit();
#endif
        }

        public static string GetSceneName(JamScene scene)
        {
            return scene switch
            {
                JamScene.Boot => "00_Boot",
                JamScene.MainMenu => "01_MainMenu",
                JamScene.Game => "02_Game",
                JamScene.GameOver => "03_GameOver",
                _ => "01_MainMenu"
            };
        }
    }
}
