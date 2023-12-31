using System;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGDatabase;
using Alias_rifegrt_Player = M_Player;
using Alias_rifegrt_Character = M_Character;

//=============================================================
//||                   Generated by BansheeGz Code Generator ||
//=============================================================

[AddComponentMenu("BansheeGz/Generated/M_Scene")]
public partial class M_Scene : BGEntityGo
{
	public override BGMetaEntity MetaConstraint => MetaDefault;
	private static BansheeGz.BGDatabase.BGMetaRow _metaDefault;
	public static BansheeGz.BGDatabase.BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BansheeGz.BGDatabase.BGMetaRow>(new BGId(4834757969916592977UL,4169097687885404317UL), () => _metaDefault = null));
	public static BansheeGz.BGDatabase.BGRepoEvents Events => BGRepo.I.Events;
	public System.String f_name
	{
		get => _f_name[Entity.Index];
		set => _f_name[Entity.Index] = value;
	}
	public System.Collections.Generic.List<BansheeGz.BGDatabase.BGEntity> f_npc => _f_npc[Entity.Index];
	private static BansheeGz.BGDatabase.BGFieldEntityName _ufle12jhs77_f_name;
	public static BansheeGz.BGDatabase.BGFieldEntityName _f_name => _ufle12jhs77_f_name ?? (_ufle12jhs77_f_name = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldEntityName>(MetaDefault, new BGId(5643502971960994141UL, 176847129528869536UL), () => _ufle12jhs77_f_name = null));
	private static BansheeGz.BGDatabase.BGFieldNested _ufle12jhs77_f_npc;
	public static BansheeGz.BGDatabase.BGFieldNested _f_npc => _ufle12jhs77_f_npc ?? (_ufle12jhs77_f_npc = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldNested>(MetaDefault, new BGId(5733162371843618538UL, 1740328310659168696UL), () => _ufle12jhs77_f_npc = null));
	public List<BGEntity> RelatedPlayerListUsingsceneRelation => BGCodeGenUtils.GetRelatedInbound<BansheeGz.BGDatabase.BGEntity>(Alias_rifegrt_Player._f_scene, Entity.Id);
	public List<BGEntity> RelatedCharacterListUsingsceneRelation => BGCodeGenUtils.GetRelatedInbound<BansheeGz.BGDatabase.BGEntity>(Alias_rifegrt_Character._f_scene, Entity.Id);
}
