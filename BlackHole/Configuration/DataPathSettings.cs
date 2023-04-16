
namespace BlackHole.Configuration
{
    /// <summary>
    /// Settings for the path of the BlackHole logs and Sqlite database.
    /// </summary>
    public class DataPathSettings
    {
        internal string DataPath { get; set; } = string.Empty;
        internal int DaysForCleanUp { get; set; } = 60;
        internal bool UseLogsCleaner { get; set; } = true;

        /// <summary>
        /// Set the path of the folder where BlackHole will store
        /// Sqlite databases, Logs and other data that will
        /// be required for the features in later updates
        /// </summary>
        /// <param name="dataPath">Full path of the data folder</param>
        public void SetDataPath(string dataPath)
        {
            DataPath = dataPath;
        }

        /// <summary>
        /// Set the path of the folder where BlackHole will store
        /// Sqlite databases, Logs and other data that will
        /// be required for the features in later updates
        /// </summary>
        /// <param name="dataPath">Full path of the data folder</param>
        /// <param name="useLogsCleaner">Choose if the logs will be automatically cleaned</param>
        public void SetDataPath(string dataPath, bool useLogsCleaner)
        {
            UseLogsCleaner = useLogsCleaner;
            DataPath = dataPath;
        }

        /// <summary>
        /// Set the path of the folder where BlackHole will store
        /// Sqlite databases, Logs and other data that will
        /// be required for the features in later updates
        /// </summary>
        /// <param name="dataPath">Full path of the data folder</param>
        /// <param name="cleanUpDays">Choose the age of the logs before they get deleted</param>
        public void SetDataPath(string dataPath, int cleanUpDays)
        {
            DataPath = dataPath;
            DaysForCleanUp = cleanUpDays;
        }
    }
}
