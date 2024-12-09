using System.Collections;
using System.Collections.Generic;
using Main.Common;
using Main.Utility;
using UnityEngine;

namespace Main.Model
{
    /// <summary>
    /// 敵が攻撃範囲へ侵入した判定のトリガー
    /// </summary>
    public class SearchRangeOfEnemyCollider : DamageSufferedZoneModel, ISearchRangeOfEnemyCollider
    {
        /// <summary>ターゲット</summary>
        public Transform Target {get; set; }
        /// <summary>円形コライダー2D</summary>
        [SerializeField] private CircleCollider2D circleCollider2D;

        private bool isEndHoming;

        private void Reset()
        {
            tags = new string[1];
            tags[0] = ConstTagNames.TAG_NAME_ENEMY;
            circleCollider2D = GetComponent<CircleCollider2D>();
            isEndHoming = false;
        }

        protected override void Start() { }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (_utility.IsCompareTagAndUpdateReactiveFlag(other, tags, IsHit) && !isEndHoming)
                Target = other.transform;
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            if (other.transform == Target)
                EndHoming();
        }

        protected override void OnDisable()
        {
            Target = null;
            isEndHoming = false;
            base.OnDisable();
        }

        public void EndHoming()
        {
            isEndHoming = true;
            Target = null;
        }

        public bool SetRadiosOfCircleCollier2D(float radios)
        {
            try
            {
                circleCollider2D.radius = radios;

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
    /// 敵が攻撃範囲へ侵入した判定のトリガー
    /// インターフェース
    /// </summary>
    public interface ISearchRangeOfEnemyCollider : IUniversalCollider
    {

    }
}
