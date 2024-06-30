using System.Collections;
using System.Collections.Generic;
using Main.Common;
using Main.Utility;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Main.Model
{
    /// <summary>
    /// 敵をスポーン
    /// </summary>
    public class EnemiesSpawnModel : SpawnModel, IEnemiesSpawnModel
    {
        /// <summary>最小半径</summary>
        [SerializeField] private float radiusMin = 10f;
        /// <summary>最大半径</summary>
        [SerializeField] private float radiusMax = 12f;
        /// <summary>トランスフォーム</summary>
        private Transform _target;
        /// <summary>陰陽（昼夜）の状態</summary>
        private float _onmyoState;
        /// <summary>敵のスポーンテーブル</summary>
        private EnemiesSpawnTable[] _enemiesSpawnTables = new EnemiesSpawnTable[2];

        protected override void Start()
        {
            var utility = new MainCommonUtility();
            // instanceRateTimeSec = utility.AdminDataSingleton.AdminBean.enemiesSpawnModel.invincibleTimeSec;

            base.Start();
            Observable.FromCoroutine<Transform>(observer => WaitForTarget(observer))
                .Subscribe(x => _target = x)
                .AddTo(gameObject);
        }

        /// <summary>
        /// ターゲットが生成されるまで待機
        /// </summary>
        /// <param name="observer">トランスフォーム</param>
        /// <returns>コルーチン</returns>
        private IEnumerator WaitForTarget(System.IObserver<Transform> observer)
        {
            Transform target = null;
            while (target == null)
            {
                var obj = GameObject.FindGameObjectWithTag(ConstTagNames.TAG_NAME_PLAYER);
                if (obj != null)
                    target = obj.transform;
                yield return null;
            }
            observer.OnNext(target);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_target != null ? _target.position : Vector2.zero, radiusMin);
            Gizmos.DrawWireSphere(_target != null ? _target.position : Vector2.zero, radiusMax);
        }

        protected override bool InstanceCloneObjects(float instanceRateTimeSec, ObjectsPoolModel objectsPoolModel)
        {
            try
            {
                float elapsedTime = 0f;
                var spawnUtility = new SpawnUtility();
                // 一定間隔で敵を生成するための実装
                this.UpdateAsObservable()
                    .Subscribe(_ =>
                    {
                        if (isActiveAndEnabled)
                        {
                            if (!spawnUtility.ManageEnemiesSpawn(_enemiesSpawnTables,
                                ref elapsedTime,
                                _target,
                                objectsPoolModel,
                                radiusMin,
                                radiusMax,
                                _onmyoState))
                                Debug.LogError("ManageEnemiesSpawn");
                        }
                    });
                // 必要になるまではゲームオブジェクトを無効
                gameObject.SetActive(false);

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public bool SetOnmyoState(float onmyoState)
        {
            try
            {
                _onmyoState = onmyoState;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public void SetEnemiesSpawnTable(EnemiesSpawnTable enemiesSpawnTable)
        {
            if (enemiesSpawnTable.sunMoonState.Equals(SunMoonState.Daytime))
                _enemiesSpawnTables[(int)SunMoonState.Daytime] = enemiesSpawnTable;
            if (enemiesSpawnTable.sunMoonState.Equals(SunMoonState.Night))
                _enemiesSpawnTables[(int)SunMoonState.Night] = enemiesSpawnTable;
        }
    }

    /// <summary>
    /// 敵をスポーン
    /// インターフェース
    /// </summary>
    public interface IEnemiesSpawnModel
    {
        /// <summary>
        /// 陰陽（昼夜）の状態をセット
        /// </summary>
        /// <param name="onmyoState">陰陽（昼夜）の状態</param>
        /// <returns>成功／失敗</returns>
        public bool SetOnmyoState(float onmyoState);
        /// <summary>
        /// オブジェクトを生成
        /// </summary>
        /// <param name="enemiesSpawnTable"></param>
        public void SetEnemiesSpawnTable(EnemiesSpawnTable enemiesSpawnTable);
    }

    /// <summary>
    /// 敵のスポーンテーブル
    /// </summary>
    [System.Serializable]
    public struct EnemiesSpawnTable
    {
        /// <summary>クローンオブジェクトを生成する時間間隔（秒）</summary>
        public float instanceRateTimeSec;
        /// <summary>クローンオブジェクトを生成する回数</summary>
        public int instanceCount;
        /// <summary>敵ID配列</summary>
        public EnemiesID[] enemiesIDs;
        /// <summary>昼夜の状態</summary>
        public SunMoonState sunMoonState;
        /// <summary>クローンオブジェクトを生成する回数（次の加算値）</summary>
        public IReactiveProperty<float> instanceCountRemaining;
    }

    /// <summary>
    /// 敵のスポーン登録情報
    /// </summary>
    [System.Serializable]
    public struct EnemiesSpawnAssign
    {
        /// <summary>敵ID</summary>
        public EnemiesID enemiesID;
        /// <summary>敵のプレハブ</summary>
        public Transform enemyPrefab;
    }

    /// <summary>
    /// 敵ID
    /// </summary>
    public enum EnemiesID
    {
        /// <summary>雑魚敵A</summary>
        EN0000,
        /// <summary>雑魚敵B</summary>
        EN0001,
        /// <summary>雑魚敵C</summary>
        EN0002,
        /// <summary>雑魚敵D</summary>
        EN0003,
        /// <summary>雑魚敵E</summary>
        EN0004,
        EN0005,
        EN0006,
        EN0007,
        EN0008,
        EN0009,
        EN0010,
        EN0011,
        EN0012,
        EN0013,
        EN0014,
        /// <summary>ラップでしか雑魚敵</summary>
        EN0000_W,
        /// <summary>ダンスでしか雑魚敵</summary>
        EN0000_D,
        /// <summary>グラフィティでしか倒せない雑魚敵</summary>
        EN0000_G,
        /// <summary>中ボス敵A</summary>
        EN1000,
        /// <summary>中ボス敵B</summary>
        EN1001,
        /// <summary>中ボス敵C</summary>
        EN1002,
        /// <summary>中ボス敵D</summary>
        EN1003,
        /// <summary>中ボス敵E</summary>
        EN1004,
        /// <summary>中ボス敵F</summary>
        EN1005,
        /// <summary>大ボス敵A</summary>
        EN2000,
        /// <summary>大ボス敵B</summary>
        EN2001,
        /// <summary>大ボス敵C</summary>
        EN2002,
        /// <summary>大ボス敵D</summary>
        EN2003,
        /// <summary>大ボス敵E</summary>
        EN2004,
    }
}
