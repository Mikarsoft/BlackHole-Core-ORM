namespace BlackHole.CoreSupport
{
    internal interface IBHDataProviderSelector
    {
        internal IDataProvider GetDataProvider(Type IdType, string tableName);
        internal IExecutionProvider GetExecutionProvider();
        internal string GetDatabaseSchema();
    }
}
