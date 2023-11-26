using System.Data.Common;
using System.Diagnostics;
using System.Collections.Specialized;

using System.Runtime.Versioning;


namespace BlackHole.Ray
{

    sealed internal class RayConnectionFactory : DbConnectionFactory
    {
        private RayConnectionFactory() : base() { }
        // At this time, the Ray Provider doesn't have any connection pool counters
        // because we'd only confuse people with "non-pooled" connections that are
        // actually being pooled by the native pooler.

        private const string _MetaData = ":MetaDataXml";
        private const string _defaultMetaDataXml = "defaultMetaDataXml";

        public static readonly RayConnectionFactory SingletonInstance = new RayConnectionFactory();

        override public DbProviderFactory ProviderFactory
        {
            get
            {
                return RayFactory.Instance;
            }
        }

        override protected DbConnectionInternal CreateConnection(DbConnectionOptions options, DbConnectionPoolKey poolKey, object poolGroupProviderInfo, DbConnectionPool pool, DbConnection owningObject)
        {
            DbConnectionInternal result = new RayConnectionOpen(owningObject as RayConnection, options as RayConnectionString);
            return result;
        }

        override protected DbConnectionOptions CreateConnectionOptions(string connectionString, DbConnectionOptions previous)
        {
            Debug.Assert(!ADP.IsEmpty(connectionString), "empty connectionString");
            RayConnectionString result = new RayConnectionString(connectionString, (null != previous));
            return result;
        }

        override protected DbConnectionPoolGroupOptions CreateConnectionPoolGroupOptions(DbConnectionOptions connectionOptions)
        {
            // At this time, the Ray provider only supports native pooling so we
            // simply return NULL to indicate that.
            return null;
        }

        override internal DbConnectionPoolGroupProviderInfo CreateConnectionPoolGroupProviderInfo(DbConnectionOptions connectionOptions)
        {
            return new RayConnectionPoolGroupProviderInfo();
        }

        // SxS (VSDD 545786): metadata files are opened from <.NetRuntimeFolder>\CONFIG\<metadatafilename.xml>
        // this operation is safe in SxS because the file is opened in read-only mode and each NDP runtime accesses its own copy of the metadata
        // under the runtime folder.
        [ResourceExposure(ResourceScope.None)]
        [ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
        override protected DbMetaDataFactory CreateMetaDataFactory(DbConnectionInternal internalConnection, out bool cacheMetaDataFactory)
        {

            Debug.Assert(internalConnection != null, "internalConnection may not be null.");
            cacheMetaDataFactory = false;

            RayConnection RayOuterConnection = ((RayConnectionOpen)internalConnection).OuterConnection;
            Debug.Assert(RayOuterConnection != null, "outer connection may not be null.");

            NameValueCollection settings = (NameValueCollection)PrivilegedConfigurationManager.GetSection("system.data.Ray");
            Stream XMLStream = null;

            // get the DBMS Name
            object driverName = null;
            string stringValue = RayOuterConnection.GetInfoStringUnhandled(Ray32.SQL_INFO.DRIVER_NAME);
            if (stringValue != null)
            {
                driverName = stringValue;
            }

            if (settings != null)
            {

                string[] values = null;
                string metaDataXML = null;
                // first try to get the provider specific xml

                // if driver name is not supported we can't build the settings key needed to
                // get the provider specific XML path
                if (driverName != null)
                {
                    metaDataXML = ((string)driverName) + _MetaData;
                    values = settings.GetValues(metaDataXML);
                }

                // if we did not find provider specific xml see if there is new default xml
                if (values == null)
                {
                    metaDataXML = _defaultMetaDataXml;
                    values = settings.GetValues(metaDataXML);
                }

                // If there is an XML file get it
                if (values != null)
                {
                    XMLStream = ADP.GetXmlStreamFromValues(values, metaDataXML);
                }
            }

            // use the embedded xml if the user did not over ride it
            if (XMLStream == null)
            {
                XMLStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("System.Data.Ray.RayMetaData.xml");
                cacheMetaDataFactory = true;
            }

            Debug.Assert(XMLStream != null, "XMLstream may not be null.");

            String versionString = RayOuterConnection.GetInfoStringUnhandled(Ray32.SQL_INFO.DBMS_VER);

            return new RayMetaDataFactory(XMLStream,
                                            versionString,
                                            versionString,
                                            RayOuterConnection);
        }

        override internal DbConnectionPoolGroup GetConnectionPoolGroup(DbConnection connection)
        {
            RayConnection c = (connection as RayConnection);
            if (null != c)
            {
                return c.PoolGroup;
            }
            return null;
        }

        override internal DbConnectionInternal GetInnerConnection(DbConnection connection)
        {
            RayConnection c = (connection as RayConnection);
            if (null != c)
            {
                return c.InnerConnection;
            }
            return null;
        }

        override protected int GetObjectId(DbConnection connection)
        {
            RayConnection c = (connection as RayConnection);
            if (null != c)
            {
                return c.ObjectID;
            }
            return 0;
        }

        override internal void PermissionDemand(DbConnection outerConnection)
        {
            RayConnection c = (outerConnection as RayConnection);
            if (null != c)
            {
                c.PermissionDemand();
            }
        }

        override internal void SetConnectionPoolGroup(DbConnection outerConnection, DbConnectionPoolGroup poolGroup)
        {
            RayConnection c = (outerConnection as RayConnection);
            if (null != c)
            {
                c.PoolGroup = poolGroup;
            }
        }

        override internal void SetInnerConnectionEvent(DbConnection owningObject, DbConnectionInternal to)
        {
            RayConnection c = (owningObject as RayConnection);
            if (null != c)
            {
                c.SetInnerConnectionEvent(to);
            }
        }

        override internal bool SetInnerConnectionFrom(DbConnection owningObject, DbConnectionInternal to, DbConnectionInternal from)
        {
            RayConnection c = (owningObject as RayConnection);
            if (null != c)
            {
                return c.SetInnerConnectionFrom(to, from);
            }
            return false;
        }

        override internal void SetInnerConnectionTo(DbConnection owningObject, DbConnectionInternal to)
        {
            RayConnection c = (owningObject as RayConnection);
            if (null != c)
            {
                c.SetInnerConnectionTo(to);
            }
        }



    }
}
