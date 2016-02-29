using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Write
{
    public static void Log(string message)
    {
        Debug.Log(message);
    }
    public static void Format(string format, params object[] args)
    {
        Debug.Log(string.Format(format, args));
    }
    public static void Collection<T>(string format, IEnumerable<T> list, Func<T, string> value)
    {
        if (list.Count() < 1)
        {
            Debug.Log(string.Format(format, ""));
            return;
        }

        Debug.Log(
            string.Format(format, 
                string.Join(", ", list.Select(x => value(x)).ToArray())
                ));
    }
}