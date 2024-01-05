using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.View
{
    /// <summary>
    /// 陰陽（昼夜）のアイコン
    /// </summary>
    public class SunMoonStateIconView : MonoBehaviour, ISunMoonStateIconView
    {
        /// <summary>陰陽（昼夜）の状態によるアイコン角度</summary>
        [SerializeField] private IconRotateByState iconRotateByState;
        /// <summary>トランスフォーム</summary>
        private Transform _transform;
        /// <summary>トランスフォーム</summary>
        private Transform Transform => _transform != null ? _transform : _transform = transform;

        public Vector3 SetRotate(float onmyoStateValue)
        {
            if (onmyoStateValue < -1f ||
            1f < onmyoStateValue)
                throw new System.ArgumentOutOfRangeException("onmyoStateValue", "onmyoStateValue must be in the range of -1 to 1.");

            Vector3 result;
            if (onmyoStateValue == -1f)
                result = iconRotateByState.night;
            else if (onmyoStateValue < 0f)
                result = Vector3.Lerp(iconRotateByState.night, iconRotateByState.dayAndNight, onmyoStateValue + 1);
            else if (onmyoStateValue == 0f)
                result = iconRotateByState.dayAndNight;
            else // 0 < onmyoStateValue <= 1
                result = Vector3.Lerp(iconRotateByState.dayAndNight, iconRotateByState.daytime, onmyoStateValue);
            Transform.eulerAngles = result;

            return result;
        }

        private void Reset()
        {
            iconRotateByState.daytime = new Vector3(0f, 0f, -90f);
            iconRotateByState.dayAndNight = Vector3.zero;
            iconRotateByState.night = new Vector3(0f, 0f, 90f);
        }
    }

    /// <summary>
    /// 陰陽（昼夜）の状態によるアイコン角度
    /// </summary>
    [System.Serializable]
    public struct IconRotateByState
    {
        /// <summary>昼</summary>
        public Vector3 daytime;
        /// <summary>昼夜</summary>
        public Vector3 dayAndNight;
        /// <summary>夜</summary>
        public Vector3 night;
    }

    /// <summary>
    /// 陰陽（昼夜）のアイコン
    /// インタフェース
    /// </summary>
    public interface ISunMoonStateIconView
    {
        /// <summary>
        /// 角度をセット
        /// </summary>
        /// <param name="onmyoStateValue">陰陽（昼夜）の状態</param>
        /// <returns>変更後の角度</returns>
        public Vector3 SetRotate(float onmyoStateValue);
    }
}