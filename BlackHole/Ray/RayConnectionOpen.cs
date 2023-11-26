using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Ray
{
    sealed internal class RayConnectionOpen : DbConnectionInternal
    {

        // Construct from a compiled connection string
        internal RayConnectionOpen(RayConnection outerConnection, RayConnectionString connectionOptions)
        {
#if DEBUG
            try
            { // use this to help validate this object is only created after the following permission has been previously demanded in the current codepath
                if (null != outerConnection)
                {
                    outerConnection.UserConnectionOptions.DemandPermission();
                }
                else
                {
                    connectionOptions.DemandPermission();
                }
            }
            catch (System.Security.SecurityException)
            {
                System.Diagnostics.Debug.Assert(false, "unexpected SecurityException for current codepath");
                throw;
            }
#endif
            RayEnvironmentHandle environmentHandle = RayEnvironment.GetGlobalEnvironmentHandle();
            outerConnection.ConnectionHandle = new RayConnectionHandle(outerConnection, connectionOptions, environmentHandle);
        }

        internal RayConnection OuterConnection
        {
            get
            {
                RayConnection outerConnection = (RayConnection)Owner;

                if (null == outerConnection)
                    throw Ray.OpenConnectionNoOwner();

                return outerConnection;
            }
        }

        override public string ServerVersion
        {
            get
            {
                return OuterConnection.Open_GetServerVersion();
            }
        }

        override protected void Activate(SysTx.Transaction transaction)
        {
            RayConnection.ExecutePermission.Demand();
        }

        override public DbTransaction BeginTransaction(IsolationLevel isolevel)
        {
            return BeginRayTransaction(isolevel);
        }

        internal RayTransaction BeginRayTransaction(IsolationLevel isolevel)
        {
            return OuterConnection.Open_BeginTransaction(isolevel);
        }

        override public void ChangeDatabase(string value)
        {
            OuterConnection.Open_ChangeDatabase(value);
        }

        override protected DbReferenceCollection CreateReferenceCollection()
        {
            return new RayReferenceCollection();
        }

        override protected void Deactivate()
        {
            NotifyWeakReference(RayReferenceCollection.Closing);
        }

        override public void EnlistTransaction(SysTx.Transaction transaction)
        {
            OuterConnection.Open_EnlistTransaction(transaction);
        }
    }
