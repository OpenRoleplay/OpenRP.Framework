using OpenRP.Framework.Features.Skills.Components;
using OpenRP.Framework.Features.Skills.Entities;
using OpenRP.Framework.Shared;
using OpenRP.Framework.Shared.Chat.Enums;
using OpenRP.Framework.Shared.Chat.Extensions;
using OpenRP.Framework.Shared.Dialogs;
using OpenRP.Framework.Shared.Dialogs.Enums;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenRP.Framework.Features.Skills.Services.Dialogs
{
    public class SkillDialogService : ISkillDialogService
    {
        private const string NotUsedYet = "N/A";
        private const string LastUsedFormat = "yyyy-MM-dd HH:mm:ss";

        private readonly IDialogService _dialogService;

        public SkillDialogService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public void OpenSkills(Player player)
        {
            CharacterSkills? characterSkills = player.GetComponent<CharacterSkills>();
            if (characterSkills == null || !characterSkills.HasSkills())
            {
                player.SendPlayerInfoMessage(PlayerInfoMessageType.INFO, "Your character hasn't learned any skills yet.");
                return;
            }

            BetterTablistDialog dialog = new BetterTablistDialog("Details", "Close", 4);
            dialog.SetTitle(TitleType.Children, "Skills");
            dialog.AddHeaders("Skill", "Level", "XP", "Last Used");

            Dictionary<int, CharacterSkill> skillsByRow = new Dictionary<int, CharacterSkill>();
            foreach (CharacterSkill skill in characterSkills.GetSkills())
            {
                int row = dialog.AddRow(skill.Name, skill.Level.ToString(), GetExperienceText(skill), GetLastUsedText(skill));
                skillsByRow.Add(row, skill);
            }

            _dialogService.Show(player, dialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton && skillsByRow.TryGetValue(response.ItemIndex, out CharacterSkill? selectedSkill))
                {
                    OpenSkillDetails(player, selectedSkill);
                }
            });
        }

        public void OpenSkillDetails(Player player, CharacterSkill skill)
        {
            BetterMessageDialog dialog = new BetterMessageDialog("Back", "Close");
            dialog.SetTitle(TitleType.Children, "Skills", skill.Name);
            dialog.SetContent(GetSkillDetailsText(skill));

            _dialogService.Show(player, dialog, response =>
            {
                if (response.Response == DialogResponse.LeftButton)
                {
                    OpenSkills(player);
                }
            });
        }

        private static string GetSkillDetailsText(CharacterSkill skill)
        {
            StringBuilder content = new StringBuilder();
            AppendDetail(content, "Name", skill.Name);
            if (skill.HasDescription())
            {
                AppendDetail(content, "Description", skill.Description!);
            }
            AppendDetail(content, "Level", skill.Level.ToString());
            AppendDetail(content, "Experience", GetExperienceText(skill));
            AppendDetail(content, "Last Used", GetLastUsedText(skill));
            return content.ToString();
        }

        private static void AppendDetail(StringBuilder content, string label, string value)
        {
            content.AppendLine($"{ChatColor.CornflowerBlue}{label}:");
            content.AppendLine($"{ChatColor.White}{value}");
            content.AppendLine();
        }

        private static string GetExperienceText(CharacterSkill skill)
        {
            return $"{skill.Experience}{ChatColor.CornflowerBlue}/{ChatColor.White}{skill.GetExperienceForNextLevel()}";
        }

        private static string GetLastUsedText(CharacterSkill skill)
        {
            return skill.LastUsedDate?.ToString(LastUsedFormat) ?? NotUsedYet;
        }
    }
}
