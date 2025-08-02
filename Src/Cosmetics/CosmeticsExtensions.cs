using Catapult.Modules.Items.Protocol.Dtos;
using FallGuys.Player.Protocol.Client.Cosmetics;
using FG.Common.CMS;
using FGClient.CatapultServices;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Net.WebSockets.ClientWebSocket;

namespace ThatOneRandom3AMProject.Cosmetics
{
    internal static class CosmeticsExtensions
    {
        internal static CosmeticsCollectionDto GetCosmetics()
        {
            var service = CatapultServices.Instance.PlayerCosmeticsService.Cast<PlayerCosmeticsService>();
            if (service.CosmeticsCollection != null)
                return service.CosmeticsCollection;

            service.CosmeticsCollection = new(ClassInjector.DerivedConstructorPointer<CosmeticsCollectionDto>())
            {
                ColourSchemes = BuildCMSCosmetics<ColourSchemeDto, ColourOption>(x => ItemDtoToColourSchemeDto(CMSDefinitionToItemDto(x))),
                Emotes = BuildCMSCosmetics<EmoteDto, EmotesOption>(x => ItemDtoToEmoteDto(CMSDefinitionToItemDto(x))),
                Faceplates = BuildCMSCosmetics<FaceplateDto, FaceplateOption>(x => ItemDtoToFaceplateDto(CMSDefinitionToItemDto(x))),
                LowerCostumePieces = BuildCMSCosmetics<LowerCostumePieceDto, CostumeOption>(x => ItemDtoToCostumeLowerDto(CMSDefinitionToItemDto(x))),
                MonolithicCostumes = BuildCMSCosmetics<MonolithicCostumeDto, CostumeOption>(x => ItemDtoToFullCostumeDto(CMSDefinitionToItemDto(x))),
                Nameplates = new(),
                Nicknames = new(),
                Patterns = BuildCMSCosmetics<PatternDto, SkinPatternOption>(x => ItemDtoToPatternDto(CMSDefinitionToItemDto(x))),
                Plinths = new(),
                Punchlines = BuildCMSCosmetics<PunchlineDto, VictoryOption>(x => ItemDtoToVictoryDto(CMSDefinitionToItemDto(x))),
                UpperCostumePieces = BuildCMSCosmetics<UpperCostumePieceDto, CostumeOption>(x => ItemDtoToCostumeUpperDto(CMSDefinitionToItemDto(x))),
            };

            return service.CosmeticsCollection;
        }
        internal static Il2CppSystem.Collections.Generic.List<TContent> BuildCMSCosmetics<TContent, TData>(Func<TData, TContent> push) where TContent : Il2CppSystem.Object where TData : UnityEngine.Object
        {
            var res = new Il2CppSystem.Collections.Generic.List<TContent>();

            foreach (var itm in Resources.FindObjectsOfTypeAll<TData>())
            {
                if (typeof(TContent) == typeof(LowerCostumePieceDto) && itm.Cast<CostumeOption>().CostumeType != CostumeType.Bottom)
                    continue;

                if (typeof(TContent) == typeof(UpperCostumePieceDto) && itm.Cast<CostumeOption>().CostumeType != CostumeType.Top)
                    continue;

                if (typeof(TContent) == typeof(MonolithicCostumeDto) && itm.Cast<CostumeOption>().CostumeType != CostumeType.Full)
                    continue;

                res.Add(push(itm));
            }

            return res;
        }

        internal static ItemDto CMSDefinitionToItemDto(dynamic targ)
        {
            return new()
            {
                ContentId = targ.ItemId,
                ContentType = targ.CMSGroupID,
                Id = targ.FullItemId,
                Quantity = 1,
            };
        }

        internal static ColourSchemeDto ItemDtoToColourSchemeDto(ItemDto itemDto, bool isFav = false)
        {
            ColourSchemeDto cosmeticDto = new()
            {
                EarnedAt = Il2CppSystem.DateTime.Now,
                Item = itemDto,
            };
            return cosmeticDto;
        }

        internal static PatternDto ItemDtoToPatternDto(ItemDto itemDto)
        {
            PatternDto cosmeticDto = new()
            {
                EarnedAt = Il2CppSystem.DateTime.Now,
                Item = itemDto,
            };
            return cosmeticDto;
        }

        internal static FaceplateDto ItemDtoToFaceplateDto(ItemDto itemDto)
        {
            FaceplateDto cosmeticDto = new()
            {
                EarnedAt = Il2CppSystem.DateTime.Now,
                Item = itemDto,
            };
            return cosmeticDto;
        }

        internal static LowerCostumePieceDto ItemDtoToCostumeLowerDto(ItemDto itemDto)
        {
            LowerCostumePieceDto cosmeticDto = new()
            {
                EarnedAt = Il2CppSystem.DateTime.Now,
                Item = itemDto,
            };
            return cosmeticDto;
        }

        internal static UpperCostumePieceDto ItemDtoToCostumeUpperDto(ItemDto itemDto)
        {
            UpperCostumePieceDto cosmeticDto = new()
            {
                EarnedAt = Il2CppSystem.DateTime.Now,
                Item = itemDto,
            };
            return cosmeticDto;
        }

        internal static MonolithicCostumeDto ItemDtoToFullCostumeDto(ItemDto itemDto)
        {
            MonolithicCostumeDto cosmeticDto = new()
            {
                EarnedAt = Il2CppSystem.DateTime.Now,
                Item = itemDto,
            };
            return cosmeticDto;
        }

        static EmoteDto ItemDtoToEmoteDto(ItemDto itemDto)
        {
            EmoteDto cosmeticDto = new()
            {
                EarnedAt = Il2CppSystem.DateTime.Now,
                Item = itemDto,
            };
            return cosmeticDto;
        }

        static NameplateDto ItemDtoToNameplateDto(ItemDto itemDto)
        {
            NameplateDto cosmeticDto = new()
            {
                EarnedAt = Il2CppSystem.DateTime.Now,
                Item = itemDto,
            };
            return cosmeticDto;
        }

        static PunchlineDto ItemDtoToVictoryDto(ItemDto itemDto)
        {
            PunchlineDto cosmeticDto = new()
            {
                EarnedAt = Il2CppSystem.DateTime.Now,
                Item = itemDto,
            };
            return cosmeticDto;
        }

        static NicknameDto ItemDtoToNicknameDto(ItemDto itemDto)
        {
            NicknameDto cosmeticDto = new()
            {
                EarnedAt = Il2CppSystem.DateTime.Now,
                Item = itemDto,
            };
            return cosmeticDto;
        }
    }
}
