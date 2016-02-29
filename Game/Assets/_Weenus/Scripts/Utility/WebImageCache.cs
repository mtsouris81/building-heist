using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WebImageCache : MonoBehaviour {


    public static WebImageCache GetCurrent()
    {
        return GameObject.FindObjectOfType<WebImageCache>();
    }

    public void Update()
    {
        foreach (var d in ActiveDownloads)
        {
            d.AttemptResolve();
            if (d.Resolved)
            {
                DownloadsToClear.Add(d);
            }
        }

        foreach (var d in DownloadsToClear)
        {
            ActiveDownloads.Remove(d);
            ImageDownloads.Remove(d.URL);
        }
    }

    public class ImageDownload
    {
        public ImageDownload(WebImageCache c, string url)
        {
            this.cache = c;
            this.URL = url;
            www = new WWW(url);
        }

        public Texture2D Texture { get; set; }
        public string URL { get; private set; }
        
        
        WWW www;
        bool hasResolved = false;
        WebImageCache cache;

        public void AttemptResolve()
        {
            if (hasResolved)
            {
                return;
            }
            if (www == null || !www.isDone)
            {
                return;
            }
            this.cache.Images.Add(this.URL, www.texture);
            hasResolved = true;
        }

        public bool Resolved
        {
            get
            {
                return hasResolved;
            }
        }
    }

    

    public Texture2D GetTexture(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return null;
        }
        if (Images.ContainsKey(url))
        {
            return Images[url];
        }

        if (ImageDownloads.ContainsKey(url))
        {
            // waiting for download
            return null;
        }
        
        var download = new ImageDownload(this, url);
        ImageDownloads.Add(url, download);
        ActiveDownloads.Add(download);
        return null;
    }

    List<ImageDownload> ActiveDownloads = new List<ImageDownload>();
    Dictionary<string, ImageDownload> ImageDownloads = new Dictionary<string, ImageDownload>(StringComparer.OrdinalIgnoreCase);
    Dictionary<string, Texture2D> Images = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);
    List<ImageDownload> DownloadsToClear = new List<ImageDownload>();


}
