using System;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGDatabase;
using Alias_rifegrt_InventorySlot = M_InventorySlot;

//=============================================================
//||                   Generated by BansheeGz Code Generator ||
//=============================================================

[AddComponentMenu("BansheeGz/Generated/M_Action")]
public partial class M_Action : BGEntityGo
{
	public override BGMetaEntity MetaConstraint => MetaDefault;
	private static BansheeGz.BGDatabase.BGMetaRow _metaDefault;
	public static BansheeGz.BGDatabase.BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BansheeGz.BGDatabase.BGMetaRow>(new BGId(5411308058772253045UL,9716562674314038973UL), () => _metaDefault = null));
	public static BansheeGz.BGDatabase.BGRepoEvents Events => BGRepo.I.Events;
	public System.String f_name
	{
		get => _f_name[Entity.Index];
		set => _f_name[Entity.Index] = value;
	}
	public UnityEngine.Sprite f_Icon => _f_Icon[Entity.Index];
	public System.String f_Description => _f_Description[Entity.Index];
	public System.Int32 f_MaxStackSize => _f_MaxStackSize[Entity.Index];
	public System.String f_Type => _f_Type[Entity.Index];
	public System.String f_onExecute
	{
		get => _f_onExecute[Entity.Index];
		set => _f_onExecute[Entity.Index] = value;
	}
	private static BansheeGz.BGDatabase.BGFieldEntityName _ufle12jhs77_f_name;
	public static BansheeGz.BGDatabase.BGFieldEntityName _f_name => _ufle12jhs77_f_name ?? (_ufle12jhs77_f_name = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldEntityName>(MetaDefault, new BGId(4872893849248249860UL, 654446770413383839UL), () => _ufle12jhs77_f_name = null));
	private static BansheeGz.BGDatabase.BGFieldUnitySprite _ufle12jhs77_f_Icon;
	public static BansheeGz.BGDatabase.BGFieldUnitySprite _f_Icon => _ufle12jhs77_f_Icon ?? (_ufle12jhs77_f_Icon = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldUnitySprite>(MetaDefault, new BGId(5343423378312266840UL, 7148083672561822394UL), () => _ufle12jhs77_f_Icon = null));
	private static BansheeGz.BGDatabase.BGFieldString _ufle12jhs77_f_Description;
	public static BansheeGz.BGDatabase.BGFieldString _f_Description => _ufle12jhs77_f_Description ?? (_ufle12jhs77_f_Description = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldString>(MetaDefault, new BGId(4793169888373995912UL, 10270556447637815723UL), () => _ufle12jhs77_f_Description = null));
	private static BansheeGz.BGDatabase.BGFieldInt _ufle12jhs77_f_MaxStackSize;
	public static BansheeGz.BGDatabase.BGFieldInt _f_MaxStackSize => _ufle12jhs77_f_MaxStackSize ?? (_ufle12jhs77_f_MaxStackSize = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldInt>(MetaDefault, new BGId(5265606520434578057UL, 12288896421061157513UL), () => _ufle12jhs77_f_MaxStackSize = null));
	private static BansheeGz.BGDatabase.BGFieldString _ufle12jhs77_f_Type;
	public static BansheeGz.BGDatabase.BGFieldString _f_Type => _ufle12jhs77_f_Type ?? (_ufle12jhs77_f_Type = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldString>(MetaDefault, new BGId(5477709714743376555UL, 8631031050135708342UL), () => _ufle12jhs77_f_Type = null));
	private static BansheeGz.BGDatabase.BGFieldString _ufle12jhs77_f_onExecute;
	public static BansheeGz.BGDatabase.BGFieldString _f_onExecute => _ufle12jhs77_f_onExecute ?? (_ufle12jhs77_f_onExecute = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldString>(MetaDefault, new BGId(4784952501581533248UL, 17680137530968275340UL), () => _ufle12jhs77_f_onExecute = null));
	public List<BGEntity> RelatedInventorySlotListUsingItemRelation => BGCodeGenUtils.GetRelatedInbound<BansheeGz.BGDatabase.BGEntity>(Alias_rifegrt_InventorySlot._f_Item, Entity.Id);
}
