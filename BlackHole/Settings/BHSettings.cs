namespace BlackHole.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BHSettings
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract BHAuditSettings AuditSettings(AuditSettingsBuilder auditSettingsBuilder);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract BHDefaultData DefaultData(DefaultDataBuilder defaultDataBuilder);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract BHStoredViews StoredViews(StoredViewsBuilder storedViewsBuilder);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract BHStoredProcedures StoredProcedures(StoredProceduresBuilder storedProceduresBuilder);
    }
}
