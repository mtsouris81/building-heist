using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

	public static class Urls
    {
        public const string Login = "/api/user/login";
        public const string Authors = "/api/author/all";
        public const string Stories = "/api/story/forplayer";
        public const string Story = "/api/story/{0}";


        public static class Builder
        {
            public const string Stories = "/api/story/forauthor";
            public const string StoriesByAuthor = "/api/author/{0}/stories";
            public const string Story = "/api/story/storydetail/{0}";
            public const string Part = "/api/story/part/{0}";
            public const string Characters = "/api/character/all";
            public const string Character = "/api/character/{0}";
            public const string CharacterList = "/api/character/list";
            public const string EmotionList = "/api/character/emotion/list";
            public const string CharacterSave = "/api/character/";
            public const string StoryPartAdd = "/api/story/{0}/part/{1}/{2}/add";
            public const string StoryPartSave = "/api/story/part/{0}/{1}";
            public const string StoryPartDelete = "/api/story/part/{0}/{1}/{2}/delete";
            public const string PartTypes = "/api/story/parttypes";

            public const string StorySettings = "/api/story/{0}/settings";
            public const string NewStory = "/api/story/new";


            public const string User = "/api/user/";
            public const string UserFollow = "/api/user/follow";
            public const string UserFollows = "/api/user/follows";
            public const string UserRegister = "/api/user/register";
            public const string UserChangePassword = "/api/user/changepassword";

            public const string DeletePart = "/api/story/part/{0}/remove";
            public const string MovePart =   "/api/story/part/{0}/move/{1}";
            public const string DeleteTalk = "/api/story/part/{0}/talk/{1}/delete";

        }

	}

    public static class ServiceUrl
    {

        public static string GetBaseUrl()
        {
            return IsLocal ? BaseUrlLocal : BaseUrlProd;
        }

        public static bool IsLocal { get; set; }
        public const string BaseUrlLocal = "http://story.weenus.com";// "http://localhost:2881"; ////http://story.weenus.com
        public const string BaseUrlProd = "http://storybuilder.weenussoft.com";

        public static string Get(string suffix, params object[] args)
        {
            string finalUrl = "";
            string baseUrl = IsLocal ? BaseUrlLocal : BaseUrlProd;
            if (args != null && args.Length > 0)
            {
                finalUrl = baseUrl + string.Format(suffix, args);
            }
            else
            {
                finalUrl = baseUrl + suffix;
            }
            return finalUrl;
        }
    }
