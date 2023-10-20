using System;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGDatabase;

//=============================================================
//||                   Generated by BansheeGz Code Generator ||
//=============================================================

[AddComponentMenu("BansheeGz/Generated/M_Player")]
public partial class M_Player : BGEntityGo
{
	public override BGMetaEntity MetaConstraint => MetaDefault;
	private static BansheeGz.BGDatabase.BGMetaRow _metaDefault;
	public static BansheeGz.BGDatabase.BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BansheeGz.BGDatabase.BGMetaRow>(new BGId(5685567762489749590UL,6315470072465355455UL), () => _metaDefault = null));
	public static BansheeGz.BGDatabase.BGRepoEvents Events => BGRepo.I.Events;
	public System.String f_name
	{
		get => _f_name[Entity.Index];
		set => _f_name[Entity.Index] = value;
	}
	public BansheeGz.BGDatabase.BGEntity f_scene
	{
		get => _f_scene[Entity.Index];
		set => _f_scene[Entity.Index] = value;
	}
	public BansheeGz.BGDatabase.BGEntity f_camera
	{
		get => _f_camera[Entity.Index];
		set => _f_camera[Entity.Index] = value;
	}
	public UnityEngine.Vector3 f_position
	{
		get => _f_position[Entity.Index];
		set => _f_position[Entity.Index] = value;
	}
	public BansheeGz.BGDatabase.BGEntity f_ownCharacter
	{
		get => _f_ownCharacter[Entity.Index];
		set => _f_ownCharacter[Entity.Index] = value;
	}
	private static BansheeGz.BGDatabase.BGFieldEntityName _ufle12jhs77_f_name;
	public static BansheeGz.BGDatabase.BGFieldEntityName _f_name => _ufle12jhs77_f_name ?? (_ufle12jhs77_f_name = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldEntityName>(MetaDefault, new BGId(5387341737950914718UL, 13949678309022716574UL), () => _ufle12jhs77_f_name = null));
	private static BansheeGz.BGDatabase.BGFieldRelationSingle _ufle12jhs77_f_scene;
	public static BansheeGz.BGDatabase.BGFieldRelationSingle _f_scene => _ufle12jhs77_f_scene ?? (_ufle12jhs77_f_scene = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldRelationSingle>(MetaDefault, new BGId(5370908962325218532UL, 2190189887116766639UL), () => _ufle12jhs77_f_scene = null));
	private static BansheeGz.BGDatabase.BGFieldRelationSingle _ufle12jhs77_f_camera;
	public static BansheeGz.BGDatabase.BGFieldRelationSingle _f_camera => _ufle12jhs77_f_camera ?? (_ufle12jhs77_f_camera = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldRelationSingle>(MetaDefault, new BGId(4856665909881205659UL, 8813011512213028764UL), () => _ufle12jhs77_f_camera = null));
	private static BansheeGz.BGDatabase.BGFieldVector3 _ufle12jhs77_f_position;
	public static BansheeGz.BGDatabase.BGFieldVector3 _f_position => _ufle12jhs77_f_position ?? (_ufle12jhs77_f_position = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldVector3>(MetaDefault, new BGId(4742175626008293568UL, 7870218691469650093UL), () => _ufle12jhs77_f_position = null));
	private static BansheeGz.BGDatabase.BGFieldRelationSingle _ufle12jhs77_f_ownCharacter;
	public static BansheeGz.BGDatabase.BGFieldRelationSingle _f_ownCharacter => _ufle12jhs77_f_ownCharacter ?? (_ufle12jhs77_f_ownCharacter = BGCodeGenUtils.GetField<BansheeGz.BGDatabase.BGFieldRelationSingle>(MetaDefault, new BGId(5748150778135015119UL, 8317494869552672422UL), () => _ufle12jhs77_f_ownCharacter = null));
}