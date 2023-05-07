using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorUtils
{
    // Start is called before the first frame update
    public static string GetScriptingDefineSymbols()
    {
        string symbols;

#if UNITY_SWITCH
        symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Switch);
#elif UNITY_XBOXONE
        symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.XboxOne);
#elif UNITY_PS4
        symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.PS4);
#elif UNITY_IOS
        symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
#elif UNITY_ANDROID
        symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
#else
        symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
#endif

        return symbols;
    }

    // Update is called once per frame
    public static void SetScriptingDefineSymbols(string symbols)
    {
#if UNITY_SWITCH
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Switch, symbols);
#elif UNITY_XBOXONE
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.XboxOne, symbols);
#elif UNITY_PS4
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.PS4, symbols);
#elif UNITY_IOS
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, symbols);
#elif UNITY_ANDROID
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, symbols);
#else
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbols);
#endif

    }


    public static void AddScriptingDefineSymbol(string symbol)
    {
        string symbols = EditorUtils.GetScriptingDefineSymbols();

        if (!symbols.Contains(symbol))
        {
                symbols = symbols + ";" + symbol;
                EditorUtils.SetScriptingDefineSymbols(symbols);
                AssetDatabase.Refresh(ImportAssetOptions.Default);
        }
    }

    public static void RemoveScriptingDefineSymbol(string symbol)
    {
        string symbols = EditorUtils.GetScriptingDefineSymbols();

        if (symbols.Contains(symbol))
        {
                symbols = symbols.Replace(symbol,"");
                EditorUtils.SetScriptingDefineSymbols(symbols);
                AssetDatabase.Refresh(ImportAssetOptions.Default);
        }

    }

    public static bool IsScriptingDefineSymbolDefined(string symbol)
    {
        string symbols = EditorUtils.GetScriptingDefineSymbols();

        if (symbols.Contains(symbol))
        {
            return true;
        }
        else
            return false;
    }

}
