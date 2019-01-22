using ColossalFramework;
using ColossalFramework.DataBinding;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.ServiceVehiclesManager.i18n;
using Klyte.ServiceVehiclesManager.Utils;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("2.0.0.0")]

namespace Klyte.ServiceVehiclesManager
{
    public class ServiceVehiclesManagerMod : MonoBehaviour, IUserMod, ILoadingExtension
    {

        public static string minorVersion => majorVersion + "." + typeof(ServiceVehiclesManagerMod).Assembly.GetName().Version.Build;
        public static string majorVersion => typeof(ServiceVehiclesManagerMod).Assembly.GetName().Version.Major + "." + typeof(ServiceVehiclesManagerMod).Assembly.GetName().Version.Minor;
        public static string fullVersion => minorVersion + " r" + typeof(ServiceVehiclesManagerMod).Assembly.GetName().Version.Revision;
        public static string version
        {
            get {
                if (typeof(ServiceVehiclesManagerMod).Assembly.GetName().Version.Minor == 0 && typeof(ServiceVehiclesManagerMod).Assembly.GetName().Version.Build == 0)
                {
                    return typeof(ServiceVehiclesManagerMod).Assembly.GetName().Version.Major.ToString();
                }
                if (typeof(ServiceVehiclesManagerMod).Assembly.GetName().Version.Build > 0)
                {
                    return minorVersion;
                }
                else
                {
                    return majorVersion;
                }
            }
        }


        public static ServiceVehiclesManagerMod instance;

        private SavedBool m_debugMode;
        public bool needShowPopup;
        private static bool isLocaleLoaded = false;


        private SavedBool m_allowOutsidersAsDefault;
        private SavedBool m_allowGoOutsideAsDefault;

        public static bool LocaleLoaded => isLocaleLoaded;

        private string currentSelectedConfigEditor => currentCityId;

        private static bool m_isKlyteCommonsLoaded = false;
        public static bool IsKlyteCommonsEnabled()
        {
            if (!m_isKlyteCommonsLoaded)
            {
                try
                {
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var assembly = (from a in assemblies
                                    where a.GetType("Klyte.Commons.KlyteCommonsMod") != null
                                    select a).SingleOrDefault();
                    if (assembly != null)
                    {
                        m_isKlyteCommonsLoaded = true;
                    }
                }
                catch { }
            }
            return m_isKlyteCommonsLoaded;
        }

        public static bool debugMode => instance.m_debugMode.value;
        public static bool allowOutsidersAsDefault => instance.m_allowOutsidersAsDefault.value;
        public static bool allowGoOutsideAsDefault => instance.m_allowGoOutsideAsDefault.value;

        private SavedString currentSaveVersion => new SavedString("SVMSaveVersion", Settings.gameSettingsFile, "null", true);

        private SavedInt currentLanguageId => new SavedInt("SVMLanguage", Settings.gameSettingsFile, 0, true);

        internal SVMConfigWarehouse currentLoadedCityConfig => SVMConfigWarehouse.getConfig(currentCityId, currentCityName);

        public static bool isCityLoaded => Singleton<SimulationManager>.instance.m_metaData != null;

        private string currentCityId => isCityLoaded ? Singleton<SimulationManager>.instance.m_metaData.m_gameInstanceIdentifier : SVMConfigWarehouse.GLOBAL_CONFIG_INDEX;

        private string currentCityName => isCityLoaded ? Singleton<SimulationManager>.instance.m_metaData.m_CityName : SVMConfigWarehouse.GLOBAL_CONFIG_INDEX;

        private SVMConfigWarehouse currentConfigWarehouseEditor => SVMConfigWarehouse.getConfig(currentSelectedConfigEditor, currentCityName);

        private string[] getOptionsForLoadConfig() => currentCityId == SVMConfigWarehouse.GLOBAL_CONFIG_INDEX ? new string[] { SVMConfigWarehouse.GLOBAL_CONFIG_INDEX } : new string[] { currentCityName, SVMConfigWarehouse.GLOBAL_CONFIG_INDEX };

        public string Name => "Services Vehicles Manager " + version;

        public string Description => "Extension for managing the service vehicles. Requires Klyte Commons.";

        public void OnCreated(ILoading loading)
        {
        }

        public ServiceVehiclesManagerMod()
        {

            Debug.LogWarningFormat("SVMv" + majorVersion + " LOADING SVM ");
            SettingsFile svmSettings = new SettingsFile
            {
                fileName = SVMConfigWarehouse.CONFIG_FILENAME
            };
            Debug.LogWarningFormat("SVMv" + majorVersion + " SETTING FILES");
            try
            {
                GameSettings.AddSettingsFile(svmSettings);
            }
            catch (Exception e)
            {
                SettingsFile tryLoad = GameSettings.FindSettingsFileByName(SVMConfigWarehouse.CONFIG_FILENAME);
                if (tryLoad == null)
                {
                    Debug.LogErrorFormat("SVMv" + majorVersion + " SETTING FILES FAIL!!! ");
                    Debug.LogError(e.Message);
                    Debug.LogError(e.StackTrace);
                }
                else
                {
                    svmSettings = tryLoad;
                }
            }
            m_debugMode = new SavedBool("SVMdebugMode", Settings.gameSettingsFile, false, true);
            if (m_debugMode.value)
                Debug.LogWarningFormat("currentSaveVersion.value = {0}, fullVersion = {1}", currentSaveVersion.value, fullVersion);
            if (currentSaveVersion.value != fullVersion)
            {
                needShowPopup = true;
            }
            LocaleManager.eventLocaleChanged += new LocaleManager.LocaleChangedHandler(autoLoadSVMLocale);
            if (instance != null) { Destroy(instance); }
            m_allowGoOutsideAsDefault = new SavedBool("SVMAllowGoOutsideAsDefault", Settings.gameSettingsFile, true, true);
            m_allowOutsidersAsDefault = new SavedBool("SVMAllowOutsidersAsDefault", Settings.gameSettingsFile, true, true);
            instance = this;
        }

        public void OnSettingsUI(UIHelperBase helperDefault)
        {
            UIHelperExtension helper = new UIHelperExtension((UIHelper)helperDefault);
            void ev()
            {
                foreach (Transform child in helper.self.transform)
                {
                    GameObject.Destroy(child?.gameObject);
                }

                helper.self.eventVisibilityChanged += delegate (UIComponent component, bool b)
                {
                    if (b)
                    {
                        showVersionInfoPopup();
                    }
                };
                UIHelperExtension group8 = helper.AddGroupExtended(Locale.Get("SVM_DISTRICT_SERVICE_RESTRICTIONS"));
                group8.AddCheckboxLocale("SVM_DEFAULT_ALLOW_OUTSIDERS", m_allowOutsidersAsDefault.value, (x) => {  m_allowOutsidersAsDefault.value = x; });
                group8.AddCheckboxLocale("SVM_DEFAULT_ALLOW_GO_OUTSIDE", m_allowGoOutsideAsDefault.value, (x) => { m_allowGoOutsideAsDefault.value = x; });
                group8.AddLabel(Locale.Get("SVM_DEFAULT_RESTRICTIONS_NOTE2")).textColor = Color.white;
                group8.AddLabel(Locale.Get("SVM_DEFAULT_RESTRICTIONS_NOTE")).textColor = Color.yellow;
                group8.AddLabel(Locale.Get("SVM_DEFAULT_RESTRICTIONS_NOTE3")).textColor = Color.red;

                UIHelperExtension group9 = helper.AddGroupExtended(Locale.Get("SVM_BETAS_EXTRA_INFO"));
                group9.AddDropdownLocalized("SVM_MOD_LANG", SVMLocaleUtils.getLanguageIndex(), currentLanguageId.value, delegate (int idx)
                {
                    currentLanguageId.value = idx;
                    loadSVMLocale(true);
                });
                group9.AddCheckbox(Locale.Get("SVM_DEBUG_MODE"), m_debugMode.value, delegate (bool val) { m_debugMode.value = val; });
                group9.AddLabel("Version: " + fullVersion);
                group9.AddLabel(Locale.Get("SVM_ORIGINAL_TLM_VERSION") + " " + string.Join(".", ResourceLoader.loadResourceString("TLMVersion.txt").Split(".".ToCharArray()).Take(3).ToArray()));
                group9.AddButton(Locale.Get("SVM_RELEASE_NOTES"), delegate ()
                {
                    showVersionInfoPopup(true);
                });
                //UILabel lblResult = null;
                //group9.AddButton("TEST SUBTYPE", delegate ()
                // {
                //     var subclasses = SVMUtils.GetSubtypesRecursive(typeof(BasicBuildingAIOverrides<,>), typeof(BasicBuildingAIOverrides<,>));
                //     lblResult.text = string.Format("GetOverride pr� - subclasses:\r\n\t{0}", string.Join("\r\n\t", subclasses?.Select(x => x.ToString())?.ToArray() ?? new string[0]));
                // });
                //lblResult = group9.AddLabel("XXX1");
                //string testString = "";
                //for (int i = 0; i < 360; i++)
                //{
                //    var angle = Vector2.zero.GetAngleToPoint(new Vector2(Mathf.Sin(i * Mathf.Deg2Rad), Mathf.Cos(i * Mathf.Deg2Rad)));
                //    testString += $"{i:n0}� => {angle:n1} ({Mathf.Sin(i * Mathf.Deg2Rad):n3}, {Mathf.Cos(i * Mathf.Deg2Rad):n3})\n";
                //}
                //group9.AddLabel("TST:\n" + testString);
                SVMUtils.doLog("End Loading Options");
            }
            if (IsKlyteCommonsEnabled())
            {
                loadSVMLocale(false);
                ev();
            }
            else
            {
                eventOnLoadLocaleEnd = null;
                eventOnLoadLocaleEnd += ev;
            }
        }

        public bool showVersionInfoPopup(bool force = false)
        {
            if (needShowPopup || force)
            {
                try
                {
                    UIComponent uIComponent = UIView.library.ShowModal("ExceptionPanel");
                    if (uIComponent != null)
                    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        BindPropertyByKey component = uIComponent.GetComponent<BindPropertyByKey>();
                        if (component != null)
                        {
                            string title = "Service Vehicles Manager v" + version;
                            string notes = ResourceLoader.loadResourceString("UI.VersionNotes.txt");
                            string text = "Service Vehicles Manager was updated! Release notes:\r\n\r\n" + notes;
                            string img = "IconMessage";
                            component.SetProperties(TooltipHelper.Format(new string[]
                            {
                            "title",
                            title,
                            "message",
                            text,
                            "img",
                            img
                            }));
                            needShowPopup = false;
                            currentSaveVersion.value = fullVersion;
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        SVMUtils.doLog("PANEL NOT FOUND!!!!");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    SVMUtils.doErrorLog("showVersionInfoPopup ERROR {0} {1}", e.GetType(), e.Message);
                }
            }
            return false;
        }

        public void autoLoadSVMLocale()
        {
            if (currentLanguageId.value == 0)
            {
                loadSVMLocale(false);
            }
        }
        public void loadSVMLocale(bool force)
        {
            if (SingletonLite<LocaleManager>.exists && IsKlyteCommonsEnabled())
            {
                SVMLocaleUtils.loadLocale(currentLanguageId.value == 0 ? SingletonLite<LocaleManager>.instance.language : SVMLocaleUtils.getSelectedLocaleByIndex(currentLanguageId.value), force);
                if (!isLocaleLoaded)
                {
                    isLocaleLoaded = true;
                    eventOnLoadLocaleEnd?.Invoke();
                }
            }
        }
        private delegate void OnLocaleLoadedFirstTime();
        private event OnLocaleLoadedFirstTime eventOnLoadLocaleEnd;

        private bool m_loaded = false;

        public void OnLevelLoaded(LoadMode mode)
        {
            SVMUtils.doLog("LEVEL LOAD");
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && mode != LoadMode.NewGameFromScenario)
            {
                m_loaded = false;
                SVMUtils.doLog("NOT GAME ({0})", mode);
                return;
            }
            if (!IsKlyteCommonsEnabled())
            {
                throw new Exception("SVM requires Klyte Commons active!");
            }
            if (SVMController.taSVM == null)
            {
                SVMController.taSVM = CreateTextureAtlas("UI.Images.sprites.png", "ServiceVehicleManagerSprites", GameObject.FindObjectOfType<UIView>().FindUIComponent<UIPanel>("InfoPanel").atlas.material, 64, 64, new string[] {
                    "ServiceVehiclesManagerIcon","ServiceVehiclesManagerIconSmall","ToolbarIconGroup6Hovered","ToolbarIconGroup6Focused","HelicopterIndicator","RemoveUnwantedIcon","CargoIndicator", "OutsideIndicator", "BioIndicator"
                });
            }
            loadSVMLocale(false);

            SVMController.instance.Awake();

            m_loaded = true;
        }

        public void OnLevelUnloading()
        {
            if (!m_loaded) { return; }
            if (SVMController.instance != null)
            {
                Destroy(SVMController.instance);
            }
            //			Log.debug ("LEVELUNLOAD");
            m_loaded = false;
        }

        public void OnReleased()
        {

        }

        UITextureAtlas CreateTextureAtlas(string textureFile, string atlasName, Material baseMaterial, int spriteWidth, int spriteHeight, string[] spriteNames)
        {
            Texture2D tex = new Texture2D(spriteWidth * spriteNames.Length, spriteHeight, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear
            };
            { // LoadTexture
                tex.LoadImage(ResourceLoader.loadResourceData(textureFile));
                tex.Apply(true, true);
            }
            UITextureAtlas atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            { // Setup atlas
                Material material = (Material)Material.Instantiate(baseMaterial);
                material.mainTexture = tex;
                atlas.material = material;
                atlas.name = atlasName;
            }
            // Add sprites
            for (int i = 0; i < spriteNames.Length; ++i)
            {
                float uw = 1.0f / spriteNames.Length;
                var spriteInfo = new UITextureAtlas.SpriteInfo()
                {
                    name = spriteNames[i],
                    texture = tex,
                    region = new Rect(i * uw, 0, uw, 1),
                };
                atlas.AddSprite(spriteInfo);
            }
            return atlas;
        }

    }

}
