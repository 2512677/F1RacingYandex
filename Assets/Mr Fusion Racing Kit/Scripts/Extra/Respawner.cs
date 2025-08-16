using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class Respawner : MonoBehaviour
    {
        public RespawnSettings respawnSettings;
        private bool isSafe;
        private Sensor respawnSensor;
        private MeshRenderer[] renderers;
        private float flickerRate;
        private Rigidbody rigid;
        private RacerStatistics racerStatistics;
        private float lastRespawn;
        private bool isFlipped;
        private float respawnWaitTimer;
        private bool hasRespawned = false;
        private TrackLayout trackLayout;


        void Awake()
        {
            //Get components
            rigid = GetComponent<Rigidbody>();
            racerStatistics = GetComponent<RacerStatistics>();
            trackLayout = FindObjectOfType<TrackLayout>();

            //Get all renderers
            renderers = transform.GetComponentsInChildren<MeshRenderer>();

            //Create the sensor
            BoxCollider sensor = new GameObject("RespawnSensor").AddComponent<BoxCollider>();
            sensor.isTrigger = true;
            sensor.transform.SetParent(transform, false);
            sensor.size = Helper.GetTotalMeshFilterBounds(transform).size;
            respawnSensor = sensor.gameObject.AddComponent<Sensor>();
            respawnSensor.AddLayer(LayerMask.NameToLayer("Vehicle"));
        }


        void Update()
        {
            if (isFlipped)
            {
                respawnWaitTimer += Time.deltaTime;
                if (respawnWaitTimer > respawnSettings.respawnWait)
                {
                    Respawn();
                }
            }
            else
            {
                respawnWaitTimer = 0;
            }
        }



        void FixedUpdate()
        {
            // Проверяем, сильно ли отклонён вектор "вверх" автомобиля от мирового "вверх"
            isFlipped = Vector3.Dot(transform.up, Vector3.up) < 0.5f;
            isSafe = respawnSensor.collidersInRange.Count == 0;
        }



        public void Respawn()
        {
            // Если уже произошёл респаун недавно, не делаем новый
            if (hasRespawned)
            {
                Debug.Log("[Respawn] Already respawned recently; skipping.");
                return;
            }

            // Не респаунимся, пока гонка не стартовала
            if (RaceManager.instance != null && !RaceManager.instance.raceStarted)
                return;

            if (Time.time > lastRespawn)
            {
                lastRespawn = Time.time + 5;

                if (rigid != null)
                {
                    rigid.linearVelocity = Vector3.zero;
                    rigid.angularVelocity = Vector3.zero;
                }

                if (racerStatistics != null)
                {
                    // 1) Базовый узел респауна (как было раньше)
                    Transform node = racerStatistics.GetSafeRespawnNode();

                    // 2) ДОПОЛНИТЕЛЬНО: если это игрок в режиме Chase И цели нет –
                    //    респаунимся возле бота-лидера
                    if (RaceManager.instance &&
                        RaceManager.instance.raceType == RaceType.Chase &&
                        racerStatistics.isPlayer)
                    {
                        var ptm = FindObjectOfType<PursuitTargetManager>();
                        bool hasTarget = ptm && ptm.currentTarget;

                        if (!hasTarget)                    // цель не выбрана
                        {
                            BotHealth best = null;
                            float bestDt = -1f;

                            foreach (var bot in FindObjectsOfType<BotHealth>())
                            {
                                if (bot.dead) continue;

                                var rs = bot.GetComponent<RacerStatistics>();
                                if (!rs || rs.isPlayer) continue;   // пропускаем игрока

                                if (rs.totalDistance > bestDt)
                                {
                                    bestDt = rs.totalDistance;
                                    best = bot;
                                }
                            }

                            if (best)
                            {
                                var leaderRS = best.GetComponent<RacerStatistics>();
                                Transform lNode = leaderRS ? leaderRS.GetSafeRespawnNode() : null;
                                if (lNode) node = lNode;            // переопределяем узел
                            }
                        }
                    }

                    // 3) Телепортируем
                    if (node != null)
                    {
                        Debug.Log("[Respawn] Respawning at node position: " + node.position);
                        rigid.position = new Vector3(node.position.x, node.position.y + 1.0f, node.position.z);
                        Vector3 nodeEuler = node.rotation.eulerAngles;
                        rigid.rotation = Quaternion.Euler(0, nodeEuler.y, 0);
                        racerStatistics.RevertTotalDistance();
                    }
                }

                SendMessage("ResetValues", SendMessageOptions.DontRequireReceiver);
                StartCoroutine(RespawnRoutine());

                // Ставим флаг, чтобы избежать повторных респаунов
                hasRespawned = true;
                StartCoroutine(ClearRespawnFlag());
            }
        }


        IEnumerator ClearRespawnFlag()
        {
            // Например, ждем 2 секунды, затем разрешаем новый респаун
            yield return new WaitForSeconds(2.0f);
            hasRespawned = false;
            //Debug.Log("[Respawn] Respawn flag cleared.");
        }




        IEnumerator RespawnRoutine()
        {
            gameObject.SetColliderLayer("IgnoreCollision");

            float timer = 0;

            while (timer < respawnSettings.ignoreCollisionDuration)
            {
                if (isSafe)
                {
                    timer += Time.deltaTime;
                }

                if (respawnSettings.meshFlicker)
                {
                    Flicker();
                }

                yield return null;
            }

            gameObject.SetColliderLayer("Vehicle");

            if (respawnSettings.meshFlicker)
            {
                foreach (Renderer r in renderers)
                {
                    if (!r.GetComponent<ParticleSystem>())
                        r.enabled = true;
                }
            }
        }


        void Flicker()
        {
            flickerRate = Mathf.Repeat(Time.time * 4.0f, 2);

            foreach (Renderer r in renderers)
            {
                if (!r.GetComponent<ParticleSystem>())
                    r.enabled = flickerRate < 1;
            }
        }
    }

    [System.Serializable]
    public class RespawnSettings
    {
        public bool enableRespawns = true; //are the racers able to respawn
        public bool autoRespawn; //respawn when flipped
        public bool meshFlicker; //should the mesh flicker when respawning
        public float ignoreCollisionDuration = 3; //how long should the vehicle ignore collisions when repsawning
        public float respawnWait = 5; //how long to wait when stuck or flipped over
    }
}