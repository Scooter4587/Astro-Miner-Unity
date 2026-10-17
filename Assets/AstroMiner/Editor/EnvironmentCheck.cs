using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AstroMiner.Editor
{
    // Editor-only setup check. This class does not add gameplay or run in builds.
    public static class EnvironmentCheck
    {
        [MenuItem("Astro Miner/Setup/Check Environment")]
        public static void CheckEnvironment()
        {
            var scene = SceneManager.GetActiveScene();
            Debug.Log($"Astro Miner setup | Unity {Application.unityVersion} | Scene: {scene.path} | Editor code compiled.");
        }
    }
}
