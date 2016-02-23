using System;
using System.Linq;
using UnityEngine;

public class StyleSheet : MonoBehaviour
{


    public bool LiveUpdate = true;
    public StyleSheetDeclaration[] Declarations = null;

    public StyleSheetDeclaration GetStyle(string name)
    {
        if (name == null)
            return null;

        return Declarations.Where(x => x.Name == name).FirstOrDefault();
    }

    [Serializable]
    public class StyleSheetDeclaration
    {
        public string Name;
        public Color ValueColor = Color.clear;
        public Color ValuePressedColor = Color.clear;
    }

    [Serializable]
    public class StyleSheetDeclarationRule
    {
        public StyleSheetRule Type = StyleSheetRule.Color;
        public Color ValueColor = Color.white;
        public Color ValuePressedColor = Color.white;
    }
    
    public enum StyleSheetRule
    {
        Color
    }
}
