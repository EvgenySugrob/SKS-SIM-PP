using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MapFileGeneration : MonoBehaviour
{
    [InitializeOnLoadMethod]
    static void SetAdditionalIl2CppArgs()
    {
       string pathFile = Path.Combine(Application.dataPath, "Mapfile.map");
       PlayerSettings.SetAdditionalIl2CppArgs("--linker-flags=\"/MAP:\"" + pathFile + "\"\"");
       //PlayerSettings.SetAdditionalIl2CppArgs("");
       //PlayerSettings.SetAdditionalIl2CppArgs("--linker-flags=\"/MAP:\"D:\\mapfile.map\"\""); //Backup
    }
}
