using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Main.Model;
using UniRx;
using UnityEngine;

namespace Main.Utility
{
    /// <summary>
    /// 敵とプレイヤーのユーティリティ
    /// </summary>
    public class EnemyPlayerModelUtility : IEnemyPlayerModelUtility
    {
        public bool IsCompareTagAndUpdateReactiveFlag(Collider2D other, string[] tags, IReactiveProperty<bool> isHit)
        {
            return 0 < tags.Where(q => other.CompareTag(q))
            .Select(q => q)
            .ToArray()
            .Length &&
            !isHit.Value;
        }

        public bool UpdateStateHPAndIsDead(CharacterState state)
        {
            try
            {
                state.IsHit.ObserveEveryValueChanged(x => x.Value)
                    .Subscribe(x =>
                    {
                        if (x)
                            if (state.Damage.Value < state.HP.Value)
                                state.HP.Value -= state.Damage.Value;
                            else
                            {
                                state.HP.Value = 0;
                                state.IsDead.Value = x;
                            }
                    });

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
    /// 敵とプレイヤーのユーティリティ
    /// インターフェース
    /// </summary>
    public interface IEnemyPlayerModelUtility
    {
        /// <summary>
        /// 対象がタグ内に含まれている場合はフラグを更新
        /// </summary>
        /// <param name="other">衝突した対象</param>
        /// <param name="tags">対象タグ</param>
        /// <param name="isHit">ヒットフラグ</param>
        /// <returns></returns>
        public bool IsCompareTagAndUpdateReactiveFlag(Collider2D other, string[] tags, IReactiveProperty<bool> isHit);

        /// <summary>
        /// ステータスを更新する
        /// HPを加算してMAXを超えると死亡フラグを有効にする
        /// </summary>
        /// <param name="isHit">当たったか</param>
        /// <param name="hp">HP</param>
        /// <param name="hpMax">最大HP</param>
        /// <param name="isDead">死亡フラグ</param>
        /// <returns>成功／失敗</returns>
        public bool UpdateStateHPAndIsDead(CharacterState state);
    }
}