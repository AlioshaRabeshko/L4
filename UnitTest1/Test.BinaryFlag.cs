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

        [Theory]
        [InlineData(5)]
        [InlineData(200000)]
        public void TestAddFlagTrue(ulong length) {
            MultipleBinaryFlag mbf = new MultipleBinaryFlag(length);
            bool actual = mbf.GetFlag();
            Assert.True(flagDatabaseUtils.AddFlag(mbf.ToString(), actual));
            flagDatabaseUtils.ExecSql("DELETE FROM dbo.MultipleBinaryFlags");
        }

        [Theory]
        [InlineData(5)]
        [InlineData(200000)]
        public void TestAddFlagFalse(ulong length) {
            MultipleBinaryFlag mbf = new MultipleBinaryFlag(length, false);
            bool actual = mbf.GetFlag();
            Assert.True(flagDatabaseUtils.AddFlag(mbf.ToString(), actual));
            flagDatabaseUtils.ExecSql("DELETE FROM dbo.MultipleBinaryFlags");
        }



        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestAddFlag(bool direction) {
            MultipleBinaryFlag mbf = new MultipleBinaryFlag(20, direction);
            if (direction) {
                mbf.SetFlag(2);
                mbf.SetFlag(4);
                mbf.SetFlag(9);
                mbf.SetFlag(15);
            } else {
                mbf.ResetFlag(1);
                mbf.ResetFlag(7);
                mbf.ResetFlag(4);
                mbf.ResetFlag(13);
            }
            bool actual = mbf.GetFlag();
            Assert.True(flagDatabaseUtils.AddFlag(mbf.ToString(), actual));
            flagDatabaseUtils.ExecSql("DELETE FROM dbo.MultipleBinaryFlags");
        }
    }
}