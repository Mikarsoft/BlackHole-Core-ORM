using Mikarsoft.BlackHoleCore.Connector.Enums;

namespace Mikarsoft.BlackHoleCore.Connector.Statements
{
    public class JoinStatement
    {
        public JoinStatement(JoinType joinType, byte[] tableLetters)
        {
            JType = joinType;
            TableACode = tableLetters[0];
            TableDCode = tableLetters[1];
            JoinPoints = new();
        }

        public JoinType JType { get;  private set; }

        public byte TableACode { get; private set; }

        public byte TableDCode { get; private set; }

        public List<JoinPoint> JoinPoints { get; private set; }

        public void AddJoinPoint(string columnA, string columnB, OuterPairType pairType)
        {
            JoinPoints.Add(new JoinPoint(columnA, columnB, pairType));
        }
    }

    public class JoinPoint
    {
        public JoinPoint(string columnA, string columnB, OuterPairType pairType)
        {
            PairType = pairType;
            ColumnA = columnA;
            ColumnB = columnB;
        }

        internal OuterPairType PairType { get; private set; }
        internal string ColumnA { get; private set; }
        internal string ColumnB { get; private set; }
    }
}