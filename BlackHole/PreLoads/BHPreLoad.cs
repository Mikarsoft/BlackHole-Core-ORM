namespace BlackHole.PreLoads
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BHPreLoad
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
        public abstract void StoredViews(StoredViewsBuilder storedViewsBuilder);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract BHStoredProcedures StoredProcedures(StoredProceduresBuilder storedProceduresBuilder);
    }
}
