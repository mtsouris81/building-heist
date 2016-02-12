using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace GiveUp.Core
{
    public class SpawnManager : MonoBehaviour
    {

        public enum SpawnMode
        {
            RespawnAll,
            BurstThenRespawn,
            BurstThenFinishSet,
            FinishSet
        }

        public delegate void SpawnedNpcDelegate(SpawnManager manager);
        GameObject Player;
        ActionTimer timer;


        public SpawnMode Mode = SpawnMode.RespawnAll;
        public int BurstAmount = 0;
        public int MaxLiveNpcs = 0;
        public int TotalSpawnCount = 4;
        public float SpawnInterval = 3;
        public bool IsBoss = false;
        public bool KillAllOtherEnemies = false;
        public bool SpawnerActive = true;
        public string TargetTagName = "Player";
        public string MusicName = null;
        public Transform BlockerObject = null;


        bool hasKilledOtherEnemies = false;
        SphereCollider sphereTester;
        bool hasBurst;
        bool spawnedAllOnce = false;
        int _spawnCounter = 0;
        int currentNpc = 0;
        NpcCore[] NpcPrefabs;
        float burstTimerSpeed = 0.004f;
        bool shouldUpdateTimer = false;

        public SpawnedNpcDelegate OnSpawnedNpcCallback { get; set; }
        public GameObject LastSpawnedNpc { get; private set; }



        public void Reset()
        {
            currentNpc = 0;
            LastSpawnedNpc = null;
            _spawnCounter = 0;
            hasBurst = false;
            hasKilledOtherEnemies = false;
            spawnedAllOnce = false;

            if (this.BlockerObject != null)
            {
                BlockerObject.gameObject.SetActive(false);
            }
        }



        void Start()
        {
            if (string.IsNullOrEmpty(this.MusicName))
            {
                this.MusicName = this.MusicName.Trim();
            }

            Player = GameObject.Find("Hero");
            sphereTester = GetComponent<SphereCollider>();
            timer = new ActionTimer(SpawnInterval, SpawnNPC);
            timer.Loop = true;
            timer.Start();
            NpcPrefabs = GetComponentsInChildren<NpcCore>(true);
            if (NpcPrefabs != null && NpcPrefabs.Length > 0)
            {
                foreach (var p in NpcPrefabs)
                {
                    p.gameObject.SetActive(false);
                }
            }
            if (this.Mode == SpawnMode.BurstThenFinishSet || this.Mode == SpawnMode.FinishSet)
            {
                TotalSpawnCount = this.NpcPrefabs.Length;
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (LevelContext.Current.Cinematics.Instance.IsCinematicPlaying)
            {
                return;
            }

            if (_spawnCounter >= TotalSpawnCount)
                return;

            if (other.gameObject.tag == "Player")
            {
                shouldUpdateTimer = true;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.HasTag("Player"))
            {
                return;
            }

            if (LevelContext.Current.Music != null)
            {
                if (this.IsBoss)
                {
                    LevelContext.Current.Music.PlayBackgroundMusic(MusicManager.LevelMusicType.Boss);
                }
                else if (!string.IsNullOrEmpty(this.MusicName))
                {
                    LevelContext.Current.Music.PlayBackgroundMusic(this.MusicName);
                }
            }

            if (BlockerObject != null)
            {
                BlockerObject.gameObject.SetActive(true);
            }

            if (this.Mode == SpawnMode.RespawnAll || this.Mode == SpawnMode.FinishSet || this.hasBurst)
                return;

            this.timer.TimeLimit = burstTimerSpeed;
            this.hasBurst = true;


        }

        void Update()
        {
            if (LevelContext.Current.Cinematics.Instance.IsCinematicPlaying)
            {
                return;
            }

            if (shouldUpdateTimer && SpawnerActive)
            {

                if (KillAllOtherEnemies && !hasKilledOtherEnemies)
                {
                    hasKilledOtherEnemies = true;

                    List<NpcCore> _npcs = new List<NpcCore>();
                    var npcs = GameObject.FindObjectsOfType<NpcCore>();

                    if (npcs != null && npcs.Length > 0)
                    {
                        _npcs.AddRange(npcs);
                    }

                    for (int i = 0; i < _npcs.Count; i++)
                    {
                        if (_npcs[i] != null && _npcs[i].HealthManager != null && _npcs[i].HealthManager.IsAlive() && _npcs[i] != LastSpawnedNpc)
                        {
                            _npcs[i].HealthManager.Hurt(_npcs[i].MaxHealth);
                            _npcs.Remove(_npcs[i]);
                            i--;
                        }
                    }
                }

                timer.Update();
                shouldUpdateTimer = false;

            }
        }





        public void SpawnNPC()
        {
            if (_spawnCounter >= TotalSpawnCount)
                return;

            if (!SpawnerActive || (NpcPrefabs.Length < 1))
            {
                return;
            }

            int totalLiveNpcs = 0;
            foreach (var n in LevelContext.Current.NPCs.List)
            {
                if (n == null || n.HealthManager == null)
                    continue;

                if (n.HealthManager.IsAlive())
                {
                    totalLiveNpcs++;
                }
            }

            if (totalLiveNpcs >= MaxLiveNpcs)
            {
                return; // too many live NPC's right now
            }

            if (currentNpc >= NpcPrefabs.Length)
            {
                spawnedAllOnce = true;
                currentNpc = 0;
            }

            if (spawnedAllOnce && (NpcPrefabs[currentNpc].Type == EnemyType.StandingThrower || NpcPrefabs[currentNpc].Type == EnemyType.FloatingShooter))
            {
                currentNpc++;
                return;
            }

            // scale can be affected by a scaled spawn manager, so make sure npc is spawned at native scale
            Vector3 npcScale = determineNpcScale(NpcPrefabs[currentNpc]);

            var npc = Instantiate(NpcPrefabs[currentNpc], NpcPrefabs[currentNpc].gameObject.transform.position, NpcPrefabs[currentNpc].gameObject.transform.rotation) as NpcCore;

            if (npc != null)
            {
                LastSpawnedNpc = npc.gameObject;
                npc.transform.localScale = npcScale;
                npc.gameObject.SetActive(true);
                //npc.RegisterSelf();
            }
            else
            {
                Debug.Log("failed spawn");
            }


            if (OnSpawnedNpcCallback != null)
                OnSpawnedNpcCallback(this);



            currentNpc++;
            _spawnCounter++;




            if (this.Mode == SpawnMode.BurstThenFinishSet || this.Mode == SpawnMode.BurstThenRespawn)
            {
                if (_spawnCounter >= this.BurstAmount)
                {
                    // set back to normal spawn interval
                    this.timer.TimeLimit = this.SpawnInterval;
                }
            }
        }
        private Vector3 determineNpcScale(NpcCore npcCore)
        {
            return Vector3.Scale(this.transform.localScale, npcCore.transform.localScale);
        }

    }

}