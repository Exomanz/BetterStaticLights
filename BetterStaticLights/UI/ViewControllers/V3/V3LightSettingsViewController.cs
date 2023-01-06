using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using BetterStaticLights.Configuration;
using SiraUtil.Logging;
using System.Collections.Generic;
using Zenject;

namespace BetterStaticLights.UI.ViewControllers.V3
{
    [ViewDefinition("BetterStaticLights.UI.BSML.v3settings.bsml")]
    [HotReload(RelativePathToLayout = "../../BSML/v3settings.bsml")]
    public class V3LightSettingsViewController : BSMLAutomaticViewController
    {
        [UIParams] private readonly BSMLParserParams parser;
        [Inject] private readonly MockSceneTransitionHelper transitionHelper;
        [Inject] private readonly SiraLog logger;

        private PreviewerConfigurationData previewerConfigurationData;

        [UIComponent("groupid-list-component")]
        public DropDownListSetting idIntegerList;

        [UIValue("groupids")]
        public List<object> groupIds = new List<object>() { "0" };

        [UIValue("preview-groupid-setting")]
        public string groupIdSetting;

        [Inject]
        internal void Construct(PluginConfig config)
        {
            this.previewerConfigurationData = config.previewerConfigurationData;
            transitionHelper.previewerDidFinishEvent += HandlePreviewerDidFinishEvent;
        }

        public void HandlePreviewerDidFinishEvent(bool state)
        {
            this.gameObject.SetActive(state);
            if (state)
            {
                idIntegerList.values = this.PopulateLightGroupIdList();
                idIntegerList.UpdateChoices();
                idIntegerList.dropdown.SelectCellWithIdx(0);
                groupIdSetting = "0";
            }
        }

        private List<object> PopulateLightGroupIdList()
        {
            List<object> list = new List<object>();
            for (int i = 0; i < transitionHelper.environmentLightGroups?.Count; i++)
            {
                list.Add(i.ToString());
            }

            return list;
        }
    }
}
