using System;
using System.Collections.Generic;

namespace SimpleXAMLLocalizationHelper.Model
{
    public class SettingModel
    {
        public DateTime LAST_LOGIN { get; set; }
        public List<string> USE_LANGUAGES { get; set; }
        public Dictionary<string, string> FAVORITES { get; set; }
    }
}