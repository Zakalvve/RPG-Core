using System;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGDatabase;

//=============================================================
//||                   Generated by BansheeGz Code Generator ||
//=============================================================

[AddComponentMenu("BansheeGz/Generated/M_NPC")]
public partial class M_NPC : BGEntityGo
{
	public override BGMetaEntity MetaConstraint => MetaDefault;
	private static BansheeGz.BGDatabase.BGMetaNested _metaDefault;
	public static BansheeGz.BGDatabase.BGMetaNested MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BansheeGz.BGDatabase.BGMetaNested>(new BGId(4956097922287413687UL,5297873573340634261UL), () => _metaDefault = null));
	public static BansheeGz.BGDatabase.BGRepoEvents Events => BGRepo.I.Events;
	public System.String f_name => _f_name[Entity.Index];
	public BansheeGz.BGDatabase.BGEntity f_Scene
	{
		get => _f_Scene[Entity.Index];
		set => _f_Scene[Entity.Index] = value;
	}
	public BansheeGz.BGDatabase.BGEntity f_npc => _f_npc[Entity.Index];
	private static BansheeGz.BGDatabase.BGFieldEntityName _ufle12jhs77_f_name;
	public static BansheeGz.BGDatabase.BGFieldEntityName _f_name => _ufle12jhs77_f_name ?? (_ufle12jhs77_f_name = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldEntityName>(MetaDefault, new BGId(5488248818935139405UL, 327170787595306633UL), () => _ufle12jhs77_f_name = null));
	private static BansheeGz.BGDatabase.BGFieldRelationSingle _ufle12jhs77_f_Scene;
	public static BansheeGz.BGDatabase.BGFieldRelationSingle _f_Scene => _ufle12jhs77_f_Scene ?? (_ufle12jhs77_f_Scene = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldRelationSingle>(MetaDefault, new BGId(4835906638483492665UL, 7373194279420833979UL), () => _ufle12jhs77_f_Scene = null));
	private static BansheeGz.BGDatabase.BGFieldRelationSingle _ufle12jhs77_f_npc;
	public static BansheeGz.BGDatabase.BGFieldRelationSingle _f_npc => _ufle12jhs77_f_npc ?? (_ufle12jhs77_f_npc = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldRelationSingle>(MetaDefault, new BGId(4743586419059696507UL, 16182210010245930658UL), () => _ufle12jhs77_f_npc = null));
}