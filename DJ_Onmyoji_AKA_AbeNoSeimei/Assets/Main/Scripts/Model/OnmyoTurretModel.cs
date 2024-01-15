using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;

namespace Main.Model
{
    /// <summary>
    /// 陰陽玉（陰陽砲台）
    /// </summary>
    public class OnmyoTurretModel : TurretModel
    {
        protected override IEnumerator InstanceCloneObjects(float instanceRateTimeSec, ObjectsPoolModel objectsPoolModel)
        {
            // 一定間隔で弾を生成するための実装
            while (true)
            {
                var bullet = objectsPoolModel.GetOnmyoBulletModel();
                if (!bullet.Initialize(CalibrationFromTarget(RectTransform),
                    RectTransform.parent.eulerAngles,
                    _utility.GetMainSkillValue(_shikigamiInfo, MainSkillType.BulletLifeTime),
                    (int)_utility.GetMainSkillValue(_shikigamiInfo, MainSkillType.AttackPoint)))
                    Debug.LogError("Initialize");
                if (!bullet.isActiveAndEnabled)
                    bullet.gameObject.SetActive(true);
                yield return new WaitForSeconds(instanceRateTimeSec);
            }
        }
    }
}
