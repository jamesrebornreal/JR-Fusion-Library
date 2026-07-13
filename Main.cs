using BoneLib;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Bonelab.SaveData;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.PuppetMasta;
using Il2CppSLZ.Marrow.SaveData;
using Il2CppSLZ.Marrow.VFX;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSystem.Diagnostics;
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
using LabFusion.Scene;
using MelonLoader;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static LabFusion.RPC.NetworkAssetSpawner;
[assembly: MelonInfo(typeof(BoneLib.BuildInfo), BoneLib.BuildInfo.Name, BoneLib.BuildInfo.Version, BoneLib.BuildInfo.Author)]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]
namespace FusionLibrary
{
    public static class BuildInfo
    {
        public const string Name = "JR Fusion Library";
        public const string Description = "A bunch of Fusion functions I made over time for Fusion Protector and other projects to help developers create code mods and enhance the Fusion experience."; // Description for the Mod.  (Set as null if none)
        public const string Author = "James Reborn";
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

            public static void JR_Despawn(this NetworkEntity entity) =>FusionLibrary.DespawnNow(entity);
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
                return player.JR_SteamID() == FusionLibrary.SteamIdYours();
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
        }
    public static class BarCodeIDExtensions
    {
        public static string JR_BarcodeCrateName(this string idnow)
        {
            if (FusionLibrary.IsAvatarCrateExist(idnow))
                return FusionLibrary.StripColorTags(new AvatarCrateReference(idnow)?.Crate?.name);

            if (FusionLibrary.  IsLevelCrateExist(idnow))
                return FusionLibrary.StripColorTags(new LevelCrateReference(idnow)?.Crate?.name);

            if (FusionLibrary.IsSpawnableCrateExist(idnow))
                return FusionLibrary.StripColorTags(new SpawnableCrateReference(idnow)?.Crate?.name);

            return "NULL";
        }

        public static string JR_BarcodePalletName(this string idnow)
        {
            if (FusionLibrary.IsAvatarCrateExist(idnow))
                return FusionLibrary.StripColorTags(new AvatarCrateReference(idnow)?.Crate?.Pallet?.name);

            if (FusionLibrary.IsLevelCrateExist(idnow))
                return FusionLibrary.StripColorTags(new LevelCrateReference(idnow)?.Crate?.Pallet?.name);

            if (FusionLibrary.IsSpawnableCrateExist(idnow))
                return FusionLibrary.StripColorTags(new SpawnableCrateReference(idnow)?.Crate?.Pallet?.name);

            return "NULL";
        }

        public static string JR_BarcodeAuthor(this string idnow)
        {
            if (FusionLibrary.IsAvatarCrateExist(idnow))
                return FusionLibrary.StripColorTags(new AvatarCrateReference(idnow)?.Crate?.Pallet?.Author);

            if (FusionLibrary.IsLevelCrateExist(idnow))
                return FusionLibrary.StripColorTags(new LevelCrateReference(idnow)?.Crate?.Pallet?.Author);

            if (FusionLibrary.IsSpawnableCrateExist(idnow))
                return FusionLibrary.StripColorTags(new SpawnableCrateReference(idnow)?.Crate?.Pallet?.Author);

            return "NULL";
        }


    }
    //

    public class FusionLibrary : MelonMod
    {
        public static handnow handnowreal = handnow.Right;

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

        //returns your active steam id
        public static ulong SteamIdYours()
        {
            if (!Steamworks.SteamClient.IsValid || !Steamworks.SteamClient.IsLoggedOn)
                return 0;

            return Steamworks.SteamClient.SteamId.Value;
        }
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
        //returns your current network player
        public static NetworkPlayer JR_YourNetworkPlayer()
        {
            return LocalPlayer.GetNetworkPlayer();
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
                Tag = BuildInfo.Name
            });
        }
        //opens steam profile on pc
        public static void CheckSteamID(ulong steamid)
        {
            string steamProfileUrl = $"https://steamcommunity.com/profiles/{steamid}";
            try
            {

                OpenPageNow(steamProfileUrl);
                NotificationNow(BuildInfo.Name, "Opened Steam profile in browser.", LabFusion.UI.Popups.NotificationType.SUCCESS);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Failed to open URL: {ex.Message}");
                NotificationNow(BuildInfo.Name, "Failed to open profile link.", LabFusion.UI.Popups.NotificationType.ERROR);
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
                    NotificationNow(BuildInfo.Name, "Invalid Permissions!", LabFusion.UI.Popups.NotificationType.ERROR, 3.0f);
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
        //this looksup information about a specific #modioID
        private static bool _isLookingUpMod = false;
        public static IEnumerator ModioInfo(int modIOID, Action<ModCallbackInfo> onFinished)
        {
            if (_isLookingUpMod)
            {
                NotificationNow(
                    BuildInfo.Name,
                    $"Already looking up a mod please WAIT!",
                    LabFusion.UI.Popups.NotificationType.WARNING
                );
                yield break;
            }

            _isLookingUpMod = true;

            NotificationNow(
                BuildInfo.Name,
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
                        BuildInfo.Name,
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
                        BuildInfo.Name,
                        $"Mod found: {info.Data.NameID}",
                        LabFusion.UI.Popups.NotificationType.INFORMATION
                    );
                }
                else
                {
                    NotificationNow(
                        BuildInfo.Name,
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
                    BuildInfo.Name,
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
                        BuildInfo.Name,
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
                    NotificationNow(BuildInfo.Name, $"Changed {slotindex} Slot And Saved!", LabFusion.UI.Popups.NotificationType.SUCCESS);
                }
                DataManager.TrySaveActiveSave(SaveFlags.Complete);
                JR_BodyLog(Player.PhysicsRig).bodylogreturn.LoadFavoriteAvatars();
                JR_BodyLog(Player.PhysicsRig).bodylogreturn.BodyMallUpdate();
            }
            else
            {
                NotificationNow(BuildInfo.Name, "This Does Not Exist In Your Game Install This Mod For It To Exist...", LabFusion.UI.Popups.NotificationType.ERROR, 3.0f);
            }
        }
        //clear constraints on a player (owner only locked to prevent abuse)
        public static void ClearConstraints(NetworkPlayer Netty)
        {
            if (AreYouOWNER())
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

                    SpawnEffects.CallDespawnEffect(FusionLibrary.JR_YourGetHand(hand)?.JR_GetMarrowEntity());
                    SpawnEffects.CallSpawnEffect(FusionLibrary.JR_YourGetHand(hand)?.JR_GetMarrowEntity());

                }
            }
        }
        //returns the barcode currently in both hands
        public static string BarcodeInHand()
        {
            var hand = handnowreal == handnow.Left ? WhichHand.Left : WhichHand.Right;
            var entity = JR_YourGetHand(hand)?.JR_GetMarrowEntity();
            return entity != null ? entity.JR_GetBarcodeID() : string.Empty;
        }
    }
}