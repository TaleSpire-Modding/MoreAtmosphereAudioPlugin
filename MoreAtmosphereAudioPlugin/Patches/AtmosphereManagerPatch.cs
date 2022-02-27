using System.Collections.Generic;
using Bounce.BlobAssets;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using HarmonyLib;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace MoreAtmosphereAudioPlugin.Patches
{
    [HarmonyPatch(typeof(AssetDb), "OnSetupInternals")]
    public class AtmosphereManagerPatch
    {
        static void Postfix()
        {
            foreach (var file in MoreAtmosphereAudioPlugin.Ambient)
            {
                if (AssetDb.Music.TryGetValue(file.Key, out _)) continue;
                var builder = new BlobBuilder(Allocator.Persistent);
                builder.ConstructRoot<MusicData>();
                var x = builder.CreateBlobAssetReference<MusicData>(Allocator.Persistent);
                x.Value = new MusicData();
                MusicData.Construct(builder,ref x.Value,file.Key,file.Key,file.Value.Item1,file.Value.Item1,
                    new string[]{},"", file.Value.Item1, MusicData.MusicKind.Ambient);
                                      
                Debug.Log($"{MusicData.MusicKind.Ambient} Added:" + AssetDb.Music.TryAdd(file.Key, x.TakeView()));
            }

            foreach (var file in MoreAtmosphereAudioPlugin.Music)
            {
                if (AssetDb.Music.TryGetValue(file.Key, out _)) continue;
                var builder = new BlobBuilder(Allocator.Persistent);
                builder.ConstructRoot<MusicData>();
                var x = builder.CreateBlobAssetReference<MusicData>(Allocator.Persistent);
                x.Value = new MusicData();
                MusicData.Construct(builder, ref x.Value, file.Key, file.Key, file.Value.Item1, file.Value.Item1,
                    new string[] { }, "", file.Value.Item1, MusicData.MusicKind.Music);
                Debug.Log($"{MusicData.MusicKind.Music} Added:" + AssetDb.Music.TryAdd(file.Key, x.TakeView()));
            }
        }
    }

    [HarmonyPatch(typeof(AtmosphereManager.LoadedAudioClip), "Load")]
    public class LoadedAudioClipLoadPatch
    {
        static bool Prefix(System.Action<AtmosphereManager.LoadedAudioClip, AudioClip> ClipLoaded,
            ref AudioClip ____clip,
            ref AtmosphereManager.LoadedAudioClip __instance
        )
        {
            if (MoreAtmosphereAudioPlugin.Ambient.ContainsKey(__instance.GUID) ||
                MoreAtmosphereAudioPlugin.Music.ContainsKey(__instance.GUID))
            {
                if (____clip == null)
                {
                    MoreAtmosphereAudioPlugin.LoadAudioCallback(
                        new object[]
                        {
                            __instance,
                            ____clip,
                            ClipLoaded
                        }
                        );
                    return false;
                }
            }
            return true;
            
        }
    }

    
    [HarmonyPatch(typeof(UI_MusicSelectionDropdown), "OnEnable")]
    public class UI_MusicSelectionDropdownPatch
    {
        static void Postfix(
            ref TMP_Dropdown ____dropdown,
            ref List<NGuid> ____guidList,
            ref List<string> ____options,
            ref MusicData.MusicKind ____type
        )
        {
            var files = MoreAtmosphereAudioPlugin.Ambient;
            if (____type == MusicData.MusicKind.Music)
                files = MoreAtmosphereAudioPlugin.Music;
            foreach (var file in files)
            {
                var index = ____guidList.IndexOf(file.Key);
                if (index > -1) ____options[index+1] = file.Value.Item1;
            }
            ____dropdown.options.Clear();
            ____dropdown.AddOptions(____options);
        }
    }
}
