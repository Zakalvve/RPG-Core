using BansheeGz.BGDatabase;
using RPG.Core.Events;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoad : MonoBehaviour
{
    public bool HasSavedFile
    {
        get { return File.Exists(SavePathFile); }
    }

    public string SavePathFile
    {
        get { return Path.Combine(Application.persistentDataPath, "gameSaveExample.dat"); }
    }
    public void Save()
    {
        File.WriteAllBytes(SavePathFile, BGRepo.I.Addons.Get<BGAddonSaveLoad>().Save());
    }
    public void Load()
    {
        if (!HasSavedFile) return;
        EventEmitter.UnsubscribeFromAllGlobal();
        BGRepo.I.Addons.Get<BGAddonSaveLoad>().Load(File.ReadAllBytes(SavePathFile));
        SceneManager.LoadScene(E_Player.GetEntity(0).f_scene.Name);
    }
}
