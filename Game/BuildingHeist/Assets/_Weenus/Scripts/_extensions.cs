using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine
{
	public static class _extensions
    {
        public static void SetToParentZero(this Component item, Transform parent)
        {
            item.transform.SetParent(parent);
            item.transform.localPosition = Vector3.zero;
        }
        public static string[] ToLines(this string s)
        {
            List<string> lines = new List<string>();
            string[] a = s.Split(new string[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < a.Length; i++)
            {
                string y = a[i].Trim();
                if (!string.IsNullOrEmpty(y))
                {
                    lines.Add(y);
                }
            }
            return lines.ToArray();
        }
        public static bool SameAs(this string source, string compareTo)
        {
            if (source == null)
                return false;

            return source.Equals(compareTo, StringComparison.OrdinalIgnoreCase);
        }
        public static bool HasTag(this Collider source, string tag)
        {
            if (source == null || source.gameObject == null || source.gameObject.tag == null)
                return false;

            return source.gameObject.tag.Equals(tag, StringComparison.OrdinalIgnoreCase);
        }
        public static bool HasTag(this Collision source, string tag)
        {
            if (source == null || source.gameObject == null || source.gameObject.tag == null)
                return false;

            return source.gameObject.tag.Equals(tag, StringComparison.OrdinalIgnoreCase);
        }
        public static bool HasSameTagAs(this Collision source, MonoBehaviour self)
        {
            if (source == null || source.gameObject == null || source.gameObject.tag == null)
                return false;

            return source.gameObject.tag.Equals(self.gameObject.tag, StringComparison.OrdinalIgnoreCase);
        }
        public static bool HasSameTagAs(this Collider source, MonoBehaviour self)
        {
            if (source == null || source.gameObject == null || source.gameObject.tag == null)
                return false;

            return source.gameObject.tag.Equals(self.gameObject.tag, StringComparison.OrdinalIgnoreCase);
        }

        //
	}
}
