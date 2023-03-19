namespace BlackHole.CoreSupport
{
    internal interface IBHDataProviderSelector
    {
        internal IDataProvider GetDataProvider(Type IdType);
        internal IExecutionProvider GetExecutionProvider();
    }
}
