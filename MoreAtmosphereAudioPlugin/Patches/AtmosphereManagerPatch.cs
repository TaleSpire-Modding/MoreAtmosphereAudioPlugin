using Bounce.BlobAssets;
using Bounce.TaleSpire.AssetManagement;
using HarmonyLib;
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
                ref var root = ref builder.ConstructRoot<MusicData>();
                MusicData.Construct(builder,ref root,file.Key,file.Key,file.Value.Item1,file.Value.Item1,
                    new string[]{},"", file.Value.Item1, MusicData.MusicKind.Ambient);
                var x = builder.CreateBlobAssetReference<MusicData>(Allocator.Persistent);
                Debug.Log($"{MusicData.MusicKind.Ambient} Added {file.Value.Item1}:" + AssetDb.Music.TryAdd(file.Key, x.TakeView()));
            }

            foreach (var file in MoreAtmosphereAudioPlugin.Music)
            {
                if (AssetDb.Music.TryGetValue(file.Key, out _)) continue;
                var builder = new BlobBuilder(Allocator.Persistent);
                ref var root = ref builder.ConstructRoot<MusicData>();
                MusicData.Construct(builder, ref root, file.Key, file.Key, file.Value.Item1, file.Value.Item1,
                    new string[] { }, "", file.Value.Item1, MusicData.MusicKind.Music);
                var x = builder.CreateBlobAssetReference<MusicData>(Allocator.Persistent);
                Debug.Log($"{MusicData.MusicKind.Music} Added {file.Value.Item1}:" + AssetDb.Music.TryAdd(file.Key, x.TakeView()));
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
            if (!MoreAtmosphereAudioPlugin.Ambient.ContainsKey(__instance.GUID) &&
                !MoreAtmosphereAudioPlugin.Music.ContainsKey(__instance.GUID)) return true;
            if (____clip != null) return true;
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
}
