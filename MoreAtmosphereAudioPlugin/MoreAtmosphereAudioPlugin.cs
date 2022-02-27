using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using Bounce.Unmanaged;
using HarmonyLib;
using LordAshes;
using UnityEngine;
using UnityEngine.Networking;
using MD5 = System.Security.Cryptography.MD5;

namespace MoreAtmosphereAudioPlugin
{

    [BepInPlugin(Guid, "HolloFoxes' More Atmosphere Audio Plugin", Version)]
    [BepInDependency(FileAccessPlugin.Guid)]
    public class MoreAtmosphereAudioPlugin : BaseUnityPlugin
    {
        static MoreAtmosphereAudioPlugin _singleton;

        // constants
        public const string Guid = "org.hollofox.plugins.AtmosphereAudio";
        internal const string Version = "1.0.0.0";

        internal static Dictionary<NGuid, (string, string)> Music
            = new Dictionary<NGuid, (string, string)>();

        internal static Dictionary<NGuid, (string, string)> Ambient
            = new Dictionary<NGuid, (string, string)>();

        private void LoadAudio(string SubFolder)
        {
            var files =
                FileAccessPlugin.File.Find($"CustomData\\Audio\\{SubFolder}")
                    .Where(f =>
                        f.EndsWith(".mp3") ||
                        f.EndsWith(".aif") ||
                        f.EndsWith(".wav") ||
                        f.EndsWith(".ogg")
                    );

            foreach (var file in files)
            {
                string songPath = 
                    file.StartsWith("https://") || file.StartsWith("http://") ? 
                        file : $"file:///{file}";
                var components = file.Split('/');
                var builder = new StringBuilder();
                for (int i = 0; i < 5; i++)
                {
                    builder.Append(components[components.Length - 1 - i]);
                }
                var id = GenerateID(builder.ToString());

                var name = components.Last();
                Debug.Log($"Added:{name}");
                name = name
                    .Replace(".mp3", "")
                    .Replace(".aif", "")
                    .Replace(".wav", "")
                    .Replace(".ogg", "");

                switch (SubFolder)
                {
                    case "Ambient":
                        Ambient.Add(id, (name, songPath));
                        break;
                    case "Music":
                        Music.Add(id, (name, songPath));
                        break;
                }
            }
        }

        /// <summary>
        /// Awake plugin
        /// </summary>
        void Awake()
        {
            Debug.Log("More Atmosphere Audio Plug-in loaded");
            ModdingUtils.Initialize(this, Logger);
            var harmony = new Harmony(Guid);
            harmony.PatchAll();
            LoadAudio("Ambient");
            LoadAudio("Music");
            _singleton = this;
        }


        public static void LoadAudioCallback(object[] args)
        {
            _singleton.StartCoroutine("LoadAudioFromSource", args);
        }

        public static NGuid GenerateID(string id)
        {
            return new NGuid(System.Guid.Parse(CreateMD5(id)));
        }

        private static string CreateMD5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                for (var i = 0; i < hashBytes.Length; i++) sb.Append(hashBytes[i].ToString("X2"));
                return sb.ToString();
            }
        }

        IEnumerator LoadAudioFromSource(object[] args)
        {
            var __instance = (AtmosphereManager.LoadedAudioClip)args[0];
            var ____clip = (AudioClip) args[1];
            var ClipLoaded = (System.Action<AtmosphereManager.LoadedAudioClip, AudioClip >) args[2];
            
            var source = Ambient.ContainsKey(__instance.GUID)
                ? Ambient[__instance.GUID].Item2
                : Music[__instance.GUID].Item2;

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(source, AudioType.UNKNOWN))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log("Audio Plugin: Failure To Load ...");
                    Debug.Log(www.error);
                }
                else
                {
                    ____clip = DownloadHandlerAudioClip.GetContent(www);
                }
            }
            ClipLoaded(__instance, ____clip);
        }

    }
}
