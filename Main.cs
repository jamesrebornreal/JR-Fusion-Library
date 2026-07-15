using BoneLib;
using FusionLibrary;
using HarmonyLib;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Bonelab.SaveData;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.PuppetMasta;
using Il2CppSLZ.Marrow.SaveData;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.VFX;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSystem.Runtime.Remoting.Messaging;
using Il2CppSystem.Text.RegularExpressions;
using LabFusion.Data;
using LabFusion.Downloading;
using LabFusion.Downloading.ModIO;
using LabFusion.Entities;
using LabFusion.Marrow;
using LabFusion.Marrow.Pool;
using LabFusion.MonoBehaviours;
using LabFusion.Network;
using LabFusion.Player;
using LabFusion.Representation;
using LabFusion.RPC;
using LabFusion.Safety;
using LabFusion.Scene;
using LabFusion.SDK.Metadata;
using LabFusion.SDK.Points;
using LabFusion.Senders;
using LabFusion.UI.Popups;
using LabFusion.Utilities;
using MelonLoader;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;
using UnityEngine.Networking;
using static LabFusion.RPC.NetworkAssetSpawner;
[assembly: MelonInfo(typeof(FusionLibrary.LibraryCode), JRLibraryInfo.LibraryName, JRLibraryInfo.Version, JRLibraryInfo.LibraryCreator)]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]
namespace FusionLibrary
{
    internal static class JRLibraryInfo
    {

        public const string LibraryName = "JR Fusion Library";
        public const string LibraryCreator = "James Reborn";
        public const string Version = "1.0.0";

    }
    public enum handnow
    {
        Left,
        Right
    }
    public enum WhichHand
    {
        Both,
        Left,
        Right
    }
    
    //method extensions to help enhance code mods and make stuff more streamlined
    public static class GameObjectExtensions
        {
            public static void DestroyNow(this GameObject target) => GameObject.DestroyImmediate(target, true);
            public static void ToggleActive(this GameObject target) => target?.SetActive(!target.activeSelf);
            public static bool IsTooClose(this GameObject target, Transform closetoobject, float meters)
            {
                if (target == null)
                    return false;


                if (closetoobject == null)
                    return false;

                float distance = Vector3.Distance(closetoobject.position, target.transform.position);

                return distance < meters;
            }
            public static AIBrain JR_GetNPCAIBrain(this GameObject entity) => entity?.GetComponentInChildren<AIBrain>();
            public static bool JR_IsNPC(this GameObject entity) => entity?.GetComponentInChildren<AgentLinkControl>() != null || entity?.GetComponentInChildren<AIBrain>() != null;
            public static bool JR_HasPoolee(this GameObject entity) => entity?.GetComponentInChildren<Poolee>() != null;
            public static Poolee JR_GetPoolee(this GameObject entity) => entity?.GetComponentInChildren<Poolee>();
            public static bool JR_IsWeapon(this GameObject entity) => entity?.GetComponentInChildren<Gun>() != null || entity?.GetComponentInChildren<Gun>() != null;
            public static bool JR_IsMelee(this GameObject entity) => entity?.GetComponentInChildren<StabSlash>() != null || entity?.GetComponentInChildren<StabSlash>() != null;

        }
    public static class HandExtensions
    {
        public static SpawnGun JR_HandGrabbedSpawnGun(this Hand Handnow) => Handnow?.JR_GetAttachedObject()?.transform?.root?.GetComponentInChildren<SpawnGun>();
        public static bool JR_IsGrabbedSpawnGun(this Hand Handnow) => Handnow?.JR_GetAttachedObject()?.transform?.root?.GetComponentInChildren<SpawnGun>() != null;
        public static FlyingGun JR_HandGrabbedNimbusGun(this Hand Handnow) => Handnow?.JR_GetAttachedObject()?.transform?.root?.GetComponent<FlyingGun>();
        public static bool JR_IsGrabbedNimbusGun(this Hand Handnow) => Handnow?.JR_GetAttachedObject()?.transform?.root?.GetComponent<FlyingGun>() != null;
        public static AIBrain JR_IsHandGrabbedNPCAIBrain(this Hand Handnow) => Handnow?.JR_GetAttachedObject()?.transform?.root?.GetComponent<AIBrain>();
        public static bool JR_IsHandGrabbingNPC(this Hand Handnow) => Handnow?.JR_GetAttachedObject()?.transform?.root?.GetComponent<AIBrain>() != null;
        public static bool JR_IsHandGrabbingAnyNetPlayer(this Hand Handnow) => Handnow?.JR_GetAttachedObject()?.transform?.root?.GetComponent<RigManager>() != null;
        public static bool JR_IsHandGrabbingNetPlayer(this Hand Handnow, NetworkPlayer PlayerNow) => Handnow?.JR_GetAttachedObject()?.transform?.root?.GetComponent<RigManager>() == PlayerNow?.RigRefs?.RigManager;
        public static bool JR_IsHandGrabbingYou(this Hand Handnow) => Handnow?.JR_GetAttachedObject()?.transform?.root?.GetComponent<RigManager>() == Player.RigManager;
        public static GameObject JR_GetAttachedObject(this Hand Handnow) => Handnow?.m_CurrentAttachedGO;
        public static MarrowEntity JR_GetMarrowEntity(this Hand Handnow) => Handnow?.JR_GetAttachedObject()?.transform?.root?.GetComponent<MarrowEntity>();
        public static Gun JR_GetGun(this Hand Handnow) => Handnow?.JR_GetAttachedObject()?.transform?.root?.GetComponent<Gun>();
        public static bool JR_HasGun(this Hand Handnow) => Handnow?.JR_GetAttachedObject()?.transform?.root?.GetComponent<Gun>() != null;
        public static StabSlash JR_GetMelee(this Hand Handnow) => Handnow?.JR_GetAttachedObject()?.transform?.root?.GetComponent<StabSlash>();
        public static bool JR_HasMelee(this Hand Handnow) => Handnow?.JR_GetAttachedObject()?.transform?.root?.GetComponent<StabSlash>() != null;

    }
    public static class NetworkEntityExtensions
        {

            public static void JR_Despawn(this NetworkEntity entity) => LibraryCode.DespawnNow(entity);
            public static bool JR_IsNetPlayer(this NetworkEntity entity) => entity?.JR_GetMarrowEntity() != null && (entity.JR_GetMarrowEntity()?.JR_IsNetPlayer() ?? false);
            public static bool JR_IsMagazine(this NetworkEntity entity)
            {
                if (entity == null)
                    return false;

                var marrow = entity.JR_GetMarrowEntity();
                if (marrow == null)
                    return false;

                var go = marrow.gameObject;
                if (go == null)
                    return false;

                Magazine mag = null;
                try
                {
                    mag = go.GetComponent<Magazine>();
                }
                catch
                {
                    return false;
                }

                if (mag == null)
                    return false;

                bool isGun = false;
                try
                {
                    isGun = entity.JR_IsGun();
                }
                catch
                {
                    isGun = false;
                }

                return !isGun;
            }
            public static bool JR_IsGun(this NetworkEntity entity) => entity?.JR_GetMarrowEntity()?.gameObject?.GetComponent<Gun>() != null;
            public static bool JR_IsMelee(this NetworkEntity entity) => entity?.JR_GetMarrowEntity()?.gameObject?.GetComponent<StabSlash>() != null;
            public static bool JR_IsNPC(this NetworkEntity entity) => entity?.JR_GetMarrowEntity()?.gameObject?.GetComponent<AIBrain>() != null;
            public static Gun JR_GetGun(this NetworkEntity entity) => entity?.JR_GetMarrowEntity()?.gameObject?.GetComponent<Gun>();
            public static StabSlash JR_GetMelee(this NetworkEntity entity) => entity?.JR_GetMarrowEntity()?.gameObject?.GetComponent<StabSlash>();
            public static AIBrain JR_GetNPCAIBrain(this NetworkEntity entity) => entity?.JR_GetMarrowEntity()?.gameObject?.GetComponent<AIBrain>();
            public static MarrowEntity JR_GetMarrowEntity(this NetworkEntity entity) => entity?.GetExtender<IMarrowEntityExtender>()?.MarrowEntity;
        }
    public static class MarrowEntityExtensions
        {
            public static AIBrain JR_GetNPCAIBrain(this MarrowEntity entity) => entity?.gameObject?.GetComponent<AIBrain>();
            public static string JR_GetBarcodeID(this MarrowEntity entity) => entity?._poolee?._SpawnableCrate_k__BackingField?.Barcode?.ID ?? "NULL";
            public static bool JR_IsNetPlayer(this MarrowEntity entity) => entity?.gameObject?.transform?.root?.GetComponent<AntiHasher>() != null;
        }
    public static class NetworkPlayerExtensions
        {
            public static bool IsMe(this NetworkPlayer player)
            {
                return player.JR_SteamID() == LibraryCode.SteamIdYours();
            }
            public static PullCordDevice JR_PlayersBodyLog(this NetworkPlayer player)
            {
                Transform? right = player?.JR_PlayersPhysicsRig()?.m_elbowRt?.Find("BodyLogSlot/BodyLog");
                if (right != null)
                {
                    var device = right.GetComponent<PullCordDevice>();
                    if (device != null)
                        return device;
                }

                Transform? left = player?.JR_PlayersPhysicsRig()?.m_elbowLf?.Find("BodyLogSlot/BodyLog");
                if (left != null)
                {
                    var device = left.GetComponent<PullCordDevice>();
                    if (device != null)
                        return device;
                }

                return null;
            }
            public static bool JR_IsGrabbingNPC(this NetworkPlayer player, WhichHand hand)
            {
                return hand switch
                {
                    WhichHand.Left => player?.JR_GetObjectInHand(WhichHand.Left)?.transform?.root?.GetComponent<AIBrain>() != null,
                    WhichHand.Right => player?.JR_GetObjectInHand(WhichHand.Right)?.transform?.root?.GetComponent<AIBrain>() != null,
                    WhichHand.Both =>
                        player?.JR_GetObjectInHand(WhichHand.Left)?.transform?.root?.GetComponent<AIBrain>() != null
                        || player?.JR_GetObjectInHand(WhichHand.Right)?.transform?.root?.GetComponent<AIBrain>() != null,
                    _ => false
                };
            }
            public static PuppetMaster JR_GetGrabbedPuppetMaster(this NetworkPlayer player, WhichHand hand) => player?.JR_GetMarrowEntityInHand(hand)?.JR_GetNPCAIBrain()?.puppetMaster;
            public static bool JR_IsGrabbingSelf(this NetworkPlayer player, WhichHand Hand)
            {
                return Hand switch
                {
                    WhichHand.Left => player?.JR_GetObjectInHand(WhichHand.Left)?.transform?.root?.GetComponent<RigManager>() == player?.RigRefs?.RigManager,
                    WhichHand.Right => player?.JR_GetObjectInHand(WhichHand.Right)?.transform?.root?.GetComponent<RigManager>() == player?.RigRefs?.RigManager,
                    WhichHand.Both =>
                        player?.JR_GetObjectInHand(WhichHand.Left)?.transform?.root?.GetComponent<RigManager>() == player?.RigRefs?.RigManager
                        || player?.JR_GetObjectInHand(WhichHand.Right)?.transform?.root?.GetComponent<RigManager>() == player?.RigRefs?.RigManager,
                    _ => false
                };
            }
            public static bool JR_IsGrabbingAnyThing(this NetworkPlayer player, WhichHand Hand)
            {
                return Hand switch
                {
                    WhichHand.Left => player?.JR_GetObjectInHand(WhichHand.Left) != null,
                    WhichHand.Right => player?.JR_GetObjectInHand(WhichHand.Right) != null,
                    WhichHand.Both =>
                        player?.JR_GetObjectInHand(WhichHand.Left) != null
                        || player?.JR_GetObjectInHand(WhichHand.Right) != null,
                    _ => false
                };
            }
            public static MarrowEntity JR_GetMarrowEntityInHand(this NetworkPlayer player, WhichHand Hand)
            {
                return Hand switch
                {
                    WhichHand.Left => player?.JR_GetObjectInHand(WhichHand.Left)?.gameObject?.transform?.root?.gameObject?.GetComponent<MarrowEntity>(),
                    WhichHand.Right => player?.JR_GetObjectInHand(WhichHand.Right)?.gameObject?.transform?.root?.gameObject?.GetComponent<MarrowEntity>(),
                    _ => null
                };
            }
            public static GameObject JR_GetObjectInHand(this NetworkPlayer player, WhichHand Hand)
            {
                return Hand switch
                {
                    WhichHand.Left => player?.JR_GetHand(WhichHand.Left)?.m_CurrentAttachedGO,
                    WhichHand.Right => player?.JR_GetHand(WhichHand.Right)?.m_CurrentAttachedGO,
                    _ => null
                };
            }
            public static NetworkPlayer JR_GrabbedPlayer(this NetworkPlayer player, WhichHand hand)
            {
                var handObj = hand switch
                {
                    WhichHand.Left => player?.JR_GetHand(WhichHand.Left),
                    WhichHand.Right => player?.JR_GetHand(WhichHand.Right),
                    _ => null
                };

                if (handObj?.m_CurrentAttachedGO == null)
                    return null;

                var grabbedRig = handObj.m_CurrentAttachedGO.transform?.root?.GetComponent<RigManager>();
                if (grabbedRig == null)
                    return null;

                return NetworkPlayerManager.TryGetPlayer(grabbedRig, out var networkPlayer) ? networkPlayer : null;
            }
            public static bool JR_IsGrabbingAnyNetPlayer(this NetworkPlayer player, WhichHand Hand)
            {
                return Hand switch
                {
                    WhichHand.Left => player?.JR_GetObjectInHand(WhichHand.Left)?.transform.root.GetComponent<RigManager>() != null,
                    WhichHand.Right => player?.JR_GetObjectInHand(WhichHand.Right)?.transform.root.GetComponent<RigManager>() != null,
                    WhichHand.Both =>
                        player?.JR_GetObjectInHand(WhichHand.Left)?.transform.root.GetComponent<RigManager>() != null
                        || player?.JR_GetObjectInHand(WhichHand.Right)?.transform.root.GetComponent<RigManager>() != null,
                    _ => false
                };
            }
            public static bool JR_IsGrabbingYou(this NetworkPlayer player, WhichHand hand)
            {
                if (player?.JR_PlayersPhysicsRig() == null || Player.RigManager == null)
                    return false;

                return hand switch
                {
                    WhichHand.Left => player?.JR_GetObjectInHand(WhichHand.Left)?.transform?.root?.GetComponent<RigManager>() == Player.RigManager,
                    WhichHand.Right => player?.JR_GetObjectInHand(WhichHand.Right)?.transform?.root?.GetComponent<RigManager>() == Player.RigManager,
                    WhichHand.Both =>
                        (player?.JR_GetObjectInHand(WhichHand.Left)?.transform?.root?.GetComponent<RigManager>() == Player.RigManager) ||
                        (player?.JR_GetObjectInHand(WhichHand.Right)?.transform?.root?.GetComponent<RigManager>() == Player.RigManager),
                    _ => false
                };
            }
            public static Transform JR_PlayersHead(this NetworkPlayer player) => player?.JR_PlayersPhysicsRig()?.m_head;
            public static PhysicsRig JR_PlayersPhysicsRig(this NetworkPlayer player) => player?.RigRefs?.RigManager?.physicsRig;
            public static SerializedAvatarStats JR_SerializedAvatarStats(this NetworkPlayer player)
            {
                var avatar = player?.RigRefs?.RigManager?.avatar;
                if (avatar == null)
                    return null;

                return new SerializedAvatarStats(avatar);
            }
            public static string JR_PlayersAvatarBarcodeID(this NetworkPlayer player) => player?.RigRefs?.RigManager?.AvatarCrate?.Barcode?.ID ?? "NULL";
            public static byte JR_SmallID(this NetworkPlayer player) => (byte)(player?.PlayerID?.SmallID ?? -1);
            public static ulong JR_SteamID(this NetworkPlayer player) => player?.PlayerID?.PlatformID ?? 0;
            public static int JR_AvatarMODIOID(this NetworkPlayer player) => player?.PlayerID?.Metadata?.AvatarModID?.GetValue() ?? 0;
            public static string JR_Username(this NetworkPlayer player) => player?.PlayerID?.Metadata?.Username?.GetValue() ?? "NULL";
            public static string JR_Nickname(this NetworkPlayer player) => player?.PlayerID?.Metadata?.Nickname?.GetValue() ?? "NULL";
            public static string JR_Description(this NetworkPlayer player) => player?.PlayerID?.Metadata?.Description?.GetValue() ?? "NULL";
            public static string JR_PermissionLevel(this NetworkPlayer player) => player?.PlayerID?.Metadata?.PermissionLevel?.GetValue() ?? "NULL";
            public static Hand JR_GetHand(this NetworkPlayer player, WhichHand hand)
            {
                if (player?.JR_PlayersPhysicsRig() == null)
                    return null;

                return hand switch
                {
                    WhichHand.Left => player.JR_PlayersPhysicsRig().leftHand,
                    WhichHand.Right => player.JR_PlayersPhysicsRig().rightHand,
                    _ => null
                };
            }
            public static bool JR_IsHoldingBarcode(this NetworkPlayer player, WhichHand hand, string barcode)
            {
                if (player?.JR_PlayersPhysicsRig() == null)
                    return false;

                return hand switch
                {
                    WhichHand.Left => player?.JR_GetObjectInHand(WhichHand.Left)?.transform?.root?.GetComponent<MarrowEntity>()?.JR_GetBarcodeID() == barcode,
                    WhichHand.Right => player?.JR_GetObjectInHand(WhichHand.Right)?.transform?.root?.GetComponent<MarrowEntity>()?.JR_GetBarcodeID() == barcode,
                    WhichHand.Both =>
                        (player?.JR_GetObjectInHand(WhichHand.Left)?.transform?.root?.GetComponent<MarrowEntity>()?.JR_GetBarcodeID() == barcode) ||
                        (player?.JR_GetObjectInHand(WhichHand.Right)?.transform?.root?.GetComponent<MarrowEntity>()?.JR_GetBarcodeID() == barcode),
                    _ => false
                };
            }
            //clear constraints on a player (owner only locked to prevent abuse)
            public static void ClearConstraints(this NetworkPlayer Netty)
        {
            if (LibraryCode.AreYouOWNER())
            {
                if (!Netty.PlayerID.IsValid)
                {
                    return;
                }

                try
                {
                    foreach (ConstraintTracker componentsInChild in Netty.RigRefs.RigManager.physicsRig.GetComponentsInChildren<ConstraintTracker>())
                    {
                        componentsInChild.DeleteConstraint();
                    }
                }
                catch
                {

                }
            }
        }
            //hide holsters for specific players
            public static void HolsterHiderAll(this NetworkPlayer playerTodo, bool activeNow = false)
        {
            var rig = playerTodo != null
                ? playerTodo.RigRefs?.RigManager?.physicsRig
                : Player.RigManager?.physicsRig;

            if (rig == null) return;

            void Toggle(Transform root, string path)
            {
                if (root == null) return;
                var t = root.Find(path);
                if (t == null) return;
                var mr = t.GetComponent<UnityEngine.MeshRenderer>();
                mr?.gameObject.SetActive(activeNow);
            }

            Toggle(rig.m_spine?.transform, "SideRt/prop_handGunHolster/strap_geo");
            Toggle(rig.m_spine?.transform, "SideRt/prop_handGunHolster/handgunHolster_geo");
            Toggle(rig.m_spine?.transform, "SideLf/prop_handGunHolster/strap_geo");
            Toggle(rig.m_spine?.transform, "SideLf/prop_handGunHolster/handgunHolster_geo");
            Toggle(rig.m_pelvis?.transform, "BeltLf1/InventoryAmmoReceiver/Holder");
            Toggle(rig.m_pelvis?.transform, "BeltRt1/InventoryAmmoReceiver/Holder");
            Toggle(rig.m_pelvis?.transform, "BackCt/prop_pouch");
        }




    }
    public static class BarCodeIDExtensions
    {
        public static string JR_BarcodeCrateName(this string idnow)
        {
            if (LibraryCode.IsAvatarCrateExist(idnow))
                return LibraryCode.StripColorTags(new AvatarCrateReference(idnow)?.Crate?.name);

            if (LibraryCode.IsLevelCrateExist(idnow))
                return LibraryCode.StripColorTags(new LevelCrateReference(idnow)?.Crate?.name);

            if (LibraryCode.IsSpawnableCrateExist(idnow))
                return LibraryCode.StripColorTags(new SpawnableCrateReference(idnow)?.Crate?.name);

            return "NULL";
        }

        public static string JR_BarcodePalletName(this string idnow)
        {
            if (LibraryCode.IsAvatarCrateExist(idnow))
                return LibraryCode.StripColorTags(new AvatarCrateReference(idnow)?.Crate?.Pallet?.name);

            if (LibraryCode.IsLevelCrateExist(idnow))
                return LibraryCode.StripColorTags(new LevelCrateReference(idnow)?.Crate?.Pallet?.name);

            if (LibraryCode.IsSpawnableCrateExist(idnow))
                return LibraryCode.StripColorTags(new SpawnableCrateReference(idnow)?.Crate?.Pallet?.name);

            return "NULL";
        }

        public static string JR_BarcodeAuthor(this string idnow)
        {
            if (LibraryCode.IsAvatarCrateExist(idnow))
                return LibraryCode.StripColorTags(new AvatarCrateReference(idnow)?.Crate?.Pallet?.Author);

            if (LibraryCode.IsLevelCrateExist(idnow))
                return LibraryCode.StripColorTags(new LevelCrateReference(idnow)?.Crate?.Pallet?.Author);

            if (LibraryCode.IsSpawnableCrateExist(idnow))
                return LibraryCode.StripColorTags(new SpawnableCrateReference(idnow)?.Crate?.Pallet?.Author);

            return "NULL";
        }


    }
    //

    //able to create individual timer objects 
    public class SimpleTimer
    {
        public System.Action? _codenow;
        public float _mins;
        public object? _coroutine;

        private bool _quicker;
        private float _quickerSeconds;

        private bool _running;

        public SimpleTimer(System.Action codenow, float mins)
        {
            _codenow = codenow ?? throw new System.ArgumentNullException(nameof(codenow));
            _mins = mins;
        }

        public SimpleTimer Start(bool quicker = false, float quickerseconds = 10f)
        {
            Stop();

            _quicker = quicker;
            _quickerSeconds = quickerseconds;

            _running = true;
            _coroutine = MelonCoroutines.Start(RunEveryXMins());

            return this;
        }

        public void Stop()
        {
            _running = false;

            if (_coroutine != null)
            {
                try
                {
                    MelonCoroutines.Stop(_coroutine);
                }
                catch { }

                _coroutine = null;
            }
        }

        public void Refresh(System.Action? newAction = null, float? newMins = null)
        {
            Stop();

            if (newAction != null)
                _codenow = newAction;

            if (newMins.HasValue)
                _mins = newMins.Value;

            Start(_quicker, _quickerSeconds);

            MelonLogger.Warning("Timer refreshed, first execution will happen after interval.");
        }

        public System.Collections.IEnumerator RunEveryXMins()
        {
            while (_running)
            {
                float waitTime = _quicker ? _quickerSeconds : _mins * 60f;

                yield return new WaitForSecondsRealtime(waitTime);

                if (!_running)
                    yield break;

                try
                {
                    var action = _codenow;

                    if (action == null)
                        continue;

                    action.Invoke();
                }
                catch (System.NullReferenceException)
                {
                    MelonLogger.Warning("Timer skipped null Unity reference (object destroyed).");
                }
                catch (System.Exception ex)
                {
                    MelonLogger.Error($"Timer error: {ex}");
                }
            }
        }
    }


    public class LibraryCode : MelonMod
    {
        //returns the text currently on your clipboard
        public static string ClipBoardText() { 
            return GUIUtility.systemCopyBuffer;
        }
        //returns active hand
        public static Hand JR_YourGetHand(WhichHand hand)
        {
            var rig = Player.RigManager?.physicsRig;
            if (rig == null)
                return null;

            return hand switch
            {
                WhichHand.Left => rig.leftHand,
                WhichHand.Right => rig.rightHand,
                _ => null
            };
        }
        //returns your current network player
        public static NetworkPlayer JR_YourNetworkPlayer()
        {
            return LocalPlayer.GetNetworkPlayer();
        }
        //returns your active steam id
        public static ulong SteamIdYours()
        {
            if (!Steamworks.SteamClient.IsValid || !Steamworks.SteamClient.IsLoggedOn)
                return 0;

            return Steamworks.SteamClient.SteamId.Value;
        }
        //reads site into string
        public static IEnumerator ReadFromSite(string url,Action<string> callbackoftext)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
    if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                MelonLoader.MelonLogger.Error(request.error);
            }
            else
            {
                string text = request.downloadHandler.text;
                callbackoftext?.Invoke(text);
            }

            request.Dispose(); // If available
        }


        //magazine checkers (attempt could be better but it is where its at right now which is fine for me :) )
        public static bool IsMagazine(SpawnableCrateReference reffy)
        {
            if (reffy == null || reffy.Crate == null)
                return false;

            var crate = reffy.Crate;
            var name = crate.name.ToLower();
            var barcode = crate.Barcode?.ID.ToLower();

            for (int i = 0; i < crate.Tags.Count; i++)
            {
                crate.Tags[i] = crate.Tags[i]?.ToLowerInvariant();
            }

            var tags = crate.Tags;

            bool matches =
    name.Contains(" mag ") ||
    name.EndsWith(" mag") ||
    name.StartsWith("mag ") ||
    name.StartsWith("mag_") ||
    name.Contains("magazine") ||
    name.EndsWith(" mag") ||
    name.EndsWith("_mag") ||
    name.StartsWith("cartridge") ||
    name.Contains("cartridge") ||
    name.EndsWith(" shells") ||
    name.StartsWith("cartridge - ") ||


    tags.Contains("mag") ||
    tags.Contains("magazine") ||
    tags.Contains("magazines") ||
    tags.Contains("cartridge") ||


    barcode.EndsWith("mag") ||
    barcode.EndsWith("cartridge") ||
    barcode.StartsWith("cartridge") ||
    barcode.Contains("cartridge");



            return matches;
        }
        public static bool IsMagazine(Crate reffy)
        {
            if (reffy == null)
                return false;

            var crate = reffy;
            var name = crate.name.ToLower();
            var barcode = crate.Barcode?.ID.ToLower();

            for (int i = 0; i < crate.Tags.Count; i++)
            {
                crate.Tags[i] = crate.Tags[i]?.ToLowerInvariant();
            }

            var tags = crate.Tags;

            bool matches =
    name.Contains(" mag ") ||
    name.EndsWith(" mag") ||
    name.StartsWith("mag ") ||
    name.StartsWith("mag_") ||
    name.Contains("magazine") ||
    name.EndsWith(" mag") ||
    name.EndsWith("_mag") ||
    name.StartsWith("cartridge") ||
    name.Contains("cartridge") ||
    name.EndsWith(" shells") ||
    name.StartsWith("cartridge - ") ||


    tags.Contains("mag") ||
    tags.Contains("magazine") ||
    tags.Contains("magazines") ||
    tags.Contains("cartridge") ||


    barcode.EndsWith("mag") ||
    barcode.EndsWith("cartridge") ||
    barcode.StartsWith("cartridge") ||
    barcode.Contains("cartridge");



            return matches;
        }
        //


        //strip colors from names to prevent weird html related stuff
        public static string StripColorTags(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return Regex.Replace(input, "<.*?>", string.Empty, RegexOptions.Singleline);
        }
        //can edit bonelab fusion melon prefs on the fly
        public static void EditFusionPreferences(string valuetoedit, object valuetochange)
        {
            var cat = MelonPreferences.GetCategory("BONELAB Fusion");
            if (cat != null)
            {
                var targetEntry = cat.Entries.FirstOrDefault(e => e.DisplayName == valuetoedit);
                if (targetEntry != null)
                {
                    targetEntry.BoxedValue = valuetochange;
                    cat.SaveToFile();
                }
            }
        }
        //returns related stuff to bodylog
        public static (PullCordDevice bodylogreturn, UnityEngine.MeshRenderer Outerring) JR_BodyLog(PhysicsRig PlayerTodo)
        {
            var physicsRig = PlayerTodo;
            if (physicsRig != null)
            {
                var right = physicsRig.m_elbowRt?.Find("BodyLogSlot/BodyLog");
                Transform rightMeshT = physicsRig.m_elbowRt?.Find("BodyLogSlot/BodyLog/BodyLog/BodyLog");
                UnityEngine.MeshRenderer rightMesh = rightMeshT != null ? rightMeshT.GetComponent<UnityEngine.MeshRenderer>() : null;

                if (right != null)
                {
                    var device = right.GetComponent<PullCordDevice>();
                    if (device != null)
                        return (device, rightMesh);
                }

                var left = physicsRig.m_elbowLf?.Find("BodyLogSlot/BodyLog");
                Transform leftMeshT = physicsRig.m_elbowLf?.Find("BodyLogSlot/BodyLog/BodyLog/BodyLog");
                UnityEngine.MeshRenderer leftMesh = leftMeshT != null ? leftMeshT.GetComponent<UnityEngine.MeshRenderer>() : null;

                if (left != null)
                {
                    var device = left.GetComponent<PullCordDevice>();
                    if (device != null)
                        return (device, leftMesh);
                }
            }

            return (null, null);
        }
        //do you have owner permission in the server currently
        public static bool AreYouOWNER()
        {
            if (!NetworkInfo.HasServer)
                return true;
            FusionPermissions.FetchPermissionLevel(SteamIdYours(), out var xc, out _);
            return xc == PermissionLevel.OWNER;
        }
        //do you have operator permission in the server currently
        public static bool AreYouOPERATOR()
        {
            if (!NetworkInfo.HasServer)
                return true;
            FusionPermissions.FetchPermissionLevel(SteamIdYours(), out var xc, out _);
            return xc == PermissionLevel.OPERATOR || xc == PermissionLevel.OWNER;
        }
        //are you the host 
        public static bool HostIsMe(NetworkPlayer playerNow)
        {
            return playerNow != null &&
                   playerNow.PlayerID != null &&
                   playerNow.PlayerID.IsHost &&
                   playerNow.IsMe();
        }
        //list of net entities with some filtered out because you dont wanna mess with some stuff
        public static System.Collections.Generic.HashSet<NetworkEntity> NetworkEntities()
        {
            return NetworkEntityManager.IDManager?.RegisteredEntities?.EntityIDLookup?.Keys?
                .Where(p =>
                {
                    var entity = p.JR_GetMarrowEntity();
                    return entity != null &&
                           !entity.JR_IsNetPlayer() &&
                           entity.JR_GetBarcodeID() != "Lakatrazz.FusionContent.Spawnable.NameTag" && p != JR_YourNetworkPlayer().NetworkEntity;
                })
                .ToHashSet();
        }
        //list of active players in lobby with host first then the rest
        public static System.Collections.Generic.HashSet<NetworkPlayer> NetworkPlayers(bool excludeMe = false, bool excludeMeAndHost = false)
        {
            return LabFusion.Entities.NetworkPlayer.Players
                .Where(p =>
                {
                    if (p == null || !p.PlayerID.IsValid)
                        return false;

                    if (excludeMe && p.IsMe())
                        return false;

                    if (excludeMeAndHost && (p.IsMe() || p.PlayerID.IsHost))
                        return false;

                    return true;
                })
                .OrderByDescending(p => p.PlayerID.IsHost)
                .ThenBy(p =>
                {
                    return StripColorTags(string.IsNullOrEmpty(p.Username) ? "" : p.Username).Trim();
                }, StringComparer.OrdinalIgnoreCase)
                .ToHashSet();
        }
        //opens a web page
        public static void OpenPageNow(string linknow)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = linknow,
                UseShellExecute = true
            });
        }
        //notification to popupon screen
        public static void NotificationNow(string Title, string Message, LabFusion.UI.Popups.NotificationType Type, float length = 1.0f, bool showtitle = true, bool savetomenu = false, System.Action Accept = default, System.Action Decline = default, bool donotdisturb = false)
        {
            if (donotdisturb)
                return;
            LabFusion.UI.Popups.Notifier.Cancel(LabFusion.UI.Popups.Notifier.CurrentNotification);
            LabFusion.UI.Popups.Notifier.Send(new LabFusion.UI.Popups.Notification
            {
                PopupLength = length,
                Title = Title,
                Message = Message,
                ShowPopup = showtitle,
                SaveToMenu = savetomenu,
                OnAccepted = Accept,
                OnDeclined = Decline,
                Type = Type,
                Tag = JRLibraryInfo.LibraryName
            });
        }
        //opens steam profile on pc
        public static void CheckSteamID(ulong steamid)
        {
            string steamProfileUrl = $"https://steamcommunity.com/profiles/{steamid}";
            try
            {

                OpenPageNow(steamProfileUrl);
                NotificationNow(JRLibraryInfo.LibraryName, "Opened Steam profile in browser.", LabFusion.UI.Popups.NotificationType.SUCCESS);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Failed to open URL: {ex.Message}");
                NotificationNow(JRLibraryInfo.LibraryName, "Failed to open profile link.", LabFusion.UI.Popups.NotificationType.ERROR);
            }
        }
        //the barcode string to your current avatar
        public static string JR_YourAvatarBarcodeID()
        {
            return Player.RigManager?.AvatarCrate?.Barcode?.ID ?? "NULL";
        }
        //changes you into the avatar
        public static void ChangeIntoAvi(string avibarcode)
        {
            if (IsBarcodeInGame(avibarcode))
            {
                if (JR_YourAvatarBarcodeID() == avibarcode)
                    return;


                var CrateRed = new AvatarCrateReference(avibarcode);

                Player.RigManager.SwapAvatarCrate(CrateRed.Barcode);

                DataManager.ActiveSave.PlayerSettings.CurrentAvatar = CrateRed.Barcode.ID;
                DataManager.TrySaveActiveSave(SaveFlags.Complete);

                var crate = CrateRed.Crate;
                if (crate != null)
                {
                    LocalPlayer.Metadata.AvatarTitle.SetValue(crate.Title);
                    LocalPlayer.Metadata.AvatarModID.SetValue(CrateFilterer.GetModID(crate.Pallet));
                }
            }
        }
        //returns the basecontroller class for rightcontroller
        public static BaseController RightController()
        {
            var rig = Player.RigManager;
            if (rig == null)
                return null;

            var controllerRig = rig.ControllerRig;
            if (controllerRig == null)
                return null;

            return controllerRig.rightController;
        }
        //returns the basecontroller class for leftcontroller
        public static BaseController LeftController()
        {
            var rig = Player.RigManager;
            if (rig == null)
                return null;

            var controllerRig = rig.ControllerRig;
            if (controllerRig == null)
                return null;

            return controllerRig.leftController;
        }
        //returns is game is fully loaded!
        public static bool FullLoadedNow()
        {
            if (!NetworkInfo.HasServer)
                return false;

            if (!NetworkInfo.HasLayer)
                return false;

            if (!FusionSceneManager.HasTargetLoaded() && !FusionSceneManager.IsDelayedLoading())
                return false;

            if (!RigData.HasPlayer)
                return false;

            return true;
        }
        //full spawner for fusion and compatible with singleplayer only
        public static Poolee SpawnIt(string BARCODE, Vector3 Position, Quaternion rotation, bool localonly = false)
        {
            if (localonly || !NetworkInfo.HasServer)
            {
                var spawnable = new Spawnable { crateRef = new SpawnableCrateReference(BARCODE) };
                Poolee spawnyc = null;
                LocalAssetSpawner.Register(spawnable);
                LocalAssetSpawner.Spawn(spawnable, Position, rotation, callbackpoole =>
                {
                    spawnyc = callbackpoole;
                });
                return spawnyc;
            }
            else
            {
                FusionPermissions.FetchPermissionLevel(SteamIdYours(), out var selfLevel, out _);
                if (FusionPermissions.HasSufficientPermissions(selfLevel, LobbyInfoManager.LobbyInfo.DevTools))
                {
                    var spawnable = new Spawnable { crateRef = new SpawnableCrateReference(BARCODE) };
                    var info = new SpawnRequestInfo
                    {
                        Spawnable = spawnable,
                        Position = Position,
                        Rotation = rotation,
                        SpawnSource = EntitySource.Player,
                        SpawnEffect = true,
                        SpawnCallback = _ => { }
                    };

                    Spawn(info);
                    return null;
                }
                else
                {
                    NotificationNow(JRLibraryInfo.LibraryName, "Invalid Permissions!", LabFusion.UI.Popups.NotificationType.ERROR, 3.0f);
                    return null;
                }
            }
        }
        //returns playerinfo based on steam id if they are online in fusion you will get a result
        public static void FindPlayersLobbyFromPlayerSteamID(ulong steamId, System.Action<bool, PlayerInfo> onResult)
        {
            var matchmaker = NetworkLayerManager.Layer?.Matchmaker;

            if (matchmaker == null)
            {
                onResult?.Invoke(false, null);
                return;
            }

            matchmaker.RequestLobbies(info =>
            {
                foreach (var lobby in info.Lobbies)
                {
                    var players = lobby.Metadata.LobbyInfo?.PlayerList?.Players;
                    if (players == null)
                        continue;

                    foreach (var player in players)
                    {
                        if (player != null && player.PlatformID == steamId)
                        {
                            onResult?.Invoke(true, player);
                            return;
                        }
                    }
                }

                onResult?.Invoke(false, new PlayerInfo
                {
                    AvatarModID = -1,
                    Description = "Player Not On Fusion Publically!",
                    AvatarTitle = "Player Not On Fusion Publically!",
                    Nickname = "Player Not On Fusion Publically!",
                    Username = "Player Not On Fusion Publically!"
                });
            });
        }
        //returns playerinfo based on playername if they are online in fusion you will get a result
        public static void FindPlayersLobbyFromPlayerName(string searchName, System.Collections.Generic.Dictionary<LobbyInfo, System.Collections.Generic.List<PlayerInfo>> results, Action<bool> onResult)
        {
            var matchmaker = NetworkLayerManager.Layer?.Matchmaker;

            if (matchmaker == null || string.IsNullOrWhiteSpace(searchName))
            {
                onResult?.Invoke(false);
                return;
            }

            string search = searchName.Trim().ToLowerInvariant();
            results.Clear();

            matchmaker.RequestLobbies(lobbiesInfo =>
            {
                foreach (var lobbyWrapper in lobbiesInfo.Lobbies)
                {
                    var lobbyInfo = lobbyWrapper.Metadata.LobbyInfo;
                    var players = lobbyInfo?.PlayerList?.Players;

                    if (players == null)
                        continue;

                    foreach (var player in players)
                    {
                        if (player == null)
                            continue;

                        bool matched = false;

                        if (!string.IsNullOrWhiteSpace(player.Nickname))
                        {
                            string nickname = player.Nickname.Trim().ToLowerInvariant();
                            if (nickname.Contains(search))
                                matched = true;
                        }

                        if (!matched && !string.IsNullOrWhiteSpace(player.Username))
                        {
                            string username = player.Username.Trim().ToLowerInvariant();
                            if (username.Contains(search))
                                matched = true;
                        }

                        if (matched)
                        {
                            if (!results.TryGetValue(lobbyInfo, out var playerList))
                            {
                                playerList = new System.Collections.Generic.List<PlayerInfo>();
                                results[lobbyInfo] = playerList;
                            }

                            playerList.Add(player);
                        }
                    }
                }

                onResult?.Invoke(results.Count > 0);
            });
        }
        //returns true if the barcode string you provided is a avatar crate
        public static bool IsAvatarCrateExist(string barcode)
        {
            var avatarRef = new AvatarCrateReference(barcode);
            if (avatarRef.Crate != null)
            {
                var pallet = avatarRef.Crate.Pallet;
                return pallet != null && pallet.Barcode != null;
            }
            return false;
        }
        //returns true if the barcode string you provided is a level crate
        public static bool IsLevelCrateExist(string barcode)
        {
            var avatarRef = new LevelCrateReference(barcode);
            if (avatarRef.Crate != null)
            {
                var pallet = avatarRef.Crate.Pallet;
                return pallet != null && pallet.Barcode != null;
            }
            return false;
        }
        //returns true if the barcode string you provided is a spawnable crate
        public static bool IsSpawnableCrateExist(string barcode)
        {
            var avatarRef = new SpawnableCrateReference(barcode);
            if (avatarRef.Crate != null)
            {
                var pallet = avatarRef.Crate.Pallet;
                return pallet != null && pallet.Barcode != null;
            }
            return false;
        }
        //returns true if the barcode you provided at all is installed to the game
        public static bool IsBarcodeInGame(string barcode)
        {
            var spawnableRef = new SpawnableCrateReference(barcode);
            if (spawnableRef.Crate != null)
            {
                var pallet = spawnableRef.Crate.Pallet;
                return pallet != null && pallet.Barcode != null;
            }

            var levelRef = new LevelCrateReference(barcode);
            if (levelRef.Crate != null)
            {
                var pallet = levelRef.Crate.Pallet;
                return pallet != null && pallet.Barcode != null;
            }

            var avatarRef = new AvatarCrateReference(barcode);
            if (avatarRef.Crate != null)
            {
                var pallet = avatarRef.Crate.Pallet;
                return pallet != null && pallet.Barcode != null;
            }

            return false;
        }
        //despawns select entity
        public static void DespawnNow(NetworkEntity entity)
        {
            if (AreYouOWNER())
            {
                NetworkAssetSpawner.Despawn(new NetworkAssetSpawner.DespawnRequestInfo()
                {
                    EntityID = entity.ID,
                    DespawnEffect = false,
                });
            }
        }
        //despawns all matching a specific barcode
        public static void DespawnAllMatchingBarcode(string barcode)
        {
            foreach(var Netentity in NetworkEntities())
            {
                if (Netentity.JR_GetMarrowEntity().JR_GetBarcodeID() == barcode)
                {
                    DespawnNow(Netentity);
                }
            }
        }

        
        //this looksup information about a specific #modioID
        private static bool _isLookingUpMod = false;
        public static IEnumerator ModioInfo(int modIOID, Action<ModCallbackInfo> onFinished)
        {
            if (_isLookingUpMod)
            {
                NotificationNow(
                    JRLibraryInfo.LibraryName,
                    $"Already looking up a mod please WAIT!",
                    LabFusion.UI.Popups.NotificationType.WARNING
                );
                yield break;
            }

            _isLookingUpMod = true;

            NotificationNow(
                JRLibraryInfo.LibraryName,
                $"Reading mod info for ID {modIOID}... please wait.",
                LabFusion.UI.Popups.NotificationType.INFORMATION
            );

            ModCallbackInfo infoNow = default;
            bool finished = false;

            ModTransaction transaction = new()
            {
                ModFile = new ModIOFile(modIOID),
                Callback = Callback
            };

            void Callback(DownloadCallbackInfo info)
            {
                if (info.Result != ModResult.SUCCEEDED)
                {
                    NotificationNow(
                        JRLibraryInfo.LibraryName,
                        "The content failed to install! Make sure you are logged into mod.io in VoidG114 or BONELAB Hub!",
                        LabFusion.UI.Popups.NotificationType.WARNING
                    );
                }
            }

            ModIOFile modFile = transaction.ModFile;

            ModIOManager.GetMod(modFile.ModID, OnRequestedMod);

            void OnRequestedMod(ModCallbackInfo info)
            {
                infoNow = info;
                finished = true;

                if (info.Result == ModResult.SUCCEEDED)
                {
                    NotificationNow(
                        JRLibraryInfo.LibraryName,
                        $"Mod found: {info.Data.NameID}",
                        LabFusion.UI.Popups.NotificationType.INFORMATION
                    );
                }
                else
                {
                    NotificationNow(
                        JRLibraryInfo.LibraryName,
                        "Failed to retrieve mod info.",
                        LabFusion.UI.Popups.NotificationType.WARNING
                    );
                }
            }

            while (!finished)
                yield return null;

            _isLookingUpMod = false;

            onFinished?.Invoke(infoNow);
        }
        //downloads mod directly to your game with the #modioID
        public static void DownloadModIOMod(int modIOID, bool noti = true)
        {
            ModTransaction transaction = new ModTransaction()
            {
                ModFile = new ModIOFile(modIOID),
                Callback = Callback
            };

            ModIODownloader.EnqueueDownload(transaction);

            if (noti)
            {
                NotificationNow(
                    JRLibraryInfo.LibraryName,
                    "Wait Until You See Installed Notification Then Press Whatever You Pressed AGAIN!",
                    LabFusion.UI.Popups.NotificationType.WARNING,
                    6.0f
                );

            }

            void Callback(DownloadCallbackInfo info)
            {
                if (info.Result != ModResult.SUCCEEDED)
                {
                    NotificationNow(
                        JRLibraryInfo.LibraryName,
                        "The Content failed to install! Make sure you are logged into mod.io in VoidG114 or BONELAB Hub!",
                        LabFusion.UI.Popups.NotificationType.WARNING
                    );
                }
            }
        }
        //is the float value within two values returns true if so used to mainly parse serialized stats but can be used for anything you need
        
        
        public static bool Iswithintwovalues(float valuetocheck, float min, float max)
        {
            if (valuetocheck >= min && valuetocheck <= max)
            {
                return true;
            }
            return false;
        }
        //changes the bodlylog slot index to any barcode you provide
        public static void ChangeBodyLogAvatarSlot(int slotindex, string avatarbarcode, bool notification = true)
        {
            if (IsBarcodeInGame(avatarbarcode))
            {
                if (DataManager.ActiveSave.PlayerSettings.FavoriteAvatars == null)
                {
                    //this code is to prevent errors and fill your empty bodylog do not remove this!
                    DataManager.ActiveSave.PlayerSettings.FavoriteAvatars = new Il2CppSystem.Collections.Generic.List<string>();
                    for (int i = 0; i < 6; i++)  // 0 to 5
                    {
                        DataManager.ActiveSave.PlayerSettings.FavoriteAvatars.Add("EMPTY");
                    }
                    //
                }


                DataManager.ActiveSave.PlayerSettings.FavoriteAvatars[slotindex - 1] = avatarbarcode;
                if (notification)
                {
                    NotificationNow(JRLibraryInfo.LibraryName, $"Changed {slotindex} Slot And Saved!", LabFusion.UI.Popups.NotificationType.SUCCESS);
                }
                DataManager.TrySaveActiveSave(SaveFlags.Complete);
                JR_BodyLog(Player.PhysicsRig).bodylogreturn.LoadFavoriteAvatars();
                JR_BodyLog(Player.PhysicsRig).bodylogreturn.BodyMallUpdate();
            }
            else
            {
                NotificationNow(JRLibraryInfo.LibraryName, "This Does Not Exist In Your Game Install This Mod For It To Exist...", LabFusion.UI.Popups.NotificationType.ERROR, 3.0f);
            }
        }
 
        //if you dont have this modid installed then it auto installs it
        public static bool IfDontHaveInstallThenDo(string barcode, int modioID, bool notification = false)
        {
            if (!CrateFilterer.HasCrate<SpawnableCrate>(new Barcode(barcode)))
            {

                DownloadModIOMod(modioID, notification);
                return false;
            }


            return true;
        }
        //sets the barcode to your held spawn gun 
        public static void SetBarCodeToSpawnGun(string barcode)
        {
            foreach (var hand in new[] { WhichHand.Left, WhichHand.Right })
            {
                var yourHand = JR_YourGetHand(hand);
                if (yourHand.JR_IsGrabbedSpawnGun())
                {
                    var spawnGun = yourHand.JR_HandGrabbedSpawnGun();
                    var crateRef = new SpawnableCrateReference(barcode);

                    spawnGun._selectedCrate = crateRef.Crate;
                    spawnGun.SetPreviewMesh();

                    SpawnEffects.CallDespawnEffect(LibraryCode.JR_YourGetHand(hand)?.JR_GetMarrowEntity());
                    SpawnEffects.CallSpawnEffect(LibraryCode.JR_YourGetHand(hand)?.JR_GetMarrowEntity());

                }
            }
        }
        //returns the barcode currently in hand left or right you can assign it
        public static string BarcodeInHand(WhichHand hand)
        {
            return JR_YourGetHand(hand)?
                .JR_GetMarrowEntity()?
                .JR_GetBarcodeID() ?? string.Empty;
        }
        //returns the barcode currently in both hands
        public static string BarcodesInBothHands()
        {
            string leftHand = BarcodeInHand(WhichHand.Left);
            string rightHand = BarcodeInHand(WhichHand.Right);

            return $"Left Hand: {leftHand} | Right Hand: {rightHand}";
        }


        //unloads and uninstalls mod by pallet
        public static void DeleteModioMod(Pallet PalletNow, bool notif = true)
        {
            if (CrateFilterer.GetModID(PalletNow) != -1)
            {

                var (folder, manif, fullpal, manny) = GetPalletFolder(PalletNow?.name);

                if (!string.IsNullOrEmpty(folder) && System.IO.Directory.Exists(folder))
                {
                    UnloadPallet(PalletNow);
                    System.IO.Directory.Delete(folder, true);
                    System.IO.File.Delete(manif);
                    if (notif)
                        NotificationNow(JRLibraryInfo.LibraryName, $"Deleted {PalletNow?.name}.", NotificationType.ERROR, 3.0f);
                }
            }
        }
        public static void UnloadPallet(Pallet pallet)
        {
            if (pallet == null)
                return;

            var bundles = pallet._packedAssets;
            if (bundles != null)
            {
                foreach (var bundle in bundles)
                {
                    bundle?.marrowAsset.UnloadAsset(true);
                }
            }

            AssetWarehouse.Instance.UnloadPallet(pallet);

            MelonLogger.Msg($"Unloaded pallet: {pallet.Title}");
        }
        public static (string folderpath, string manifestpath, string fullpallet, PalletManifest palletm) GetPalletFolder(string palletTitle, bool openSelectionPc = false)
        {
            var manifests = AssetWarehouse.Instance?.GetPalletManifests()?.ToArray();
            if (manifests == null)
                return (null, null, null, null);

            var pallet = manifests
                .FirstOrDefault(m => m.Pallet != null && m.Pallet.Title == palletTitle);

            if (pallet?.PalletPath == null)
                return (null, null, null, null);

            string folderPath = Path.GetDirectoryName(pallet.PalletPath);
            if (openSelectionPc && System.IO.Directory.Exists(folderPath))
            {
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{folderPath}\"");
                NotificationNow(JRLibraryInfo.LibraryName, $"Opened {palletTitle} download folder.", NotificationType.SUCCESS, 2f);
            }

            return (folderPath, pallet.ManifestPath, pallet.PalletPath, pallet);
        }
        //


        //just reads and outputs your current modio token to a string
        public static string? ReadModIOToken()
        {
            try
            {
                string settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "AppData",
                    "LocalLow",
                    "Stress Level Zero",
                    "BONELAB",
                    "mod_settings.json"
                );

                if (!File.Exists(settingsPath))
                    return null;

                string jsonText = File.ReadAllText(settingsPath);

                JObject settingsJson = JObject.Parse(jsonText);

                return settingsJson.SelectToken("mod.io.access_token")?.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read Mod.io token: {ex.Message}");
                return null;
            }
        }
        
        //this function dumps all pallets to a neat format on your pc
        private static bool _isDumpRunning = false;
        public static IEnumerator DumpPalletsCoroutine(string PalletDumpLocation)
        {
            if (_isDumpRunning)
            {
                MelonLogger.Error("⚠️ Crate dump is already running!");
                yield break;
            }

            _isDumpRunning = true;
            MelonLogger.Warning("🔍 Starting crate dump...");

            StringBuilder sb = new();

            var pallets = AssetWarehouse.Instance.GetPallets();
            int batchSize = 150;
            int processedCrates = 0;

            int totalCrates = 0;
            foreach (var pallet in pallets)
                if (pallet.Crates != null)
                    totalCrates += pallet.Crates.Count;

            foreach (var pallet in pallets)
            {
                sb.AppendLine($"[Pallet] {StripColorTags(pallet.Title)} (Author: {pallet.Author})");

                if (pallet.Crates == null) continue;

                foreach (var cratey in pallet.Crates)
                {
                    if (cratey.Tags == null) continue;

                    sb.AppendLine($"  └── [Crate] {StripColorTags(cratey.Title)}");
                    sb.AppendLine($"      └── Barcode: {cratey.Barcode.ID}");

                    var tagsSet = new System.Collections.Generic.HashSet<string>();
                    foreach (var tag in cratey.Tags)
                        tagsSet.Add(tag);

                    if (tagsSet.Count > 0)
                        sb.AppendLine($"      └── Tags: {string.Join(", ", tagsSet)}");

                    processedCrates++;

                    if (processedCrates % batchSize == 0)
                    {
                        int barLength = 30;
                        float progress = (float)processedCrates / totalCrates;
                        int filled = (int)(progress * barLength);
                        string bar = $"[{new string('#', filled)}{new string('-', barLength - filled)}] {progress:P1}";
                        MelonLogger.Warning($"⏳ Dumping crates... {bar}");

                        yield return new WaitForSeconds(0.1f);
                    }
                }

                sb.AppendLine();
            }

            MelonLogger.Warning("⏳ Dumping crates... [##############################] 100%");

            try
            {
                File.WriteAllText(PalletDumpLocation, sb.ToString());
                MelonLogger.Warning($"✅ Crate dump written to: {PalletDumpLocation}");

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/c start \"\" \"{PalletDumpLocation}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to write pallet dump: {ex.Message}");
            }

            _isDumpRunning = false;
        }
        //outputs a randomly selected player with custom excludes
        public static NetworkPlayer GetRandomOtherPlayer(NetworkPlayer excludeCertainPlayer = null, PlayerID excludecertainplayerid = null)
        {
            var me = JR_YourNetworkPlayer();
            var others = new System.Collections.Generic.HashSet<NetworkPlayer>();

            foreach (var player in NetworkPlayers())
            {
                if (player != me && player != excludeCertainPlayer && player.PlayerID != excludecertainplayerid)
                    others.Add(player);
            }

            return others.ElementAt(new System.Random().Next(others.Count));
        }
        //returns your playerid class
        public static PlayerID JR_YourPlayerID()
        {
            return PlayerIDManager.LocalID;
        }   
        //returns your unique smallid from your current lobby
        public static byte JR_YourSmallID()
        {
            return PlayerIDManager.LocalID.SmallID;
        }      
        //returns your networkmetadata class where your profile info is stored so you can edit it accordingly
        public static NetworkMetadata JR_YourMetaData()
        {

            return LocalPlayer.Metadata.Metadata;
        }
        //returns the current fusion global ban list fetches from the github
        public static void BanListChecking(Action<GlobalBanList> callback)
        {
            MelonCoroutines.Start(ReadFromSite(
                "https://raw.githubusercontent.com/Lakatrazz/Fusion-Lists/refs/heads/main/globalBans.json",
                (siteText) =>
                {
                    if (string.IsNullOrEmpty(siteText))
                    {
                        MelonLogger.Error("Failed to download global ban list.");
                        callback?.Invoke(null);
                        return;
                    }

                    try
                    {
                        JsonSerializerOptions options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            NumberHandling = JsonNumberHandling.AllowReadingFromString
                        };
                        GlobalBanList banList = JsonSerializer.Deserialize<GlobalBanList>(siteText, options);

                        if (banList?.Bans == null)
                        {
                            MelonLogger.Error("Global ban list is invalid.");
                            callback?.Invoke(null);
                            return;
                        }

                        MelonLogger.Msg($"Total Fusion Global Bans: {banList.Bans.Count}");

                        callback?.Invoke(banList);
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Error($"Failed to deserialize GlobalBanList: {ex}");
                        callback?.Invoke(null);
                    }
                }));
        }
        //returns if your banned or not from fusion (NEED TO USE THIS after you logged into steam layer and such)
        public static void AreYouBannedFromFusion(Action<bool> callback)
        {
            MelonCoroutines.Start(ReadFromSite("https://raw.githubusercontent.com/Lakatrazz/Fusion-Lists/refs/heads/main/globalBans.json", (siteText) =>
            {
                if (string.IsNullOrEmpty(siteText))
                {
                    callback(false);
                    return;
                }

                try
                {
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    };
                    GlobalBanList banList = JsonSerializer.Deserialize<GlobalBanList>(siteText, options);

                    if (banList?.Bans == null)
                    {
                        MelonLogger.Warning("Ban list missing.");
                        callback(false);
                        return;
                    }

                    bool banned = banList.Bans.Any(b =>
                        b?.Platforms != null &&
                        b.Platforms.Any(p => p.PlatformID == SteamIdYours())
                    );
                    callback(banned);
                }
                catch (Exception ex)
                {
                    callback(false);
                }
            }));
        }


        //use this to get the list of players that you have met
        public static readonly HashSet<PlayerInfo> RecentlyMetPlayers = [];
        [HarmonyPatch(typeof(SteamNetworkLayer), "OnPlayerJoin")]
        [HarmonyPriority(int.MaxValue - 1)]
        private static class RecentlyMetPlayersHook
        {
            private static void Postfix(PlayerID id)
            {
                if (id != null && !RecentlyMetPlayers.Any(p => p?.PlatformID == id.PlatformID))
                {
                    RecentlyMetPlayers.Add(new PlayerInfo
                    {
                        Nickname = id.Metadata.Nickname.GetValue(),
                        Username = id.Metadata.Username.GetValue(),
                        PlatformID = id.PlatformID,
                        AvatarModID = id.Metadata.AvatarModID.GetValue(),
                        AvatarTitle = id.Metadata.AvatarTitle.GetValue(),
                        Description = id.Metadata.Description.GetValue()
                    });
                }
            }
        }


        //unequips all cosmetics 
        public static void UnequipAllFusionCosmetics()
        {
            foreach (var equipped in JR_YourNetworkPlayer().PlayerID.EquippedItems)
            {
                PointSaveManager.SetEquipped(equipped, false);
                PointItemSender.SendPointItemEquip(equipped, false);
            }

            PointItemManager.UnequipAll();
        }
        //returns list of active cosmetics on your player (barcodes)
        public static List<string> JR_Cosmetics() { return JR_YourNetworkPlayer().PlayerID.EquippedItems; }
        //sets your nickname
        public static void JR_SetYourNickName(string newnickname) => JR_YourMetaData().TrySetMetadata("Nickname", newnickname);
        //sets your username
        public static void JR_SetYourUsername(string newusername) => JR_YourMetaData().TrySetMetadata("Username", newusername);
        //sets your description
        public static void JR_SetYourDescription(string newdescription) => JR_YourMetaData().TrySetMetadata("Description", newdescription);
        //loads into new map with barcode supports fusion&singleplayer
        public static void LoadIntoMap(string LevelBarCode)
        {
            if (NetworkInfo.IsHost && NetworkInfo.HasServer)
            {
                NetworkHelper.Disconnect();
                SceneStreamer.Load(new LevelCrateReference(LevelBarCode).Barcode);
            }
            else
            {
                SceneStreamer.Load(new LevelCrateReference(LevelBarCode).Barcode);
            }
        }
        //returns the info of the last person who did damage to you
        public static PlayerID? LastFusionAttacker()
        {
            if (!FusionPlayer.TryGetLastAttacker(out PlayerID attacker))
                return null;

            return attacker.Metadata != null ? attacker : null;
        }
        //adds holding item to save game favorites (adds it to the favorites tab in the spawngunui)
        public static void AddHoldingItemToSaveGameFavorites(WhichHand CurrentHand)
        {
            if (!DataManager.ActiveSave.PlayerSettings.FavoriteSpawnables.Contains(BarcodeInHand(CurrentHand).Trim()))
            {
                DataManager.ActiveSave.PlayerSettings.FavoriteSpawnables.Add(BarcodeInHand(CurrentHand).Trim());
                DataManager.TrySaveActiveSave(SaveFlags.Complete);
                NotificationNow(JRLibraryInfo.LibraryName, $"Added {BarcodeInHand(CurrentHand).JR_BarcodePalletName()} To SaveGame Favorites!", NotificationType.SUCCESS);
            }
            else
            {
                DataManager.ActiveSave.PlayerSettings.FavoriteSpawnables.Remove(BarcodeInHand(CurrentHand).Trim());
                DataManager.TrySaveActiveSave(SaveFlags.Complete);
                NotificationNow(JRLibraryInfo.LibraryName, $"Removed {BarcodeInHand(CurrentHand).JR_BarcodePalletName()} From SaveGame Favorites!", NotificationType.SUCCESS);
            }
        }


    }
}