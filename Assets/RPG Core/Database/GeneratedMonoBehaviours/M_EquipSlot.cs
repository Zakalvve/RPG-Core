using System;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGDatabase;

//=============================================================
//||                   Generated by BansheeGz Code Generator ||
//=============================================================

[AddComponentMenu("BansheeGz/Generated/M_EquipSlot")]
public partial class M_EquipSlot : BGEntityGo
{
	public override BGMetaEntity MetaConstraint => MetaDefault;
	private static BansheeGz.BGDatabase.BGMetaNested _metaDefault;
	public static BansheeGz.BGDatabase.BGMetaNested MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BansheeGz.BGDatabase.BGMetaNested>(new BGId(4819587845560330406UL,50932076330191794UL), () => _metaDefault = null));
	public static BansheeGz.BGDatabase.BGRepoEvents Events => BGRepo.I.Events;
	public System.String f_name
	{
		get => _f_name[Entity.Index];
		set => _f_name[Entity.Index] = value;
	}
	public BansheeGz.BGDatabase.BGEntity f_EquipmentInventory
	{
		get => _f_EquipmentInventory[Entity.Index];
		set => _f_EquipmentInventory[Entity.Index] = value;
	}
	public BansheeGz.BGDatabase.BGEntity f_slot
	{
		get => _f_slot[Entity.Index];
		set => _f_slot[Entity.Index] = value;
	}
	public RPGCore.Item.EquipmentTypes f_ValidSlotType
	{
		get => (RPGCore.Item.EquipmentTypes) _f_ValidSlotType.GetStoredValue(Entity.Index);
		set => _f_ValidSlotType.SetStoredValue(Entity.Index, (System.Int32) value);
	}
	private static BansheeGz.BGDatabase.BGFieldEntityName _ufle12jhs77_f_name;
	public static BansheeGz.BGDatabase.BGFieldEntityName _f_name => _ufle12jhs77_f_name ?? (_ufle12jhs77_f_name = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldEntityName>(MetaDefault, new BGId(5026900740402441295UL, 14126008549625490358UL), () => _ufle12jhs77_f_name = null));
	private static BansheeGz.BGDatabase.BGFieldRelationSingle _ufle12jhs77_f_EquipmentInventory;
	public static BansheeGz.BGDatabase.BGFieldRelationSingle _f_EquipmentInventory => _ufle12jhs77_f_EquipmentInventory ?? (_ufle12jhs77_f_EquipmentInventory = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldRelationSingle>(MetaDefault, new BGId(5462051136185707612UL, 6538540710858993839UL), () => _ufle12jhs77_f_EquipmentInventory = null));
	private static BansheeGz.BGDatabase.BGFieldRelationSingle _ufle12jhs77_f_slot;
	public static BansheeGz.BGDatabase.BGFieldRelationSingle _f_slot => _ufle12jhs77_f_slot ?? (_ufle12jhs77_f_slot = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldRelationSingle>(MetaDefault, new BGId(4956737682476071014UL, 10310517842298809504UL), () => _ufle12jhs77_f_slot = null));
	private static BansheeGz.BGDatabase.BGFieldEnum _ufle12jhs77_f_ValidSlotType;
	public static BansheeGz.BGDatabase.BGFieldEnum _f_ValidSlotType => _ufle12jhs77_f_ValidSlotType ?? (_ufle12jhs77_f_ValidSlotType = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldEnum>(MetaDefault, new BGId(5208802550666458663UL, 4196554463948383897UL), () => _ufle12jhs77_f_ValidSlotType = null));
}
