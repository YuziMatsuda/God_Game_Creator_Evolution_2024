using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Main.Common;
using Main.Model;
using Main.Test.Driver;
using MoonSharp.Interpreter;
using UnityEngine;
using Universal.Bean;
using Universal.Common;

namespace Main.Utility
{
    /// <summary>
    /// 式神タイプ別パラメータ管理
    /// 式神情報を元に設定値の内容を取得する
    /// </summary>
    /// <see href="https://www.notion.so/b17775a9a3b34f27a911c50454d5165e?pvs=4"/>
    /// <see href="https://www.notion.so/a72678495bbf42b2af5f88bcfcc29198?pvs=4"/>
    public class ShikigamiParameterUtility : IShikigamiParameterUtility
    {
        /// <summary>共通のユーティリティ</summary>
        private MainCommonUtility _common = new MainCommonUtility();

        /// <summary>
        /// スロット番号が未セットであることを表す定数
        /// </summary>
        private const int UNSET_SLOT_NUMBER = -1;

        public float GetMainSkillValue(ShikigamiInfo shikigamiInfo, MainSkillType mainSkillType)
        {
            var skillLists = _common.AdminDataSingleton.AdminBean.levelDesign.mainSkillLists;
            if (skillLists.Length < 1)
                throw new System.Exception($"{skillLists.Length}つのメインスキルプロパティから取得できない");

            var array = skillLists.Where(q => ((ShikigamiType)q.shikigamiType).Equals(shikigamiInfo.prop.type) &&
                ((MainSkillType)q.mainSkillType).Equals(mainSkillType) &&
                ((SkillRank)q.skillRank).Equals(GetMainSkillRank(shikigamiInfo, mainSkillType)))
                .Select(q => q.value)
                .ToArray();
            if (array.Length < 1)
                throw new System.Exception($"{skillLists.Length}つのメインスキルプロパティから取得できない[{shikigamiInfo.prop.type}][{mainSkillType}]");

            return array[0];
        }

        /// <summary>
        /// スキルランクの取得
        /// </summary>
        /// <param name="shikigamiInfo">式神の情報</param>
        /// <param name="mainSkillType">スキルタイプ</param>
        /// <returns>スキルランク</returns>
        /// <exception cref="System.Exception">メインスキルプロパティが1つもない場合、または指定したスキルタイプのメインスキルプロパティが1つもない場合にスローされます</exception>
        private SkillRank GetMainSkillRank(ShikigamiInfo shikigamiInfo, MainSkillType mainSkillType)
        {
            var skills = shikigamiInfo.prop.mainSkills;
            if (skills.Length < 1)
                throw new System.Exception($"{skills.Length}つのメインスキルプロパティから取得できない");

            var array = skills.Where(q => q.type.Equals(mainSkillType))
                .Select(q => q.rank)
                .ToArray();
            if (array.Length < 1)
                throw new System.Exception($"{skills.Length}つのメインスキルプロパティから取得できない[{mainSkillType}]");

            return array[0];
        }

        public ShikigamiInfo GetShikigamiInfo(PentagramTurnTableInfo pentagramTurnTableInfo, int instanceId)
        {
            var slots = pentagramTurnTableInfo.slots;
            if (slots.Length < 1)
                throw new System.Exception($"{slots.Length}つのスロットから取得できない");
            
            var array = pentagramTurnTableInfo.slots.Where(q => q.prop.instanceId == instanceId)
                .Select(q => q.prop.shikigamiInfo)
                .ToArray();
            if (array.Length < 1)
                throw new System.Exception($"{slots.Length}つのスロットから取得できない[{instanceId}]");

            return array[0];
        }

        public PentagramTurnTableInfo GetPentagramTurnTableInfo()
        {
            try
            {
                List<PentagramTurnTableInfo.Slot> slots = new List<PentagramTurnTableInfo.Slot>();
                var bean = _common.UserDataSingleton.UserBean;
                if (bean.pentagramTurnTableInfo == null ||
                    bean.pentagramTurnTableInfo.slots == null ||
                    bean.pentagramTurnTableInfo.slots.Length < 2)
                    throw new System.Exception("スロットから取得できない");

                foreach (var item in bean.pentagramTurnTableInfo.slots)
                {
                    slots.Add(new PentagramTurnTableInfo.Slot()
                    {
                        prop = new PentagramTurnTableInfo.Slot.Prop()
                        {
                            slotId = (SlotId)item.slotId,
                            shikigamiInfo = new ShikigamiInfo()
                            {
                                prop = new ShikigamiInfo.Prop()
                                {
                                    type = (ShikigamiType)item.shikigamiInfo.type,
                                    level = item.shikigamiInfo.level,
                                    mainSkills = ConvertMainSkills(item.shikigamiInfo.mainSkills),
                                    subSkills = ConvertSubSkills(item.shikigamiInfo.subSkills),
                                }
                            },
                            instanceId = UNSET_SLOT_NUMBER,
                        },
                    });
                }
                PentagramTurnTableInfo info = new PentagramTurnTableInfo()
                {
                    slots = slots.ToArray()
                };

                return info;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// メインスキルを変換
        /// </summary>
        /// <param name="mainSkills">Bean側のメインスキル</param>
        /// <returns>変換後のメインスキル</returns>
        /// <exception cref="System.Exception">引数データが存在しない場合に例外をスロー</exception>
        private ShikigamiInfo.Prop.MainSkill[] ConvertMainSkills(UserBean.ShikigamiInfo.MainSkill[] mainSkills)
        {
            if (mainSkills == null ||
                mainSkills.Length < 1)
                throw new System.Exception("mainSkillsが空またはnullです");
            
            List<ShikigamiInfo.Prop.MainSkill> skills = new List<ShikigamiInfo.Prop.MainSkill>();
            foreach (var item in mainSkills)
                skills.Add(new ShikigamiInfo.Prop.MainSkill()
                {
                    type = (MainSkillType)item.type,
                    rank = (SkillRank)item.rank,
                });
            return skills.ToArray();
        }

        /// <summary>
        /// サブスキルを変換
        /// </summary>
        /// <param name="subSkills">Bean側のサブスキル</param>
        /// <returns>変換後のサブスキル</returns>
        /// <exception cref="System.Exception">引数データが存在しない場合に例外をスロー</exception>
        private ShikigamiInfo.Prop.SubSkill[] ConvertSubSkills(UserBean.ShikigamiInfo.SubSkill[] subSkills)
        {
            // subSkillsが空またはnullは許容
            if (subSkills == null ||
                subSkills.Length < 1)
                return new ShikigamiInfo.Prop.SubSkill[0];

            List<ShikigamiInfo.Prop.SubSkill> skills = new List<ShikigamiInfo.Prop.SubSkill>();
            foreach (var item in subSkills)
                skills.Add(new ShikigamiInfo.Prop.SubSkill()
                {
                    type = (SubSkillType)item.type,
                    rank = (SkillRank)item.rank,
                });
            return skills.ToArray();
        }
    }

    /// <summary>
    /// 式神タイプ別パラメータ管理
    /// 式神情報を元に設定値の内容を取得する
    /// インタフェース
    /// </summary>
    public interface IShikigamiParameterUtility
    {
        /// <summary>
        /// メインスキル値の取得
        /// </summary>
        /// <param name="shikigamiInfo">式神の情報</param>
        /// <param name="mainSkillType">スキルタイプ</param>
        /// <returns>メインスキル値</returns>
        /// <exception cref="System.Exception">メインスキルプロパティが1つもない場合、または指定したスキルタイプのメインスキルプロパティが1つもない場合にスローされます</exception>
        public float GetMainSkillValue(ShikigamiInfo shikigamiInfo, MainSkillType mainSkillType);
        
        /// <summary>
        /// 式神の情報を取得
        /// </summary>
        /// <param name="pentagramTurnTableInfo">ペンダグラムターンテーブル情報</param>
        /// <param name="instanceId">オブジェクトID</param>
        /// <returns>式神の情報</returns>
        public ShikigamiInfo GetShikigamiInfo(PentagramTurnTableInfo pentagramTurnTableInfo, int instanceId);
        
        /// <summary>
        /// ペンダグラムターンテーブル情報を取得
        /// </summary>
        /// <returns>ペンダグラムテーブル情報</returns>
        public PentagramTurnTableInfo GetPentagramTurnTableInfo();
    }
}
