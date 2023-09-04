using System;
using DragonEngineLibrary;
using YazawaCommand;

namespace Brawler
{
    public class MoveBase
    {
        public virtual AttackType AttackType => AttackType.MoveBase;

        public AttackInput[] inputKeys;
        public AttackConditionType skillConditions;
        public float cooldown = 0;

        public MoveBase(float attackDelay, AttackInput[] input, AttackConditionType condition = AttackConditionType.None)
        {
            cooldown = attackDelay;
            inputKeys = input;
            skillConditions = condition;
        }

        /// <summary>
        /// We are no longer the active attack
        /// </summary>
        public virtual void OnMoveEnd() { }

        public virtual bool MoveExecuting()
        {
            return false;
        }

        public virtual bool IsSyncMove()
        {
            return false;
        }

        public virtual bool AllowHActWhileExecuting()
        {
            return false;
        }

        public virtual bool AllowChange()
        {
            return false;
        }

        public bool CheckSimpleConditions(Fighter fighter, Fighter[] targets)
        {
            if (!fighter.Character.IsValid())
                return false;

            if (skillConditions == AttackConditionType.None)
                return true;

            ECBattleStatus battleStatus = fighter.Character.GetBattleStatus();
            BrawlerFighterInfo info = BrawlerFighterInfo.Infos[fighter.Character.UID];

            if (skillConditions.HasFlag(AttackConditionType.LowHealth))
                if ((battleStatus.MaxHP * 0.4f) < battleStatus.CurrentHP)
                    return false;

            if (skillConditions.HasFlag(AttackConditionType.Down))
                if (!info.IsDown || info.IsGettingUp)
                    return false;

            if(skillConditions.HasFlag(AttackConditionType.LockedEnemyDown))
            {
                Fighter lockedEnemy = BrawlerPlayer.GetLockOnTarget(fighter);

                if (!lockedEnemy.IsValid())
                    return false;
                else
                {
                    BrawlerFighterInfo enemyInfo = BrawlerFighterInfo.Infos[lockedEnemy.Character.UID];

                    if (!enemyInfo.IsDown || enemyInfo.IsGettingUp)
                        return false;
                }
            }

            return true;
        }

        public virtual bool CheckConditions(Fighter fighter, Fighter[] targets)
        {
            return CheckSimpleConditions(fighter, targets);
        }

        public virtual void Execute(Fighter attacker, Fighter[] target) { }
        public virtual void InputUpdate() { }
        public virtual void Update() { }
    }

    public class MoveRPG : MoveBase
    {
        public override AttackType AttackType => AttackType.MoveRPG;

        public RPGSkillID ID;
        private bool m_isCooldown = false;
        private float m_cooldownDur = 1;

        private bool m_execStart = false;
        private bool m_gmtDone = false;

        public MoveRPG(RPGSkillID attack, float attackDuration, AttackInput[] input, AttackConditionType condition = AttackConditionType.None, float cooldown = 1) : base(attackDuration, input, condition)
        {
            ID = attack;
            m_cooldownDur = cooldown;
        }


        public override void Execute(Fighter attacker, Fighter[] target)
        {
            BattleTurnManager.ForceCounterCommand(attacker, target[0], ID);

            m_isCooldown = true;
            new SimpleTimer(m_cooldownDur, delegate { m_isCooldown = false; });

            m_execStart = true;
        }

        public override void Update()
        {
            base.Update();

            if(m_execStart)
            {

            }
        }

        public override void OnMoveEnd()
        {
            base.OnMoveEnd();

            m_execStart = false;
        }

        public override bool CheckConditions(Fighter fighter, Fighter[] targets)
        {
            if (m_isCooldown)
                return false;

            return base.CheckConditions(fighter, targets);
        }
    }
}
