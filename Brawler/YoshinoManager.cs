using DragonEngineLibrary;
using DragonEngineLibrary.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    internal static class YoshinoManager
    {
        private static bool m_spawnYoshinoDoOnce = false;

        public static void Update()
        {
            return;

            if (m_spawnYoshinoDoOnce)
                if (!BrawlerBattleManager.KasugaChara.IsValid())
                {
                    m_spawnYoshinoDoOnce = false;
                    return;
                }

            if (!m_spawnYoshinoDoOnce)
            {
                if(CanSpawnYoshino())
                {
                    switch(SceneService.GetSceneInfo().ScenePlay.Get().StageID)
                    {
                        case StageID.st_kamuro_yazawa_past:
                            SpawnYoshinoKamuro2000();
                            break;
                        case StageID.st_kamuro:
                            SpawnYoshinoKamuro();
                            break;
                        case StageID.st_yokohama:
                            SpawnYoshinoYokohama();
                            break;
                        case StageID.st_osaka:
                            SpawnYoshinoYokohama();
                            break;
                    }

                    m_spawnYoshinoDoOnce = true;
                    DragonEngine.Log("Yoshino Spawned");
                }
            }

        }

        private static void SpawnYoshinoGeneric(Vector4 position, float angle)
        {
            NPCRequestMaterial material = new NPCRequestMaterial();
            material.Material = new NPCMaterial();

            Character player = DragonEngine.GetHumanPlayer();

            material.Material.pos_ = position;
            material.Material.rot_y_ = angle;
            material.Material.character_id_ = (CharacterID)101;
            material.Material.npc_setup_id_ = CharacterNPCSetup.no_reaction;
            material.Material.map_icon_id_ = MapIconID.store;
            material.Material.collision_type_ = 0;

            material.Material.is_eternal_life_ = true;
            material.Material.is_encounter_ = false;
            material.Material.is_encount_btl_type_ = false;

            material.Material.parent_ = SceneService.CurrentScene.Get().GetSceneEntity<EntityBase>(SceneEntity.character_manager).UID;

            material.Material.is_force_create_ = true;
            material.Material.is_force_visible_ = true;
            material.Material.is_short_life_ = false;

            NPCFactory.RequestCreate(material);
        }

        private static void SpawnYoshinoKamuro2000()
        {
            SpawnYoshinoGeneric(new Vector4(32.12f, 0.15f, 26.49f), 0);
        }

        private static void SpawnYoshinoKamuro()
        {
        }

        private static void SpawnYoshinoYokohama()
        {
            SpawnYoshinoGeneric(new Vector4(3.73f, 3.80f, 291.09f), -2.464463f);
            SpawnYoshinoGeneric(new Vector4(-257.61f, 0.10f, 86.57f), -1.49f);
        }

        private static void SpawnYoshinoSotenbori()
        {

        }

        private static bool CanSpawnYoshino()
        {
            bool minigame = GameVarManager.GetValueBool(GameVarID.is_minigame);
            bool chase = GameVarManager.GetValueBool(GameVarID.is_chase);
            bool player_exists = BrawlerBattleManager.KasugaChara.IsValid();
            bool is_cinematic = GameVarManager.GetValueBool(GameVarID.is_cinematic);
            bool is_battle = GameVarManager.GetValueBool(GameVarID.is_battle);
            bool is_hact = GameVarManager.GetValueBool(GameVarID.is_hact);

            return !minigame && !chase && player_exists && !is_cinematic && !is_battle && !is_hact;

        }


    }
}
