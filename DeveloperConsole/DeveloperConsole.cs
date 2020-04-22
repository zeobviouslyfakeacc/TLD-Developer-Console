using UnityEngine;
using Scene = UnityEngine.SceneManagement;
using MelonLoader;

namespace DeveloperConsole {

    internal class DeveloperConsole : MelonMod {

        private bool init = false;
        private static bool flyEnabled = false;

        public override void OnUpdate() {
            if (!init) {
                MelonModLogger.Log("Update");
                Object.Instantiate(Resources.Load("uConsole"));
                uConsole.m_Instance.m_Activate = KeyCode.F1;
                AddConsoleCommands();
                init = true;
                MelonModLogger.Log("Done!");
            }
        }

        private static void AddConsoleCommands() {
            uConsole.RegisterCommand("load_scene", new System.Action(LoadScene));

            uConsole.RegisterCommand("fly", new System.Action(Fly));

            uConsole.RegisterCommand("save", new System.Action(() => GameManager.m_PendingSave = true));

            uConsole.RegisterCommand("scene_name", new System.Action(() => Debug.Log((Il2CppSystem.String) Scene.SceneManager.GetActiveScene().name)));

            uConsole.RegisterCommand("scene_index", new System.Action(() => Debug.Log((Il2CppSystem.String) Scene.SceneManager.GetActiveScene().handle.ToString())));

            uConsole.RegisterCommand("pos", new System.Action(GetPosition));

            uConsole.RegisterCommand("tp", new System.Action(Teleport));
        }

        private static void LoadScene() {
            var ind = uConsole.GetInt();
            SceneManager.LoadScene(ind);
        }

        private static void Fly() {
            bool fly = !flyEnabled;
            if (uConsole.GetNumParameters() > 0 && uConsole.NextParameterIsBool())
                fly = uConsole.GetBool();
            if (fly == flyEnabled) {
                return;
            }
            if (fly) {
                FlyMode.Enter();
            } else {
                FlyMode.TeleportPlayerAndExit();
            }
        }

        private static void GetPosition() {
            Vector3 pos = GameManager.GetVpFPSPlayer().transform.position;
            Debug.Log((Il2CppSystem.String) string.Format("[{0:F2} / {1:F2} / {2:F2}]", pos.x, pos.y, pos.z));
        }

        private static void Teleport() {
            Vector3 target;

            if (uConsole.GetNumParameters() < 2) {
                Debug.Log((Il2CppSystem.String) "Usage: tp x z    or    tp x y z.\nExample: tp 123 890");
                return;
            } else if (uConsole.GetNumParameters() == 2) {
                float x = uConsole.GetFloat();
                float z = uConsole.GetFloat();

                Vector3 start = new Vector3(x, 10000f, z);
                if (Physics.Raycast(start, Vector3.down, out RaycastHit raycastHit, float.PositiveInfinity, Utils.m_PhysicalCollisionLayerMask | 1048576 | 134217728)) {
                    target = raycastHit.point + new Vector3(0, 0.01f, 0);
                } else {
                    target = new Vector3(x, 0, z);
                }
            } else {
                float x = uConsole.GetFloat();
                float y = uConsole.GetFloat();
                float z = uConsole.GetFloat();
                target = new Vector3(x, y, z);
            }

            Quaternion rot = GameManager.GetVpFPSCamera().transform.rotation;
            GameManager.GetPlayerManagerComponent().TeleportPlayer(target, rot);
            GameManager.GetPlayerManagerComponent().StickPlayerToGround();
        }
    }
}
