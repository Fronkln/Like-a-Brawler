using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using DragonEngineLibrary;
using YazawaCommand;


namespace Brawler
{
    public class HeatAction : MoveBase
    {
        public TalkParamID heatAction;
        public HeatActionCondition heatAttackCond;
        public float dist;
        public int damage;
        public int numTargets;
        float killTime = 0;

        public static float Cost = 0.4f;


        public static HeatAction LastHeatAction = null;

        public HeatAction(TalkParamID action, HeatActionCondition cond, float dist, int damage, int numTargets = 1, float killTime = 0) : base(1, null, AttackConditionType.None)
        {
            heatAction = action;
            heatAttackCond = cond;
            this.dist = dist;
            this.numTargets = numTargets;
            this.damage = damage;
            this.killTime = killTime;
        }

        public override void OnMoveEnd()
        {
            
        }

        public override bool CheckConditions(Fighter fighter, Fighter[] targets)
        {
            int curHeat = fighter.GetStatus().Heat;
            BrawlerFighterInfo info = BrawlerFighterInfo.Infos[fighter.Character.UID];

            if (curHeat < Player.GetHeatMax(Player.ID.kasuga) * Cost)
                return false;

            List<Fighter> fightersToCheck = new List<Fighter>();

            for (int i = 0; i < numTargets; i++)
                fightersToCheck.Add(targets[i]);

            Vector3[] positions = fightersToCheck.Select(x => (Vector3)x.Character.GetPosCenter()).ToArray();
            Vector3 posSum = Vector3.zero;

            foreach (Vector3 pos in positions)
                posSum += pos;

            Vector3 center = posSum / positions.Length;

            if (Vector3.Distance(positions[0], center) > dist)
                return false;

            if (heatAttackCond == HeatActionCondition.None)
                return true;


            return true;
        }

        /// <summary>
        /// Assuming the conditions were met
        /// </summary>
        public override void Execute(Fighter attacker, Fighter[] target)
        {
            base.Execute(attacker, target);


            HActRequestOptions opts = new HActRequestOptions();
            opts.base_mtx.matrix = attacker.Character.GetPosture().GetRootMatrix();
            opts.id = heatAction;
            opts.is_force_play = true;


            opts.Register(HActReplaceID.hu_player1, attacker.Character);
            opts.RegisterWeapon(AuthAssetReplaceID.we_player_r, attacker.GetWeapon(AttachmentCombinationID.right_weapon));
            //opts.RegisterFighterAndWeapon(HActReplaceID.hu_player1, attacker, AuthAssetReplaceID.we_player_r);
           
            Fighter[] enemies = FighterManager.GetAllEnemies().Where(x => !x.IsDead()).OrderBy(x => Vector3.Distance((Vector3)attacker.Character.Transform.Position, (Vector3)x.Character.Transform.Position)).ToArray();
            HActReplaceID curReplace = HActReplaceID.hu_enemy_00;

            List<Fighter> affectedEnemies = new List<Fighter>();

            SoundManager.PlayCue(SoundCuesheetID.battle_common, 10, 0);

            for(int i = 0; i < numTargets; i++)
            {
                if (i >= enemies.Length)
                    break;

                opts.Register(curReplace, enemies[i].Character.UID);
                curReplace = (HActReplaceID)((int)curReplace + 1);
                affectedEnemies.Add(enemies[i]);
            }

            ECBattleStatus kasugaStatus = BrawlerBattleManager.Kasuga.GetStatus();

            BattleTurnManager.RequestHActEvent(opts);
            LastHeatAction = this;
            BrawlerBattleManager.KasugaChara.Status.SetNoInputTemporary();

            int heatReduce = (int)(Player.GetHeatMax(Player.ID.kasuga) * 0.3f);
            int newHeat = Player.GetHeatNow(Player.ID.kasuga) - heatReduce;

            if (newHeat < 0)
                newHeat = 0;

            Player.SetHeatNow(Player.ID.kasuga, newHeat);
        }

        public override bool MoveExecuting()
        {
            return BrawlerBattleManager.HActIsPlaying;
        }
    }
}
