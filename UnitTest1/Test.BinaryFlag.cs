using Xunit;
using System;
using System.IO;
using IIG.CoSFE.DatabaseUtils;
using IIG.BinaryFlag;

namespace Test.BinaryFlag {
    public class UnitTest1 {
        private const string Server = @"HOME-PC";
        private const string Database = @"IIG.CoSWE.FlagpoleDB";
        private const bool IsTrusted = true;
        private const string Login = @"sa";
        private const string Password = @"lab4";
        private const int ConnectionTime = 75;
        readonly FlagpoleDatabaseUtils flagDatabaseUtils = new FlagpoleDatabaseUtils(Server, Database, IsTrusted, Login, Password, ConnectionTime);

        [Fact]
        public void TestGetFlagFalse() {
            flagDatabaseUtils.ExecSql("INSERT INTO dbo.MultipleBinaryFlags (MultipleBinaryFlagView, MultipleBinaryFlagValue) values ('FFFFF', 0);");
            int id = (int)flagDatabaseUtils.GetIntBySql("SELECT MultipleBinaryFlagID FROM dbo.MultipleBinaryFlags WHERE MultipleBinaryFlagView = 'FFFFF'");
            Assert.True(flagDatabaseUtils.GetFlag(id, out string flagView, out bool? flagVal));
            Assert.Equal("FFFFF", flagView);
            Assert.False(flagVal);
            flagDatabaseUtils.ExecSql("DELETE FROM dbo.MultipleBinaryFlags");
        }

        [Fact]
        public void TestGetFlagTrue() {
            flagDatabaseUtils.ExecSql("INSERT INTO dbo.MultipleBinaryFlags (MultipleBinaryFlagView, MultipleBinaryFlagValue) values ('TTTTT', 1);");
            int id = (int)flagDatabaseUtils.GetIntBySql("SELECT MultipleBinaryFlagID FROM dbo.MultipleBinaryFlags WHERE MultipleBinaryFlagView = 'TTTTT'");
            Assert.True(flagDatabaseUtils.GetFlag(id, out string flagView, out bool? flagVal));
            Assert.Equal("TTTTT", flagView);
            Assert.True(flagVal);
            flagDatabaseUtils.ExecSql("DELETE FROM dbo.MultipleBinaryFlags");
        }

        [Fact]
        public void TestAddFlagTrue() {
            MultipleBinaryFlag mbf = new MultipleBinaryFlag(5);
            bool actual = mbf.GetFlag();
            Assert.True(flagDatabaseUtils.AddFlag(mbf.ToString(), actual));
            flagDatabaseUtils.ExecSql("DELETE FROM dbo.MultipleBinaryFlags");
        }

        [Fact]
        public void TestAddFlagFalse() {
            MultipleBinaryFlag mbf = new MultipleBinaryFlag(5, false);
            bool actual = mbf.GetFlag();
            Assert.True(flagDatabaseUtils.AddFlag(mbf.ToString(), actual));
            flagDatabaseUtils.ExecSql("DELETE FROM dbo.MultipleBinaryFlags");
        }
    }
}