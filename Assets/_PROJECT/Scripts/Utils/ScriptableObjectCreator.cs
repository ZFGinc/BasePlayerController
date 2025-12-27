using UnityEngine;

namespace ZFGinc.Utils
{
    public static class ScriptableObjectCreator
    {
#if UNITY_EDITOR
        public static T CreateScriptableObject<T>(string path) where T : ScriptableObject
        {
            var obj = ScriptableObject.CreateInstance<T>();

            UnityEditor.AssetDatabase.CreateAsset(obj, path);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            return obj;
        }
#endif
    }
}