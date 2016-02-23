using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
public class StyleSheetConsumer : MonoBehaviour
{

    bool isEditorMode = false;
    public StyleNames StyleName;

    StyleSheet.StyleSheetDeclaration _style = null;
    StyleSheet styles;
    public bool IgnoreStyle { get; set; }
    public void IgnoreStyles(bool applyToChildren)
    {
        IgnoreStyle = true;

        if (applyToChildren)
        {
            var list = this.GetComponentsInChildren<StyleSheetConsumer>();
            foreach (var i in list)
                i.IgnoreStyles(false);
        }
    }
    public void ApplyStyles(StyleSheet styleSheet)
    {
        if (IgnoreStyle)
            return;


        _style = styleSheet.GetStyle(this.StyleName.ToString());
        if (_style != null)
        {
            ApplyStyle(_style);
        }
    }
    public void Start()
    {
        isEditorMode = Application.isEditor;
        styles = GameObject.FindObjectOfType<StyleSheet>();
        if (styles != null)
        {
            _style = styles.GetStyle(this.StyleName.ToString());
            if (_style != null)
            {
                ApplyStyle(_style);
            }
        }
    }

    private void ApplyStyle(StyleSheet.StyleSheetDeclaration specificStyle)
    {
        if (IgnoreStyle)
            return;

        SetColor(specificStyle);
    }

    private void SetColor(StyleSheet.StyleSheetDeclaration rule)
    {
        if (IgnoreStyle)
            return;

        Text t1 = this.GetComponent<Text>();
        if (t1 != null)
        {
            t1.color = rule.ValueColor;
        }
        Image i1 = this.GetComponent<Image>();
        if (i1 != null)
        {
            i1.color = rule.ValueColor;
        }

        Button b1 = this.GetComponent<Button>();
        if (b1 != null) // has actual pressed value
        {
            if (b1.targetGraphic != null)
            {
                ColorBlock buttonColors = new ColorBlock()
                {
                    normalColor = rule.ValueColor,
                    highlightedColor = rule.ValueColor,
                    pressedColor = rule.ValuePressedColor,
                    colorMultiplier = 1,
                    fadeDuration = 0.13f
                };
                
                if (i1 != null)
                {
                    i1.color = Color.white;
                }

                b1.colors = buttonColors;
            }
        }
    }

    public void Update()
    {
        if (isEditorMode && styles.LiveUpdate)
        {
            if (_style == null)
            {
                if (styles != null)
                {
                    _style = styles.GetStyle(this.StyleName.ToString());
                }
            }
            if (_style != null)
            {
                ApplyStyle(_style);
            }
        }
    }
}
