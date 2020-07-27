using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class Scene
    {
        public readonly int _SceneID;
        public readonly string _SceneName;

        public Scene(int id, string name) {
            _SceneID = id;
            _SceneName = name;
        }

        /// <summary>
        /// Script UpdateFreame
        /// </summary>
        private void Update(FrameEventArgs e)
        {

        }
        
        /// <summary>
        /// Graphics Draw Update Frame
        /// </summary>
        private void Draw(FrameEventArgs e)
        {

        }

        /// <summary>
        /// When a Scene is unload/destroyed, clean all the data in thins scene
        /// </summary>
        public void OnUnloadScene()
        {
            int c = 0;

            for (int i = 0; i < Engine.Instance.GameObjects.Count; i++)
            {
                if (Engine.Instance.GameObjects[i]._SceneID == _SceneID)
                {
                    Engine.Instance.GameObjects[i].OnDestroy();
                    c++;
                }
            }

            Engine.Instance.GameObjects.RemoveAll(e => e._SceneID == _SceneID);

            Debug.Log("Destroyed : " + c + " GameObject's");

            Debug.Log("Unload Scene: " + _SceneName);

            long last = GC.GetTotalMemory(false);
            GC.Collect();
            Debug.Log("GC: Collected, Cleared: " + last + " (BYTES)");
        }
    }

    public static class SceneManager
    {
        private static Dictionary<int,Scene> _Scenes = new Dictionary<int, Scene>();
        private static int _LastSceneLoaded;

        public static Dictionary<int, Scene> GetSceneArray { get { return _Scenes; } }

        public static void LoadScene(string SceneName)
        {
            if (!_Scenes.ContainsKey(500))
            {
                UnloadLast();
                _Scenes.Add(500, new Scene(500,SceneName));
                _LastSceneLoaded = 500;
            }
            else
            {
                Debug.LogError("This Scene AllReady Loaded!");
            }
        }
        public static void UnloadScene(string SceneName)
        {
            if (_Scenes.TryGetValue(500, out Scene scene))
            {
                scene.OnUnloadScene();
                _Scenes.Remove(500);
                _LastSceneLoaded = -1;
            }
            else
            {
                Debug.LogError("This Scene isnt Loaded!");
            }
        }
        private static void UnloadLast()
        {
            if (_Scenes.TryGetValue(_LastSceneLoaded, out Scene scene) && _LastSceneLoaded != -1)
            {
                scene.OnUnloadScene();
                _Scenes.Remove(_LastSceneLoaded);
                _LastSceneLoaded = -1;
            }
            else
            {
                Debug.LogError("This Scene isnt Loaded!");
            }
        }
        /// <summary>
        /// Default Scene is a default/empty scene, for engine do stuff, and when no have scene loaded!
        /// </summary>
        public static void LoadDefaultScene()
        {
            _Scenes.Add(1, new Scene(1, "EngineDefaultScene"));
            _LastSceneLoaded = 1;
        }
        /// <summary>
        /// DontDestroy scene is All objects instantiated in this with a Dontdestroyonload, when load and unlaod a scene they dont be destroyed!
        /// </summary>
        public static void LoadDontDestroyScene()
        {
            _Scenes.Add(0, new Scene(0, "DontDestroyOnLoad"));
        }

        public static Scene LoadFileScene()
        {

            return null;
        }

        public static bool SaveFileScene(Scene scene)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(string.Concat("EditorData", scene._SceneName, ".evs"));

            bf.Serialize(file, scene);
            file.Close();
            return false;
        }
    }
}
