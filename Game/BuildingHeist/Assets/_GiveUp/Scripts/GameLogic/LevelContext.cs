using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace GiveUp.Core
{
    public class LevelContext : MonoBehaviour
    {

        // designer values
        public string Created = string.Empty;
        public string NextLevelName;
        public string BackgroundMusic;
        public float MinimumPlayerY = -50;
        public Color DeathFadeColor = Color.red;
        public float DeathFadeOutTime = 3;
        public float DeathFadeRestoreTime = 1;
        // code properties
        public Queue<Vector3> PlayerMinutePositions { get; set; }
        public Vector3 PlayerInitialPosition { get; set; }
        public LevelResourceList<PlayerRespawnTrigger> PlayerRespawnPoints { get; private set; }
        public LevelResourceList<NpcCore> NPCs { get; private set; }
        public LevelResourceList<SpawnManager> NpcSpawnManagers { get; private set; }
        public LevelResourceList<HealthPowerUp> HealthPowerUps { get; private set; }
        public LevelResourceList<AmmoPowerUp> AmmoPowerUps { get; private set; }
        public LevelResource<CinematicManager> Cinematics { get; private set; }
        public CameraManager Cameras { get; private set; }
        public PlayerRespawnTrigger CurrentRespawn { get; private set; }
        public ScreenFader HUD { get; private set; }
        public MusicManager Music { get; private set; }
        public TutorialManager TutorialManager { get; private set; }
        public Color HealthRestoreColor { get; private set; }
        public bool SupportsPause { get; private set; }
        public DialogDisplay Dialog { get; set; }

        public Hero Hero { get; private set; }

        public PointAndClickPage PauseController;
        ActionTimer PlayerPositionTimer;

        public void StorePlayerPosition()
        {
            if (PlayerMinutePositions == null)
            {
                PlayerMinutePositions = new Queue<Vector3>();
            }

            while (PlayerMinutePositions.Count >= 8)
            {
                PlayerMinutePositions.Dequeue();
            }

            PlayerMinutePositions.Enqueue(PlayerUtility.Current.transform.position);
        }

        public Vector3 RestorePlayerPosition(int minutesAgo)
        {
            Vector3 result = this.PlayerInitialPosition;

            if (PlayerMinutePositions == null || PlayerMinutePositions.Count < 1)
            {
                return result;
            }

            for (int i = 1; i <= minutesAgo; i++)
            {
                if (PlayerMinutePositions.Count == 0)
                {
                    break;
                }

                result = PlayerMinutePositions.Dequeue();
            }

            return result;

        }


        public void AttemptToSetPlayerRespawn(PlayerRespawnTrigger t)
        {
            CurrentRespawn = t;
        }

        public void RespawnPlayer()
        {
            if (CurrentRespawn != null)
            {
                CurrentRespawn.TeleportPlayer();
                return;
            }

            // default action ... shouldn't happen
            Debug.Log("couldnt find respawn position");
            PlayerUtility.Current.transform.position = PlayerInitialPosition;
        }

        public void PlayCinematic(string cinematicName)
        {
            //if (Cinematics.IsReady)
            //{
            //    SetPauseForAllNPCs(true);
            //    Cinematics.Instance.Play(cinematicName);
            //}
        }

        public void GoToNextLevel()
        {
            GoToLevel(NextLevelName);
        }

        public void GoToLevel(string level)
        {
            PlayerUtility.ClearCurrent();
            if (level == "End")
            {
                Application.LoadLevel("Credits");
                return;
            }

            Application.LoadLevel(level);
        }

        public void CinematicFinished_Intro()
        {
            if (Music != null)
            {
                //Music.PlayBackgroundMusic(MusicManager.LevelMusicType.Level);
            }
        }
        public void CinematicFinished_Boss()
        {
            if (Music != null)
            {
                //Music.PlayBackgroundMusic(MusicManager.LevelMusicType.Boss);
            }
        }
        public void CinematicFinished_End()
        {
            GoToNextLevel();
        }

        void Start()
        {
            _current = this;

            Created = DateTime.Now.ToString();
            PlayerPositionTimer = new ActionTimer(60, StorePlayerPosition);
            PlayerPositionTimer.Loop = true;
            PlayerPositionTimer.Start();
            var mus = GameObject.Find("Music");
            if (mus != null)
            {
                Music = mus.GetComponent<MusicManager>();
            }
            this.TutorialManager = this.GetComponentInChildren<TutorialManager>();
            HUD = GameObject.FindObjectOfType<ScreenFader>();

            HealthPowerUps = new LevelResourceList<HealthPowerUp>(null);
            AmmoPowerUps = new LevelResourceList<AmmoPowerUp>(null);

            PlayerRespawnPoints = new LevelResourceList<PlayerRespawnTrigger>(null);
            NPCs = new LevelResourceList<NpcCore>(null);
            NpcSpawnManagers = new LevelResourceList<SpawnManager>(null);
            Cinematics = new LevelResource<Core.CinematicManager>(null);


            Dialog = this.GetComponentInChildren<DialogDisplay>();

            // every "end" cinematic should go to the next level
            Cinematics.Instance.CinematicEndActions.Add("End", LevelContext.Current.CinematicFinished_End);
            Cinematics.Instance.CinematicEndActions.Add("Intro", LevelContext.Current.CinematicFinished_Intro);
            Cinematics.Instance.CinematicEndActions.Add("Boss", LevelContext.Current.CinematicFinished_Boss);
            Cinematics.Instance.CinematicEndActions.Add("*", delegate() { SetPauseForAllNPCs(false); });

            Cameras = new CameraManager();

            this.SupportsPause = (PauseController != null);

            PlayerInitialPosition = PlayerUtility.Current.transform.position;

            Hero = PlayerUtility.Current.GetComponent<Hero>();

            PlayerUtility.Hero.Died += new System.EventHandler(Hero_Died);
            PlayerUtility.Hero.HealthRestored += new EventHandler(Hero_HealthRestored);
            // clean out NPC prefabs that are used for spawn locations....
            RemoveSpawnPrefabsFromNpcList();

            HealthRestoreColor = Color.Lerp(Color.green, Color.clear, 0.6f);


            //WeenusSoft.MouseLook.MouseControlActive = true;

        }

        void Hero_HealthRestored(object sender, EventArgs e)
        {
        }

        private void RemoveSpawnPrefabsFromNpcList()
        {
            List<NpcCore> removeList = new List<NpcCore>();

            foreach (var npc in this.NPCs.List)
            {
                if (npc.transform.parent != null)
                {
                    SpawnManager mgr = npc.transform.parent.gameObject.GetComponent<SpawnManager>();
                    if (mgr != null)
                    {
                        removeList.Add(npc);
                    }
                }
            }

            foreach (var n in removeList)
            {
                this.NPCs.List.Remove(n);
            }
        }


        void Hero_Died(object sender, System.EventArgs e)
        {
            if (HUD == null)
            {
                DestroyAllNPCs();
                ResetCinematics();
                RespawnPlayer();
                PlayerUtility.Hero.Reset();
                ResetSpawnManagers();
            }
            else
            {

                HUD.ShowExtraItem();

                HUD.OnFadeHoldStart = delegate()  // callback when starting to fade back in
                    {
                        DestroyAllNPCs();
                        ResetCinematics();
                        RespawnPlayer();
                        ResetSpawnManagers();
                    };

                HUD.OnFadeHoldEnd = delegate()
                    {
                        HUD.HideExtraItem();
                        PlayerUtility.Hero.Reset();
                    };

                HUD.FadeOutThenHoldThenFadeIn(
                    DeathFadeColor, // out color
                    Color.black, // fade back in color
                    DeathFadeOutTime, // time per fade
                    DeathFadeRestoreTime,
                    1.2f);
            }
        }

        public void ResetCinematics()
        {
            if (this.Cinematics.IsReady)
            {
                foreach (var s in this.Cinematics.Instance.Cinematics)
                {
                    s.Reset();
                }
            }
        }

        private void ResetSpawnManagers()
        {
            foreach (var s in this.NpcSpawnManagers.List)
            {
                s.Reset();
            }
        }

        public bool IsPaused { get; private set; }

        public void SetTimeFreeze(bool active)
        {
            Time.timeScale = active ? 0 : 1;
        }


        public void TogglePaused()
        {
            SetPaused(!IsPaused);
        }

        public void SetPaused(bool paused)
        {
            IsPaused = paused;

            SetTimeFreeze(IsPaused);

            //WeenusSoft.MouseLook.MouseControlActive = !IsPaused;

            if (PauseController != null)
            {
                if (IsPaused)
                {
                    PauseController.Activate();
                }
                else
                {
                    PauseController.CloseScreen();
                }
            }
        }


        public void Pause()
        {
            if (IsPaused)
                return;

            IsPaused = true;
            SetTimeFreeze(IsPaused);
        }
        public void UnPause()
        {
            IsPaused = false;
            SetTimeFreeze(IsPaused);
        }


        bool pauseToggle = true;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                GoToNextLevel();
                return;
            }
            PlayerPositionTimer.Update();
            if (PlayerUtility.Hero.IsAlive && (MinimumPlayerY > PlayerUtility.Current.transform.position.y))
            {
                PlayerUtility.Hero.KillPlayer();
            }

           // CheckHighIntensityMusicConditions();
            bool shouldHudBeActive = false;
            if (HUD != null)
            {
                // during fades and cinematics, HUD should be disabled
                shouldHudBeActive = !Cinematics.Instance.IsCinematicPlaying && !IsPaused;// !HUD.FadeTimer.Enabled;
                HUD.gameObject.SetActive(shouldHudBeActive);

            }
            if (TutorialManager != null)
            {
                TutorialManager.gameObject.SetActive(shouldHudBeActive);
            }

            if (PauseKeyPushed())
            {
                this.TogglePaused();
            }

        }



        private bool PauseKeyPushed()
        {
            return (SupportsPause && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)));
        }

        static LevelContext _current;
        public static LevelContext Current
        {
            get
            {
                if (_current == null)
                {
                    var obj = GameObject.Find("LevelContext");
                    if (obj != null)
                    {
                        _current = obj.GetComponent<LevelContext>();
                    }
                }
                return _current;
            }
        }

        public static void ClearCurrent()
        {
            _current = null;
        }


        public int GetLiveNpcCount()
        {
            int result = 0;
            foreach (NpcCore npc in this.NPCs.List)
            {
                if (npc == null || npc.HealthManager == null)
                    continue;

                if (npc.HealthManager.IsAlive())
                {
                    result++;
                }
            }
            return result;
        }

        public void SetPauseForAllNPCs(bool isPaused)
        {
            foreach (NpcCore npc in this.NPCs.List)
            {
                if (isPaused)
                {
                    npc.LastEnabledState = npc.gameObject.activeInHierarchy;
                    npc.gameObject.SetActive(false);
                }
                else
                {
                    npc.gameObject.SetActive(npc.LastEnabledState);
                }
            }
        }

        public void DestroyAllNPCs()
        {
            NpcCore[] listCopy = GameObject.FindObjectsOfType<NpcCore>();
            foreach (NpcCore npc in listCopy)
            {
                if (npc.HealthManager != null)
                {
                    npc.HealthManager.Hurt(npc.MaxHealth + 10);
                    npc.HealthManager.CheckForDeath();
                }
            }
        }
    }
}