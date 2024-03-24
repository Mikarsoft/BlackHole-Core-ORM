
namespace BlackHole.Web
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BHWebConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract string UrlBase { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localBuilder"></param>
        /// <returns></returns>
        public abstract BHLocalizer CreateLocalization(IBHLocalizationBuilder localBuilder);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewsMapper"></param>
        /// <returns></returns>
        public abstract BHViewsMapping MapControllersAndViews(IBHViewsMapper viewsMapper);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="policyBuilder"></param>
        /// <returns></returns>
        public abstract BHAuthorizationConfig CreateAuthorizationPolicy(IBHPolicyBuilder policyBuilder);
    }
}
