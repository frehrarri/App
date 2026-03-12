using Microsoft.EntityFrameworkCore;
using Voyage.Data;
using Voyage.Data.TableModels;

namespace Voyage.Utilities
{
    public class SectionSeeder
    {
        private _AppDbContext _db;
        private const int SETTINGS_SEED = -1;

        public SectionSeeder(_AppDbContext db)
        {
            _db = db;
        }

        public async Task CreateGlobalSections()
        {
            //create new settings
            var settingExists = await _db.Settings.AnyAsync(s => s.SettingsId == SETTINGS_SEED);
            if (!settingExists)
            {
                var setting = new Settings
                {
                    SettingsKey = Guid.NewGuid(),
                    SettingsId = -1,
                    Feature = Constants.Feature.Tickets,
                    RepeatSprintOption = -1,
                    SprintStartDate = null,
                    SprintEndDate = null,
                    SprintId = -1,
                    SectionSetting = -1,
                    CreatedBy = "SYSTEM",
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    IsLatest = true,
                };

                await _db.Settings.AddAsync(setting);
                await _db.SaveChangesAsync();
            }


            //create new sections
            int i = -1;
            int sectionOrder = 991;
            var requiredSections = Enum.GetValues<Constants.RequiredTicketSections>();

            foreach (var section in requiredSections)
            {
                var sectionName = section.ToString();
                
                var sectionExists = await _db.Settings.AnyAsync(s => s.SettingsId == SETTINGS_SEED && s.Sections.Any(se => se.Title == sectionName));
                if (!sectionExists)
                {
                    var newSection = new Section
                    {
                        SectionId = i,
                        Title = sectionName,
                        SectionOrder = sectionOrder++,
                        SettingsId = -1,
                        CreatedBy = "SYSTEM",
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true,
                        IsLatest = true,
                    };

                    await _db.Sections.AddAsync(newSection);
                    i--;
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}
