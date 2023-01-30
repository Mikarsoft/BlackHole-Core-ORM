using BlackHole.Entities;
using BlackHole.Interfaces;
using BlackHole.Statics;
using Dapper;
using System.Data;

namespace BlackHole.Data
{
    internal class BHDataCascade : IBHDataCascade
    {
        private IBHDatabaseSelector _multiDatabaseSelector;
        private bool isMyShit { get; set; } = false;

        public BHDataCascade(IBHDatabaseSelector multiDatabaseSelector)
        {
            _multiDatabaseSelector = multiDatabaseSelector;
            isMyShit = _multiDatabaseSelector.GetMyShit();
        }

        async void IBHDataCascade.CascadeTable(string table , List<int> Ids)
        {
            if (Ids.Count > 0 && Ids[0] != 0)
            {
                bool successfull = true;
                bool startDigging = false;
                int diggingIndex = 0;
                int safetyLimiter = 0;

                List<CascadeTree> cascadeTree = new List<CascadeTree>();

                CascadeTree cascadeRoot = CascadeRoot(table, Ids);
                cascadeTree.Add(cascadeRoot);

                if (cascadeRoot.NonNullableCount > 0)
                {
                    startDigging = true;
                }

                while (startDigging)
                {
                    if(diggingIndex >= 0)
                    {
                        if (cascadeTree[diggingIndex].CurrentBranch < cascadeTree[diggingIndex].NonNullableCount )
                        {
                            DataConstraints currentConstraint = cascadeTree[diggingIndex].NonNullableConstraints[cascadeTree[diggingIndex].CurrentBranch];
                            cascadeTree.Add(CascadeBranch(currentConstraint.TABLE_NAME, currentConstraint.COLUMN_NAME, cascadeTree[diggingIndex].AffectedIds));
                            cascadeTree[diggingIndex].CurrentBranch++;
                            diggingIndex = cascadeTree.Count - 1;
                        }
                        else
                        {
                            diggingIndex--;
                        }
                    }
                    else
                    {
                        startDigging = false;
                    }

                    if(safetyLimiter > 100)
                    {
                        startDigging = false;
                        successfull = false;
                    }

                    safetyLimiter++;
                }

                if (successfull)
                {
                    await ExecuteCascade(cascadeTree, table, Ids);
                }
            }
        }

        private async Task ExecuteCascade(List<CascadeTree> cascadeTree , string table , List<int> Ids)
        {
            string NullableCommands = string.Empty;
            string NotNullableCommands = string.Empty;
            string BaseCommand = $"delete from {MyShit(table)} {WhereIds(MyShit("Id"), Ids)};";

            int branches = cascadeTree.Count;

            for(int i = 0; i < branches; i++)
            {
                NullableCommands += cascadeTree[branches - i - 1].NullableCascade;
                NotNullableCommands += cascadeTree[branches - i - 1].NonNullableCascade;
            }

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    await connection.ExecuteAsync(NullableCommands + NotNullableCommands + BaseCommand);
                }
            }
            catch(Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        private CascadeTree CascadeBranch(string TableName, string columnName, List<int> ParentIds)
        {
            CascadeTree cascadeTree = new CascadeTree
            {
                CurrentBranch = 0,
                NonNullableCount = 0,
            };

            if (ParentIds.Count > 0)
            {
                List<DataConstraints> constraints = CascadeRelations.dataConstrains.Where(x => x.REFERENCED_TABLE_NAME == TableName).ToList();

                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    string idsCommand = $"select {MyShit("Id")} from {MyShit(TableName)} {WhereIds(MyShit(columnName), ParentIds)}";
                    cascadeTree.AffectedIds = connection.Query<int>(idsCommand).ToList();
                }

                if(cascadeTree.AffectedIds.Count > 0)
                {
                    foreach (DataConstraints constraint in constraints.Where(x => x.IS_NULLABLE == "YES"))
                    {
                        string? column = MyShit(constraint.COLUMN_NAME);
                        string command = $"update {MyShit(constraint.TABLE_NAME)} set {column}=null {WhereIds(column, cascadeTree.AffectedIds)};";
                        cascadeTree.NullableCascade += command;
                    }

                    cascadeTree.NonNullableConstraints = constraints.Where(x => x.IS_NULLABLE == "NO").ToList();
                    cascadeTree.NonNullableCount = cascadeTree.NonNullableConstraints.Count;

                    foreach (DataConstraints constraint in cascadeTree.NonNullableConstraints)
                    {
                        string? column = MyShit(constraint.COLUMN_NAME);
                        string command = $"delete from {MyShit(constraint.TABLE_NAME)} {WhereIds(column, cascadeTree.AffectedIds)};";
                        cascadeTree.NonNullableCascade += command;
                    }
                }  
            }

            return cascadeTree;
        }

        private CascadeTree CascadeRoot(string TableName, List<int> AffectedIds)
        {
            CascadeTree cascadeTree = new CascadeTree
            {
                CurrentBranch = 0,
                NonNullableCount = 0,
                AffectedIds = AffectedIds
            };

            if (cascadeTree.AffectedIds.Count > 0)
            {
                List<DataConstraints> constraints = CascadeRelations.dataConstrains.Where(x => x.REFERENCED_TABLE_NAME == TableName).ToList();

                foreach (DataConstraints constraint in constraints.Where(x => x.IS_NULLABLE == "YES"))
                {
                    string? column = MyShit(constraint.COLUMN_NAME);
                    string command = $"update {MyShit(constraint.TABLE_NAME)} set {column}=null {WhereIds(column, cascadeTree.AffectedIds)};";
                    cascadeTree.NullableCascade += command;
                }

                cascadeTree.NonNullableConstraints = constraints.Where(x => x.IS_NULLABLE == "NO").ToList();
                cascadeTree.NonNullableCount = cascadeTree.NonNullableConstraints.Count;

                foreach (DataConstraints constraint in cascadeTree.NonNullableConstraints)
                {
                    string? column = MyShit(constraint.COLUMN_NAME);
                    string command = $"delete from {MyShit(constraint.TABLE_NAME)} {WhereIds(column, cascadeTree.AffectedIds)};";
                    cascadeTree.NonNullableCascade += command;
                }
            }

            return cascadeTree;
        }

        public string WhereIds(string? column, List<int> Ids)
        {
            string whereCommand = "where";

            foreach (int Id in Ids)
            {
                whereCommand += $" {column}={Id} or";
            }

            return whereCommand.Substring(0, whereCommand.Length - 3);            
        }

        string? MyShit(string? propName)
        {
            string? result = propName;

            if (!isMyShit)
            {
                result = $@"""{propName}""";
            }

            return result;
        }
    }
}
