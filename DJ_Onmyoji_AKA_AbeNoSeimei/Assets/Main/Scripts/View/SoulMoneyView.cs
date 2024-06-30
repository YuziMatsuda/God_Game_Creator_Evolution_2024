using System.Collections;
using System.Collections.Generic;
using Main.Model;
using Main.Utility;
using UniRx;
using UnityEngine;
using Universal.Utility;

namespace Main.View
{
    /// <summary>
    /// 魂の経験値
    /// ビュー
    /// </summary>
    public class SoulMoneyView : MonoBehaviour, ISoulMoneyView
    {
        /// <summary>アニメーション終了時間</summary>
        [SerializeField] private float[] durations = { 1.5f, 1.5f };
        /// <summary>トランスフォーム</summary>
        private Transform _transform;
        /// <summary>トランスフォーム</summary>
        public Transform Transform => _transform != null ? _transform : _transform = transform;
        /// <summary>ボディスプライトのビュー</summary>
        [SerializeField] private BodySpriteView bodySpriteView;
        /// <summary>獲得済みか</summary>
        public IReactiveProperty<bool> IsGeted { get; private set; } = new BoolReactiveProperty();
        /// <summary>敵のプロパティ</summary>
        private EnemiesProp _enemiesProp;
        /// <summary>敵のプロパティ</summary>
        public EnemiesProp EnemiesProp => _enemiesProp;
        /// <summary>アニメータのビュー</summary>
        [SerializeField] private AnimatorView animatorView;

        private Transform _player; // プレイヤーのTransform
        public float speed = 5f; // 移動速度

        private void Reset()
        {
            bodySpriteView = GetComponentInChildren<BodySpriteView>();
        }

        private void OnEnable()
        {
            IsGeted.Value = false;
            StartCoroutine(MoveToPlayer());

            //animatorView.SetFloat(ParametersOfAnim.MoveSpeed, 10f);
            //StartCoroutine(GeneralUtility.ActionsAfterDelay(durations[0],
            //    () => Observable.FromCoroutine<bool>(observer => bodySpriteView.PlayFadeAnimation(observer, Common.EnumFadeState.Close, durations[0]))
            //        .Subscribe(_ =>
            //        {
            //            IsGeted.Value = true;
            //            gameObject.SetActive(false);
            //        })
            //        .AddTo(gameObject)));
        }

        private IEnumerator MoveToPlayer()
        {
            if (_player != null) { 
            while (Vector3.Distance(transform.position, _player.position) > 0.1f)
            {
                Vector3 direction = (_player.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;

                yield return null; // 次のフレームまで待機
            }

            // 到着後の処理
            IsGeted.Value = true;
            gameObject.SetActive(false);
        }
        }

        private void Start()
        {
            gameObject.SetActive(false);
            Observable.FromCoroutine<Transform>(observer => WaitForTarget(observer))
                .Subscribe(x => _player = x)
                .AddTo(gameObject);
        }

        public bool SetEnemiesProp(EnemiesProp enemiesProp)
        {
            try
            {
                _enemiesProp = enemiesProp;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public bool PlayWalkingAnimation(float moveSpeed)
        {
            return animatorView.SetFloat(ParametersOfAnim.MoveSpeed, moveSpeed);
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
                var obj = GameObject.FindGameObjectWithTag("Player");
                if (obj != null)
                    target = obj.transform;
                yield return null;
            }
            observer.OnNext(target);
        }
    }

    /// <summary>
    /// 魂の経験値
    /// ビュー
    /// インターフェース
    /// </summary>
    public interface ISoulMoneyView
    {
        /// <summary>
        /// 敵のプロパティをセット
        /// </summary>
        /// <param name="enemiesProp">敵のプロパティ</param>
        /// <returns>成功／失敗</returns>
        public bool SetEnemiesProp(EnemiesProp enemiesProp);
    }
}
