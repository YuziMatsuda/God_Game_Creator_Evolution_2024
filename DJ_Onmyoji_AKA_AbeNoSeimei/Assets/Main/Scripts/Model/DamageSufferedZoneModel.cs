using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Main.Common;
using Main.Utility;
using UniRx;
using UnityEngine;
using Universal.Utility;

namespace Main.Model
{
    /// <summary>
    /// 攻撃を受ける判定のトリガー
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class DamageSufferedZoneModel : MonoBehaviour, IDamageSufferedZoneModel
    {
        /// <summary>当たったか</summary>
        public IReactiveProperty<bool> IsHit { get; private set; } = new BoolReactiveProperty();
        /// <summary>接触対象のオブジェクトタグ</summary>
        [Tooltip("接触対象のオブジェクトタグ")]
        [SerializeField] protected string[] tags = { ConstTagNames.TAG_NAME_ENEMY };
        /// <summary>無敵時間（秒）</summary>
        [Tooltip("無敵時間（秒）")]
        [SerializeField] protected float invincibleTimeSec = 1f;
        /// <summary>ユーティリティ</summary>
        private EnemyPlayerModelUtility _utility = new EnemyPlayerModelUtility();
        /// <summary>2Dコライダー</summary>
        private CircleCollider2D _collider2D;
        /// <summary>2Dコライダー</summary>
        private CircleCollider2D Collider2D => _collider2D != null ? _collider2D : _collider2D = GetComponent<CircleCollider2D>();
        /// <summary>ダメージ値</summary>
        public IReactiveProperty<int> Damage { get; private set; } = new IntReactiveProperty();
        /// <summary>攻撃力</summary>
        protected int AttackPoint { get; private set; }

        protected virtual void Start()
        {
            IsHit.ObserveEveryValueChanged(x => x.Value)
                .Subscribe(x =>
                {
                    if (x)
                    {
                        Collider2D.enabled = false;
                        if (gameObject.activeInHierarchy)
                            StartCoroutine(GeneralUtility.ActionsAfterDelay(invincibleTimeSec, () =>
                            {
                                if (!ResetState(IsHit, Collider2D))
                                    Debug.LogError("ResetState");
                            }));
                    }
                });
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (_utility.IsCompareTagAndUpdateReactiveFlag(other, tags, IsHit))
            {
                var atttack = other.GetComponent<DamageSufferedZoneModel>();
                if (atttack != null)
                    Damage.Value = atttack.AttackPoint;
                else
                    Damage.Value = 1;
                IsHit.Value = true;
            }
        }

        protected virtual void OnDisable()
        {
            if (!ResetState(IsHit, Collider2D))
                Debug.LogError("ResetState");
        }

        public bool SetAttackPoint(int attackPoint)
        {
            try
            {
                AttackPoint = attackPoint;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// ステータスのリセット
        /// </summary>
        /// <param name="isHit">当たったか</param>
        /// <param name="collider2D">2Dコライダー</param>
        /// <returns>成功／失敗</returns>
        private bool ResetState(IReactiveProperty<bool> isHit, CircleCollider2D collider2D)
        {
            try
            {
                isHit.Value = false;
                collider2D.enabled = true;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }

    /// <summary>
    /// 攻撃を与える判定のトリガー
    /// インターフェース
    /// </summary>
    public interface IDamageSufferedZoneModel
    {
        /// <summary>
        /// 攻撃力をセット
        /// </summary>
        /// <param name="attackPoint">攻撃力</param>
        /// <returns>成功／失敗</returns>
        public bool SetAttackPoint(int attackPoint);
    }
}