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
        public abstract void DefaultData(IDefaultDataBuilder defaultDataBuilder);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract void StoredViews(IStoredViewsBuilder storedViewsBuilder);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract void StoredProcedures(IStoredProceduresBuilder storedProceduresBuilder);
    }
}
