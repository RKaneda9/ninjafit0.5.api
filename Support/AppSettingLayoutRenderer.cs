using System;
using NLog;
using NLog.Config;
using NLog.Internal;
using NLog.LayoutRenderers;
using System.Text;

namespace NinjaFit.Api.Support
{
    /// <summary>
	/// Application setting.
	/// </summary>
	/// <remarks>
	/// Use this layout renderer to insert the value of an application setting
	/// stored in the application's App.config or Web.config file.
	/// </remarks>
	/// <code lang="NLog Layout Renderer">
	/// ${appsetting:name=mysetting:default=mydefault} - produces "mydefault" if no appsetting
	/// </code>
	[LayoutRenderer("appsetting")]
    public class AppSettingLayoutRenderer : LayoutRenderer
    {
        private IConfigurationManager configurationManager = new ConfigurationManager();

        ///<summary>
        /// The AppSetting name.
        ///</summary>
        [RequiredParameter]
        [DefaultParameter]
        public string Name { get; set; }

        ///<summary>
        /// The default value to render if the AppSetting value is null.
        ///</summary>
        public string Default { get; set; }

        internal IConfigurationManager ConfigurationManager
        {
            get { return configurationManager; }
            set { configurationManager = value; }
        }

        /// <summary>
        /// Renders the specified application setting or default value and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (Name == null)
                return;

            string value = AppSettingValue;
            if (value == null && Default != null)
                value = Default;

            if (string.IsNullOrEmpty(value) == false)
                builder.Append(value);
        }

        private bool _cachedAppSettingValue;
        private string _appSettingValue;

        private string AppSettingValue
        {
            get
            {
                if (_cachedAppSettingValue == false)
                {
                    _appSettingValue = ConfigurationManager.AppSettings[Name];
                    if (string.IsNullOrEmpty(_appSettingValue) == false && _appSettingValue.StartsWith("~"))
                    {
                        _appSettingValue = AppDomain.CurrentDomain.BaseDirectory + _appSettingValue.Substring(1);
                    }
                    _cachedAppSettingValue = true;
                }
                return _appSettingValue;
            }
        }
    }
}
