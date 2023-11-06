using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using Klyte.AirplaneLineTool.Extensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AlgernonCommons;
using UnityEngine;
using AlgernonCommons.UI;

[assembly: AssemblyVersion("1.2.0.*")]
namespace Klyte.AirplaneLineTool
{
    public class AirplaneLineToolMod : LoadingExtensionBase, IUserMod
    {
        private static bool _localizationInitialized = false;

        public static string version
        {
            get
            {
                return majorVersion + "." + typeof(AirplaneLineToolMod).Assembly.GetName().Version.Build;
            }
        }

        public static string majorVersion
        {
            get
            {
                return typeof(AirplaneLineToolMod).Assembly.GetName().Version.Major + "." + typeof(AirplaneLineToolMod).Assembly.GetName().Version.Minor;
            }
        }

        public static string fullVersion
        {
            get
            {
                return version + " r" + typeof(AirplaneLineToolMod).Assembly.GetName().Version.Revision;
            }
        }

        public string Name
        {
            get
            {
                return "Airplane Line Renewal";
            }
        }

        public string Description
        {
            get { return "Allows you to create airplane lines in the city."; }
        }

        private GameObject _planeGameObject;

        #region LoadingExtensionBase
        public override void OnLevelLoaded(LoadMode mode)
        {
            InstallLocalization();
            _planeGameObject = CreateTransportLineGo("Airplane", "PublicTransportAirportArea");
        }

        private static UITextureAtlas GetAtlas(string name)
        {
            UITextureAtlas[] atlases = Resources.FindObjectsOfTypeAll(typeof(UITextureAtlas)) as UITextureAtlas[];
            for (int i = 0; i < atlases.Length; i++)
            {
                if (atlases[i].name == name)
                    return atlases[i];
            }
            return UIView.GetAView().defaultAtlas;
        }

        UITextureAtlas ToolTipAtlas = GetAtlas("tooltiplargestand");

        public override void OnLevelUnloading()
        {
            if (_planeGameObject != null)
                GameObject.Destroy(_planeGameObject);
        }
        #endregion

        private static T GetPrivate<T>(object o, string fieldName)
        {
            var fields = o.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field = null;

            foreach (var f in fields)
            {
                if (f.Name == fieldName)
                {
                    field = f;
                    break;
                }
            }

            return (T)field.GetValue(o);
        }

        private void InstallLocalization()
        {
            if (_localizationInitialized)
                return;

            GlobalVariables.CodeLogger.Log("Updating Localization.");

            try
            {
                var locale = GetPrivate<Locale>(LocaleManager.instance, "m_Locale");
                Locale.Key k;

                // Airplane locale
                string keyIdentifier = "TRANSPORT_LINE_AIRPLANE"; // Adjust the identifier as needed

                k = new Locale.Key()
                {
                    m_Identifier = keyIdentifier,
                    m_Key = "Airplane"
                };
                if (!locale.Exists(k)) // Check if the key already exists
                {
                    locale.AddLocalizedString(k, "Plane");
                }

                // Add more keys as needed

                _localizationInitialized = true;
                GlobalVariables.CodeLogger.Log("Localization successfully updated.");
            }
            catch (ArgumentException e)
            {
                GlobalVariables.CodeLogger.Log("Unexpected " + e.GetType().Name + " updating localization: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
            }
        }


        private GameObject CreateTransportLineGo(string transportInfoName, string category)
        {
            GameObject result = null;
            try
            {


                var busTransportInfo = PrefabCollection<TransportInfo>.FindLoaded("Bus");



                var planeLinePrefab = PrefabCollection<TransportInfo>.FindLoaded(transportInfoName);
                planeLinePrefab.m_lineMaterial2 = GameObject.Instantiate(busTransportInfo.m_lineMaterial2);
                planeLinePrefab.m_lineMaterial2.shader = planeLinePrefab.m_pathMaterial2.shader;
                planeLinePrefab.m_Atlas = ToolTipAtlas;
                planeLinePrefab.m_Thumbnail = "UIFilterPlaneHubFocused";
                planeLinePrefab.m_InfoTooltipAtlas = ToolTipAtlas;
                planeLinePrefab.m_InfoTooltipThumbnail = "tooltipmediumstand";



                planeLinePrefab.m_lineMaterial = GameObject.Instantiate(busTransportInfo.m_lineMaterial);
                planeLinePrefab.m_lineMaterial.shader = planeLinePrefab.m_pathMaterial.shader;
                planeLinePrefab.m_prefabDataLayer = 0;



                // Workaround for button/panel bug when you return to main menu and then load a map again.
                Transform scrollPanel = null;
                PublicTransportPanel transportPanel = null;
                var items = GameObject.FindObjectsOfType<PublicTransportPanel>();
                foreach (var item in items)
                {
                    if (item.category == category)
                    {
                        scrollPanel = item.transform.Find("ScrollablePanel");
                        transportPanel = item;
                        break;
                    }
                }

                // This creates the button and adds the functionality.
                var methodInfo = transportPanel.GetType().BaseType.GetMethod("CreateAssetItem", BindingFlags.NonPublic | BindingFlags.Instance);
                methodInfo.Invoke(transportPanel, new object[] { planeLinePrefab });

                // Find the newly created button and assign it to the return value so we can destroy it on level unload.
                result = scrollPanel.Find(transportInfoName).gameObject;

               
            }
            catch (Exception e)
            {
                
            }
            return result;
        }
    }

}