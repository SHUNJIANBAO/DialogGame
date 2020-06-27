using UnityEditor;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerInspaector : Editor 
{
    public AudioManager manager {
        get {
            return target as AudioManager;
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        manager.MasterVolume = EditorGUILayout.Slider("总音量",manager.MasterVolume,0f,1f);
        manager.BgmVolume= EditorGUILayout.Slider("背景音乐音量", manager.BgmVolume, 0f, 1f);
        manager.AudioVolume = EditorGUILayout.Slider("音效音量", manager.AudioVolume, 0f, 1f);
    }
}
