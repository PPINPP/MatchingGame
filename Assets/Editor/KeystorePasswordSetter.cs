using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class KeystorePasswordSetter : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    private const string KeystorePass = "11501150";
    private const string KeyAliasPass = "11501150";

    public void OnPreprocessBuild(BuildReport report)
    {
        PlayerSettings.keystorePass = KeystorePass;
        PlayerSettings.keyaliasPass = KeyAliasPass;
    }
}
