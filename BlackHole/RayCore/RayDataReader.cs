using System.Collections;
using System.Data;
using System.Data.Common;

namespace BlackHole.RayCore
{
    internal class RayDataReader : DbDataReader
    {
        private bool _isClosed;
        private CommandBehavior _commandBehavior;
        private DbCache? dataCache;
        private bool _noMoreResults;
        private RayCMDWrapper _cmdWrapper;
        private RayCommand command;


        public override object this[int ordinal] => throw new NotImplementedException();

        public override object this[string name] => throw new NotImplementedException();

        public override int Depth => IsClosed ? throw RS.ReaderClosed("Depth") : 0;

        public override int FieldCount
        {
            get
            {
                if (IsClosed)
                {
                    throw RS.ReaderClosed("FieldCount");
                }
                if (_noMoreResults)
                {
                    return 0;
                }
                if (dataCache == null)
                {
                    short cColsAffected;
                    Ray64.RetCode retcode = FieldCountNoThrow(out cColsAffected);
                    if (retcode != Ray64.RetCode.SUCCESS)
                    {
                        Connection?.HandleError(StatementHandle, retcode);
                    }
                }
                return ((null != this.dataCache) ? this.dataCache._count : 0);
            }
        }

        private RayStatementHandle StatementHandler
        {
            get
            {
                return _cmdWrapper.StatementHandle;
            }
        }

        internal bool IsCancelingCommand
        {
            get
            {
                if (command != null)
                {
                    return command.Canceling;
                }
                return false;
            }
        }

        internal bool IsBehavior(CommandBehavior behavior)
        {
            return IsCommandBehavior(behavior);
        }

        private bool IsCommandBehavior(CommandBehavior condition)
        {
            return (condition == (condition & _commandBehavior));
        }

        internal Ray64.RetCode FieldCountNoThrow(out short cColsAffected)
        {
            if (IsCancelingCommand)
            {
                cColsAffected = 0;
                return Ray64.RetCode.ERROR;
            }

            Ray64.RetCode retcode = StatementHandler.NumberOfResultColumns(out cColsAffected);
            if (retcode == Ray64.RetCode.SUCCESS)
            {
                _hiddenColumns = 0;
                if (IsCommandBehavior(CommandBehavior.KeyInfo))
                {
                    // we need to search for the first hidden column
                    //
                    if (!Connection.ProviderInfo.NoSqlSoptSSNoBrowseTable && !Connection.ProviderInfo.NoSqlSoptSSHiddenColumns)
                    {
                        for (int i = 0; i < cColsAffected; i++)
                        {
                            SQLLEN isHidden = GetColAttribute(i, (Ray32.SQL_DESC)Ray32.SQL_CA_SS.COLUMN_HIDDEN, (Ray32.SQL_COLUMN)(-1), Ray32.HANDLER.IGNORE);
                            if (isHidden.ToInt64() == 1)
                            {
                                _hiddenColumns = (int)cColsAffected - i;
                                cColsAffected = (Int16)i;
                                break;
                            }
                        }
                    }
                }
                this.dataCache = new DbCache(this, cColsAffected);
            }
            else
            {
                cColsAffected = 0;
            }
            return retcode;
        }

        private RayConnection? Connection
        {
            get
            {
                if (null != _cmdWrapper)
                {
                    return _cmdWrapper.Connection;
                }
                else
                {
                    return null;
                }
            }
        }

        public override bool HasRows => throw new NotImplementedException();

        public override bool IsClosed => _isClosed;

        public override int RecordsAffected => throw new NotImplementedException();

        public override bool GetBoolean(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override byte GetByte(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetDateTime(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override decimal GetDecimal(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override double GetDouble(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override Type GetFieldType(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override float GetFloat(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override Guid GetGuid(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override short GetInt16(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetInt32(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetInt64(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override string GetName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public override string GetString(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public override bool IsDBNull(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override bool NextResult()
        {
            throw new NotImplementedException();
        }

        public override bool Read()
        {
            throw new NotImplementedException();
        }

        override public void Close()
        {

        }

        internal bool IsBehavior(CommandBehavior behavior)
        {
            return (behavior == (behavior & _commandBehavior));
        }
    }
}
