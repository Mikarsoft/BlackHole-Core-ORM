using Mikarsoft.BlackHoleCore.Connector.Enums;

namespace Mikarsoft.BlackHoleCore.Connector.Statements
{
    public class JoinStatement<T> where T : class
    {
        public JoinStatement(string table1, string table2, JoinType joinType)
        {
            byte letter1 = AddIndex();
            byte letter2 = AddIndex();

            TableKeys.Add(table1, letter1);
            TableKeys.Add(table1, letter2);

            TablesPair.Add(new(letter1, letter2, joinType));
        }

        public void AddPair(string table1, string table2, JoinType joinType)
        {
            byte letter1;
            byte letter2;

            if(TableKeys.ContainsKey(table1))
            {
                letter1 = TableKeys[table1];
            }
            else
            {
                throw new ArgumentException($"Table {table1} does Not Exist in the previous sequence." +
                    $"the first table of each join must have been used in some previous join.");
            }

            if (TableKeys.ContainsKey(table2))
            {
                letter2 = TableKeys[table2];
            }
            else
            {
                letter2 = AddIndex();
                TableKeys.Add(table2, letter2);
            }

            TablesPair.Add(new(letter1, letter2, joinType));
        }

        public void AddConnection(string tableOneProperty, string tableTwoProperty, OuterPairType outerOperator = OuterPairType.On)
        {
            LastInserted.PropertyPairs.Add(new PropertyPair(tableOneProperty, tableTwoProperty, outerOperator));
        }

        public void AddWhereCase(BHExpressionPart[] parts)
        {
            LastInserted.WhereStatements.Add(new WhereStatement(parts));
        }

        private byte JoinIndex;

        private byte AddIndex()
        {
            if(JoinIndex > 254)
            {
                throw new ArgumentOutOfRangeException("The Maximum allowed count of tables in a Join is 254..");
            }

            JoinIndex++;
            return JoinIndex;
        }

        public Dictionary<string, byte> TableKeys { get; set; } = new();

        public JoinPair LastInserted => TablesPair[TablesPair.Count - 1];

        public byte LatestFirstLetter => LastInserted.FirstLetter;

        public byte LatestSecondLetter => LastInserted.SecondLetter;

        public List<JoinPair> TablesPair { get; set; } = new List<JoinPair>();
    }

    public class JoinPair
    {
        internal JoinPair(byte letter1, byte letter2, JoinType joinType)
        {
            FirstLetter = letter1;
            SecondLetter = letter2;
            JType = joinType;
        }

        internal JoinType JType { get; set; }

        internal byte FirstLetter { get; set; }
        internal byte SecondLetter { get; set; }

        public List<PropertyPair> PropertyPairs { get; set; } = new();
        public List<WhereStatement> WhereStatements { get; set; } = new();
    }

    public class PropertyPair
    {
        public PropertyPair(string tableOneProperty, string tableTwoProperty, OuterPairType outerOperator)
        {
            TableOneProperty = tableOneProperty;
            TableTwoProperty = tableTwoProperty;
            OuterOperator = outerOperator;
        }

        public OuterPairType OuterOperator { get;}
        public string TableOneProperty { get;} 
        public string TableTwoProperty { get;}
    }
}
