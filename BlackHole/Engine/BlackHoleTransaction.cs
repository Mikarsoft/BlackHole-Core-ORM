using System.Data;

namespace BlackHole.Engine
{
    /// <summary>
    /// Transaction Object
    /// </summary>
    internal class BlackHoleTransaction : IDisposable
    {
        private bool committed = false;
        internal bool hasError = false;
        private bool pendingRollback = false;

        /// <summary>
        /// 
        /// </summary>
        public ConnectionReference[] connections;

        internal BlackHoleTransaction(int availableConnections)
        {
            connections = new ConnectionReference[availableConnections];
        }

        internal IDbConnection GetConnection(int connectionIndex)
        {
            if (connections[connectionIndex].IsOpen)
            {
                return connections[connectionIndex].Connection;
            }

            connections[connectionIndex] = new(connectionIndex);
            return connections[connectionIndex].Connection;
        }

        internal IDbTransaction GetTransaction(int connectionIndex)
        {
            if (connections[connectionIndex].IsOpen)
            {
                return connections[connectionIndex].Transaction;
            }

            connections[connectionIndex] = new(connectionIndex);
            return connections[connectionIndex].Transaction;
        }

        internal bool Commit()
        {
            return CommitAll();
        }

        private bool CommitAll()
        {
            if (!committed)
            {
                committed = true;

                if (hasError)
                {
                    RollBackAll();
                    return false;
                }

                foreach(ConnectionReference connectionRef in connections)
                {
                    if (connectionRef.IsOpen)
                    {
                        connectionRef.Transaction.Commit();
                    }
                }

                return committed;
            }
            return false;
        }

        internal bool DoNotCommit()
        {
            if (!committed)
            {
                committed = true;
                pendingRollback = true;
                return true;
            }

            return false;
        }

        internal bool RollBack()
        {
            return RollBackAll();
        }

        private bool RollBackAll()
        {
            if (!committed || pendingRollback)
            {
                foreach(ConnectionReference connectionRef in connections)
                {
                    if (connectionRef.IsOpen)
                    {
                        connectionRef.Transaction.Rollback();
                    }
                }

                hasError = false;
                return true;
            }
 
            return false;
        }

        /// <summary>
        /// Commit uncommitted transaction. Dispose the connection and the transaction
        /// </summary>
        public void Dispose()
        {
            if (!committed)
            {
                if (hasError)
                {
                    RollBackAll();
                }
                else
                {
                   CommitAll();
                }
            }

            if (pendingRollback)
            {
                RollBackAll();
            }
            
            foreach(ConnectionReference connectionRef in connections)
            {
                connectionRef.Dispose();
            }
        }
    }
}
