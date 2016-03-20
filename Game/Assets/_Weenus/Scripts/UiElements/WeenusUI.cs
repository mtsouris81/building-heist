using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GiveUp.Core;
using UnityEngine.Events;

namespace Weenus
{
	public class WeenusUI
    {

        public UIScreenManager UI
        {
            get
            {
                return MobileUIManager.Current.Manager;
            }
        }

        public Dictionary<string, WeenusFieldButton> Button = new Dictionary<string, WeenusFieldButton>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, WeenusFieldView> FieldView = new Dictionary<string, WeenusFieldView>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, WeenusInputField> FieldInput = new Dictionary<string, WeenusInputField>(StringComparer.OrdinalIgnoreCase);

        public WeenusUI(Component unityComponent)
        {
            var buttons = new List<WeenusFieldButton>();
            var views = new List<WeenusFieldView>();
            var inputs = new List<WeenusInputField>();

            RectTransform rect = unityComponent.GetComponent<RectTransform>();

            GameObjectUtilities.GetAllComponentsInChildTree<WeenusFieldButton>(rect, buttons);
            GameObjectUtilities.GetAllComponentsInChildTree<WeenusFieldView>(rect, views);
            GameObjectUtilities.GetAllComponentsInChildTree<WeenusInputField>(rect, inputs);

            foreach (var b in buttons)
                Button.Add(b.gameObject.name, b);

            foreach (var b in views)
                FieldView.Add(b.gameObject.name, b);

            foreach (var b in inputs)
                FieldInput.Add(b.gameObject.name, b);
        }

        int maxIterations = 1000;
        int currentIteration = 0;










        public void SetViewText(string name, string text)
        {
            try
            {
                FieldView[name].SetValue(text);
            }
            catch
            {
                throw new Exception(string.Format("failed binding to field : {0}", name));
            }
        }
        public void SetInputText(string name, string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    text = string.Empty;
                }
                FieldInput[name].SetValue(text);
            }
            catch
            {
                throw new Exception(string.Format("failed binding to field : {0}", name));
            }
        }
        public void SetOnChangedHandler(string name, UnityAction<string> callback)
        {
            FieldInput[name].OnChanged(callback);
        }
        public void SetClickHandler(string name, Action callback)
        {
            SetClickHandler(name, false, callback);
        }
        public void SetClickHandler(string name, bool isOptional, Action callback)
        {
            try
            {
                Button[name].OnButtonClick = callback;
            }
            catch
            {
                if (isOptional)
                {
                    return;
                }
                throw new Exception(string.Format("failed binding to button : {0}", name));
            }
        }

        public string GetInputText(string name)
        {
            return GetInputText(name, false);
        }
        public string GetInputText(string name, bool allowMissing)
        {
            try
            {
                return FieldInput[name].GetValue();
            }
            catch
            {
                if (!allowMissing)
                {
                    throw new Exception(string.Format("failed get value of field : {0}", name));
                }
                return string.Empty;
            }
        }
        public int GetInputInt(string name)
        {
            try
            {
                return FieldInput[name].GetValueInt();
            }
            catch
            {
                throw new Exception(string.Format("failed get value of field : {0}", name));
            }
        }
	}
}
