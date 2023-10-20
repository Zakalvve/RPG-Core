using System;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGDatabase;
using Alias_rifegrt_Character = M_Character;

//=============================================================
//||                   Generated by BansheeGz Code Generator ||
//=============================================================

[AddComponentMenu("BansheeGz/Generated/M_EquipmentInventory")]
public partial class M_EquipmentInventory : BGEntityGo
{
	public override BGMetaEntity MetaConstraint => MetaDefault;
	private static BansheeGz.BGDatabase.BGMetaRow _metaDefault;
	public static BansheeGz.BGDatabase.BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BansheeGz.BGDatabase.BGMetaRow>(new BGId(5386561023320349282UL,8071079279079956097UL), () => _metaDefault = null));
	public static BansheeGz.BGDatabase.BGRepoEvents Events => BGRepo.I.Events;
	public System.String f_name
	{
		get => _f_name[Entity.Index];
		set => _f_name[Entity.Index] = value;
	}
	public System.Collections.Generic.List<BansheeGz.BGDatabase.BGEntity> f_slots
	{
		get => _f_slots[Entity.Index];
		set => _f_slots[Entity.Index] = value;
	}
	private static BansheeGz.BGDatabase.BGFieldEntityName _ufle12jhs77_f_name;
	public static BansheeGz.BGDatabase.BGFieldEntityName _f_name => _ufle12jhs77_f_name ?? (_ufle12jhs77_f_name = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldEntityName>(MetaDefault, new BGId(4806166443902158062UL, 4904745704777535889UL), () => _ufle12jhs77_f_name = null));
	private static BansheeGz.BGDatabase.BGFieldRelationMultiple _ufle12jhs77_f_slots;
	public static BansheeGz.BGDatabase.BGFieldRelationMultiple _f_slots => _ufle12jhs77_f_slots ?? (_ufle12jhs77_f_slots = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldRelationMultiple>(MetaDefault, new BGId(5760435788755281860UL, 17591114784162291892UL), () => _ufle12jhs77_f_slots = null));
	public List<BGEntity> RelatedCharacterListUsingequipmentRelation => BGCodeGenUtils.GetRelatedInbound<BansheeGz.BGDatabase.BGEntity>(Alias_rifegrt_Character._f_equipment, Entity.Id);
}
