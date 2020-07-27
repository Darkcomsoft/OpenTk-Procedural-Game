using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class GameObject : ObjElement
    {
        public readonly int _SceneID;
        public string _name;
        public List<object> ScriptElements = new List<object>();
        public Transform _transform;
        private MeshRender _MeshRender;
        private Camera _camera;
        private BoxCollider _boxCollider;
        private MeshCollider _meshCollider;
        private RigidBody _rigidBody;

        public GameObject(int scene) 
        {
            _transform = new Transform(new Vector3(0, 0, 0), Quaternion.Identity, new Vector3(1, 1, 1));
            _transform._gameObject = this;
            _SceneID = scene;

            StartObject();
        }
        public GameObject(string name, int scene) 
        {
            _transform = new Transform(new Vector3(0,0,0), Quaternion.Identity, new Vector3(1, 1, 1));
            _transform._gameObject = this;
            _name = name; _SceneID = scene;

            StartObject();
        }
        public GameObject(Vector3 position, Quaternion rotation, int scene) 
        { 
            _transform = new Transform(position, rotation, new Vector3(1, 1, 1));
            _transform._gameObject = this;
            _SceneID = scene;

            StartObject();
        }
        public GameObject(Transform parent)
        {
            _transform = new Transform(parent.Position, parent.Rotation, new Vector3(1, 1, 1));
            _transform._gameObject = this;
            parent.SetChild(_transform);
            _SceneID = parent._gameObject._SceneID;

            StartObject();
        }
        public GameObject(GameObject prefab, Vector3 position, Quaternion rotation, int scene) 
        { 
            _transform = new Transform(position, rotation, new Vector3(1, 1, 1));
            _transform._gameObject = this;
            _SceneID = scene;
            _name = prefab._name;

            StartObject();
        }

        private void StartObject()
        {
            Engine.Instance.UpdateFrame += Update;
            Engine.Instance.DrawUpdate += Draw;
        }

        public float rotation;
        public float Speed = 50;

        /// <summary>
        /// Script UpdateFreame
        /// </summary>
        public void Update(object obj,FrameEventArgs e)
        {
            if (_camera != null)
            {
                _camera.Update((float)e.Time);
            }

            if (_rigidBody != null)
            {
                _rigidBody.Update();
            }

            if (_boxCollider != null)
            {
                _boxCollider.Update();
            }

            if (_meshCollider != null)
            {
                _meshCollider.Update();
            }

            //_transform.Update();

            if (ScriptElements != null)
            {
                foreach (ScriptBase item in ScriptElements)
                {
                    item.Update();
                }
            }
        }

        public void Draw(FrameEventArgs e)
        {
            if (_MeshRender != null)
            {
                _MeshRender.Draw(e);
            }
        }

        public void OnDestroy()
        {
            Engine.Instance.UpdateFrame -= Update;
            Engine.Instance.DrawUpdate -= Draw;

            if (_MeshRender != null)
            {
                _MeshRender.OnDestroy();
                _MeshRender.gameObject = null;
            }

            if (_camera != null)
            {
                _camera.OnDestroy();
                _camera.gameObject = null;
            }

            if (_boxCollider != null)
            {
                _boxCollider.OnDestroy();
            }

            if (_meshCollider != null)
            {
                _meshCollider.OnDestroy();
            }

            if (_rigidBody != null)
            {
                _rigidBody.OnDestroy();
            }

            foreach (ScriptBase item in ScriptElements)
            {
                item.OnDestroy();
                item.gameObject = null;
            }

            ScriptElements.Clear();

            _transform._gameObject = null;

            _transform = null;
            _MeshRender = null;
            _camera = null;
            _meshCollider = null;
            _boxCollider = null;
            _rigidBody = null;

            ScriptElements = null;

            GC.Collect();
        }

        public static GameObject Instantiate(int scene)
        {
            GameObject obj = new GameObject("NewGameObject", scene);
            Engine.Instance.AddObject(obj);
            return obj;
        }
        public static GameObject Instantiate(string name, int scene)
        {
            GameObject obj = new GameObject(name, scene);
            Engine.Instance.AddObject(obj);
            return obj;
        }
        public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, int scene)
        {
            GameObject obj = new GameObject(prefab, position, rotation, scene);
            Engine.Instance.AddObject(obj);
            return obj;
        }
        public static GameObject Instantiate(Vector3 position, Quaternion rotation, int scene)
        {
            GameObject obj = new GameObject(position, rotation, scene);
            Engine.Instance.AddObject(obj);
            return obj;
        }
        public static GameObject Instantiate(Transform parent)
        {
            GameObject obj = new GameObject(parent);
            Engine.Instance.AddObject(obj);
            return obj;
        }

        public Camera AddCamera(Camera camera)
        {
            _camera = camera;
            return _camera;
        }
        public Camera AddCamera()
        {
            Camera cam = new Camera(this);
            _camera = cam;
            return _camera;
        }

        public MeshRender AddMeshRender(MeshRender meshrender)
        {
            _MeshRender = meshrender;
            return _MeshRender;
        }
        public MeshRender AddMeshRender()
        {
            MeshRender meshrender = new MeshRender(this, AssetsManager.LoadModel("Assets/Models/", "Cube"), new Shader(AssetsManager.instance.GetShader("Default")));
            meshrender._shader.AddTexture(new Texture(AssetsManager.instance.GetTexture("NewGrassTeste", "png")));
            _MeshRender = meshrender;
            return _MeshRender;
        }

        public BoxCollider AddBoxCollider(BoxCollider boxCollider)
        {
            _boxCollider = boxCollider;
            return _boxCollider;
        }
        public BoxCollider AddBoxCollider()
        {
            BoxCollider boxCollider = new BoxCollider(this);
            _boxCollider = boxCollider;
            return _boxCollider;
        }

        public MeshCollider AddMeshCollider(MeshCollider meshCollider)
        {
            _meshCollider = meshCollider;
            return _meshCollider;
        }
        public MeshCollider AddMeshCollider()
        {
            MeshCollider meshCollider = new MeshCollider(this);
            _meshCollider = meshCollider;
            return _meshCollider;
        }

        public RigidBody AddRigidBody(RigidBody rigidBody)
        {
            _rigidBody = rigidBody;
            return _rigidBody;
        }
        public RigidBody AddRigidBody()
        {
            RigidBody rigidBody = new RigidBody(this);
            _rigidBody = rigidBody;
            return _rigidBody;
        }

        public RigidBody GetRigidBody()
        {
            if (_rigidBody != null)
            {
                return _rigidBody;
            }
            return null;
        }
        public MeshRender GetMeshRender()
        {
            if (_MeshRender != null)
            {
                return _MeshRender;
            }
            return null;
        }

        public void AddScript(ScriptBase script)
        {
            script.gameObject = this;
            script.Start();
            ScriptElements.Add(script);
        }

        public static void Destroy(GameObject gameObject)
        {
            Engine.Instance.RemoveObject(gameObject);
        }

        public bool HaveRigid { get { return (_rigidBody != null); } }
    }

    public class ObjElement
    {

    }
}
