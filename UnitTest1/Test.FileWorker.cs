using System;
using System.IO;
using Xunit;
using IIG.FileWorker;
using IIG.CoSFE.DatabaseUtils;
using System.Text;

namespace Test.FileWorker
{
    public class UnitTest1
    {
        private const string Server = @"HOME-PC";
        private const string Database = @"IIG.CoSWE.StorageDB";
        private const bool IsTrusted = true;
        private const string Login = @"sa";
        private const string Password = @"lab4";
        private const int ConnectionTime = 75;
        readonly StorageDatabaseUtils storageDatabaseUtils = new StorageDatabaseUtils(Server, Database, IsTrusted, Login, Password, ConnectionTime);
        static readonly string workingDirectory = Environment.CurrentDirectory;
        private readonly string testsDirFullPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName + "\\test";

        [Fact]
        public void TestAddFile() {
            string fileFullPath = testsDirFullPath + "\\test.txt";
            BaseFileWorker.Write("oh shit, i'm sorry", fileFullPath);
            string data = BaseFileWorker.ReadAll(fileFullPath);
            Assert.True(storageDatabaseUtils.AddFile("1", Encoding.UTF8.GetBytes(data)));
        }

        [Fact]
        public void TestDeleteFile() {
            int fileId = 2;
            Assert.True(storageDatabaseUtils.DeleteFile(fileId));
        }

        [Fact]
        public void TestGetFile() {
            storageDatabaseUtils.ExecSql("INSERT INTO dbo.Files (FileName, FileContent) values ('filename', CONVERT(varbinary(1024), 'sorry for what?'));");
            int fileId = (int)storageDatabaseUtils.GetIntBySql("SELECT FileID FROM dbo.Files WHERE FileName = 'filename';");
            Assert.True(storageDatabaseUtils.GetFile(fileId, out string fileName, out byte[] fileContent));
            Assert.Equal("filename", fileName);
            Assert.Equal("sorry for what?", Encoding.UTF8.GetString(fileContent));
            storageDatabaseUtils.ExecSql("DELETE FROM dbo.Files");
        }

        [Fact]
        public void TestGetFileNotExist() {
            int fileId = 54644;
            Assert.False(storageDatabaseUtils.GetFile(fileId, out _, out byte[] _));
        }

        [Fact]
        public void TestGetFiles() {
            storageDatabaseUtils.ExecSql("INSERT INTO dbo.Files (FileName, FileContent) values ('filename', CONVERT(varbinary(1024), 'sorry for what?')), ('filename', CONVERT(varbinary(1024), 'our daddy told us not to be shame'));");
            string[][] fileIds = storageDatabaseUtils.GetLstBySql("SELECT FileID FROM dbo.Files WHERE FileName = 'filename';");
            int[] ids = { Int32.Parse(fileIds[0][0]), Int32.Parse(fileIds[1][0]) };
            
            string fileName = "filename";
            System.Data.DataTable table = storageDatabaseUtils.GetFiles(fileName);
            Assert.Equal(2, table.Rows.Count);
            foreach (System.Data.DataRow row in table.Rows) {
                int fileId = (int)row[0];
                Assert.True(Array.Exists(ids, id => id == fileId));
            }
            storageDatabaseUtils.ExecSql("DELETE FROM dbo.Files");
        }

        [Fact]
        public void TestGetFilesUndefinedName() {
            string fileName = "NoSuchFile";
            System.Data.DataTable table = storageDatabaseUtils.GetFiles(fileName);
            Assert.Equal(0, table.Rows.Count);
        }

        [Fact]
        public void TestGetFilesEmptyName() {
            storageDatabaseUtils.ExecSql("INSERT INTO dbo.Files (FileName, FileContent) values ('filename', CONVERT(varbinary(1024), 'sorry for what?')), ('filename', CONVERT(varbinary(1024), 'our daddy told us not to be shame'));");
            System.Data.DataTable table = storageDatabaseUtils.GetFiles();
            Assert.Equal(2, table.Rows.Count);
            storageDatabaseUtils.ExecSql("DELETE FROM dbo.Files");
        }
    }
}