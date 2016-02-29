using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

namespace Weenus.Scripts.Utility
{

    public class WebImageHelper
    {
        public WebImageHelper(string url)
        {
            this.Url = url;
            ImageCache.GetTexture(url);
        }

        public bool HasSet { get; private set; }
        public string Url { get; set; }
        public bool IsReady { get { return (ImageCache.GetTexture(this.Url) != null); } }
        WebImageCache _cache;
        WebImageCache ImageCache
        {
            get
            {
                if (_cache == null)
                {
                    _cache = WebImageCache.GetCurrent();
                }
                return _cache;
            }
        }

        public void SetSprite(Image image)
        {
            if (image == null)
                return;

            var tex = ImageCache.GetTexture(this.Url);

            if (tex != null)
            {
                image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                HasSet = true;
            }
        }

        public void SetRenderTexture(MeshRenderer[] meshRenderer)
        {
            if (meshRenderer == null)
                return;

            var tex = ImageCache.GetTexture(this.Url);

            if (tex != null)
            {
                foreach (var r in meshRenderer)
                {
                    r.sharedMaterial.mainTexture = tex;
                }

                //Debug.Log(string.Format("set texture on {0} renderers", meshRenderer.Length));
                HasSet = true;
            }
        }
    }

}
