using FluentMigrator.VersionTableInfo;

namespace Rabbit.Components.Data.Migrators
{
    [VersionTableMetaData]
    public class VersionTable : DefaultVersionTableMetaData
    {
        private static string _tableName;

        #region Overrides of DefaultVersionTableMetaData

        public override string TableName => _tableName;

        #endregion Overrides of DefaultVersionTableMetaData

        public static void SetTableName(string tableName)
        {
            _tableName = tableName;
        }
    }
}