using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
[InitializeOnLoad]
public class StartUp {
 
    
 
    static StartUp() {
 
        PlayerSettings.keystorePass = "11501150";
        PlayerSettings.keyaliasPass = "11501150";
    }
 
    
}
#endif