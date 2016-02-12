using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GiveUp.Core
{
    public class PlayerUtility
    {

        public const string ObjectName = "Hero";
        public const string TagName = "Player";

        static GameObject _playerInstance;
        static Hero _hero;

        public static GameObject GetPlayer()
        {
            return GameObject.Find(ObjectName);
        }

        public static GameObject Current
        {
            get
            {
                if (_playerInstance == null)
                    _playerInstance = GetPlayer();

                return _playerInstance;
            }
            set
            {
                _playerInstance = value;
                _hero = null;
            }
        }
        public static Hero Hero
        {
            get
            {
                if (Current == null)
                    return null;

                if (_hero == null)
                    _hero = Current.GetComponent<Hero>();

                return _hero;
            }
        }


        public static void ClearCurrent()
        {
            _playerInstance = null;
            _hero = null;
        }
    }	
    
    
    public class NpcUtility
    {
        public const string TagName = "NPC";
	}
}
