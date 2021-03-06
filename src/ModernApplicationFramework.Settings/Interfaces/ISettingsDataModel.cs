using System;
using System.ComponentModel;

namespace ModernApplicationFramework.Settings.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    /// Provides the data model that wants to hold storable settings
    /// </summary>
    public interface ISettingsDataModel : INotifyPropertyChanged
    {
        event EventHandler SettingsChanged;


        /// <summary>
        /// The category of the data model
        /// </summary>
        ISettingsCategory Category { get; }

        /// <summary>
        /// The name of the data model
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The path of the setting inside the document
        /// </summary>
        string SettingsFilePath { get; }

        /// <summary>
        /// Loads all settings entries from the settings file or creates them if they don't exist.
        /// </summary>
        void LoadOrCreate();

        /// <summary>
        /// Stores all settings into the Settings file. 
        /// <remarks>This should not write the file to disk due to performance and possible mutexes.</remarks>
        /// </summary>
        void StoreSettings();
    }
}