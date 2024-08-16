using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Select.Model
{
    /// <summary>
    /// ���f��
    /// �`���[�g���A����ʃy�[�W
    /// </summary>
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(EventTrigger))]
    public class TutorialViewPageModel : UIEventController
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            Time.timeScale = 0f;
            if (_button == null)
                _button = GetComponent<Button>();
            _button.enabled = true;
            if (_eventTrigger == null)
                _eventTrigger = GetComponent<EventTrigger>();
            _eventTrigger.enabled = true;
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
        }

        /// <summary>
        /// �{�^���̃X�e�[�^�X��ύX
        /// </summary>
        /// <param name="enabled">�L���^����</param>
        /// <returns>�����^���s</returns>
        public bool SetButtonEnabled(bool enabled)
        {
            try
            {
                if (_button == null)
                    _button = GetComponent<Button>();
                _button.enabled = enabled;
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// �C�x���g�g���K�[�̃X�e�[�^�X��ύX
        /// </summary>
        /// <param name="enabled">�L���^����</param>
        /// <returns>�����^���s</returns>
        public bool SetEventTriggerEnabled(bool enabled)
        {
            try
            {
                if (_eventTrigger == null)
                    _eventTrigger = GetComponent<EventTrigger>();
                _eventTrigger.enabled = enabled;

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
