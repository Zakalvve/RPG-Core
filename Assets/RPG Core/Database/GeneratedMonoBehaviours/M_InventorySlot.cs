using System;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGDatabase;
using Alias_rifegrt_Inventory = M_Inventory;
using Alias_rifegrt_ActionBar = M_ActionBar;
using Alias_rifegrt_EquipmentSlot = M_EquipmentSlot;

//=============================================================
//||                   Generated by BansheeGz Code Generator ||
//=============================================================

[AddComponentMenu("BansheeGz/Generated/M_InventorySlot")]
public partial class M_InventorySlot : BGEntityGo
{
	public override BGMetaEntity MetaConstraint => MetaDefault;
	private static BansheeGz.BGDatabase.BGMetaRow _metaDefault;
	public static BansheeGz.BGDatabase.BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BansheeGz.BGDatabase.BGMetaRow>(new BGId(4991868874684857903UL,16415870564699247268UL), () => _metaDefault = null));
	public static BansheeGz.BGDatabase.BGRepoEvents Events => BGRepo.I.Events;
	public System.String f_name
	{
		get => _f_name[Entity.Index];
		set => _f_name[Entity.Index] = value;
	}
	public BansheeGz.BGDatabase.BGEntity f_Item
	{
		get => _f_Item[Entity.Index];
		set => _f_Item[Entity.Index] = value;
	}
	public System.Int32 f_StackSize
	{
		get => _f_StackSize[Entity.Index];
		set => _f_StackSize[Entity.Index] = value;
	}
	private static BansheeGz.BGDatabase.BGFieldEntityName _ufle12jhs77_f_name;
	public static BansheeGz.BGDatabase.BGFieldEntityName _f_name => _ufle12jhs77_f_name ?? (_ufle12jhs77_f_name = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldEntityName>(MetaDefault, new BGId(4927304061476817234UL, 12391829457485915278UL), () => _ufle12jhs77_f_name = null));
	private static BansheeGz.BGDatabase.BGFieldViewRelationSingle _ufle12jhs77_f_Item;
	public static BansheeGz.BGDatabase.BGFieldViewRelationSingle _f_Item => _ufle12jhs77_f_Item ?? (_ufle12jhs77_f_Item = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldViewRelationSingle>(MetaDefault, new BGId(4779014571829944867UL, 14176433916433299599UL), () => _ufle12jhs77_f_Item = null));
	private static BansheeGz.BGDatabase.BGFieldInt _ufle12jhs77_f_StackSize;
	public static BansheeGz.BGDatabase.BGFieldInt _f_StackSize => _ufle12jhs77_f_StackSize ?? (_ufle12jhs77_f_StackSize = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldInt>(MetaDefault, new BGId(4851780962771693790UL, 6167039357613567645UL), () => _ufle12jhs77_f_StackSize = null));
	public List<BGEntity> RelatedInventoryListUsingslotsRelation => BGCodeGenUtils.GetRelatedInbound<BansheeGz.BGDatabase.BGEntity>(Alias_rifegrt_Inventory._f_slots, Entity.Id);
	public List<BGEntity> RelatedActionBarListUsingslotsRelation => BGCodeGenUtils.GetRelatedInbound<BansheeGz.BGDatabase.BGEntity>(Alias_rifegrt_ActionBar._f_slots, Entity.Id);
	public List<BGEntity> RelatedEquipmentSlotListUsingslotRelation => BGCodeGenUtils.GetRelatedInbound<BansheeGz.BGDatabase.BGEntity>(Alias_rifegrt_EquipmentSlot._f_slot, Entity.Id);
}
