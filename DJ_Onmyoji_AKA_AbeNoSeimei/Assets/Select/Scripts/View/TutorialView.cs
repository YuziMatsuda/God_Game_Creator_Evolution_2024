using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Select.Common;

namespace Select.View
{
    /// <summary>
    /// �r���[
    /// �`���[�g���A�����
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class TutorialView : MonoBehaviour
    {
        /// <summary>�y�[�W�X�N���[���̃C���f�b�N�X</summary>
        [SerializeField] private float[] pagesPos = { 0f, .25f, .5f, .75f, 1f };
        /// <summary>�X�N���[������</summary>
        [SerializeField] private ScrollRect scrollRect;
        /// <summary>�A�j���[�V�����Đ�����</summary>
        [SerializeField] private float duration = .1f;
        /// <summary>����܂ł̎���</summary>
        [SerializeField] private float closedTime = .5f;

        private void Reset()
        {
            scrollRect = GetComponent<ScrollRect>();
        }

        /// <summary>
        /// �t�F�[�h��DOTween�A�j���[�V�����Đ�
        /// </summary>
        /// <param name="observer">�o�C���h</param>
        /// <param name="state">�X�e�[�^�X</param>
        /// <returns>�����^���s</returns>
        public IEnumerator PlayCloseAnimation(System.IObserver<bool> observer)
        {
            DOVirtual.DelayedCall(closedTime, () =>
            {
                observer.OnNext(true);
            });
            yield return null;
        }

        /// <summary>
        /// �y�[�W�ʒu�̕ύX
        /// </summary>
        /// <param name="pageIndex">�y�[�W�ԍ�</param>
        /// <returns>�����^���s</returns>
        public bool SetPage(EnumTutorialPagesIndex pageIndex)
        {
            try
            {
                scrollRect.horizontalNormalizedPosition = pagesPos[(int)pageIndex];
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// �y�[�W���O�A�j���[�V����
        /// </summary>
        /// <param name="pageIndex">�y�[�W�ԍ�</param>
        /// <returns>�����^���s</returns>
        public bool PlayPagingAnimation(EnumTutorialPagesIndex pageIndex)
        {
            try
            {
                scrollRect.DOHorizontalNormalizedPos(pagesPos[(int)pageIndex], duration)
                    .SetUpdate(true);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }

}
