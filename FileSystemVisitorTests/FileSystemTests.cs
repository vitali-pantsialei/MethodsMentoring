using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Visitor;

namespace FileSystemVisitorTests
{
    [TestClass]
    public class FileSystemTests
    {
        [TestMethod]
        public void IterationObjectsCountTest()
        {
            FileSystemVisitor visit = new FileSystemVisitor(@"..\..\Test");
            int objectCount = 0;

            foreach (var v in visit)
            {
                ++objectCount;
            }

            int expected = 19;
            Assert.AreEqual(expected, objectCount);
        }

        [TestMethod]
        public void IterationTextFileCountTest()
        {
            FileSystemVisitor visit = new FileSystemVisitor(@"..\..\Test", x => x.Extension == ".txt");
            int objectCount = 0;

            foreach (var v in visit)
            {
                ++objectCount;
            }

            int expected = 6;
            Assert.AreEqual(expected, objectCount);
        }

        [TestMethod]
        public void IterationExcludeFilesWith1InNameTest()
        {
            FileSystemVisitor visit = new FileSystemVisitor(@"..\..\Test");
            visit.FileFinded += visit_FileFinded;
            int objectCount = 0;

            foreach (var v in visit)
            {
                ++objectCount;
            }

            int expected = 14;
            Assert.AreEqual(expected, objectCount);
        }

        [TestMethod]
        public void IterationExcludeFoldersWith4InNameTest()
        {
            FileSystemVisitor visit = new FileSystemVisitor(@"..\..\Test");
            visit.DirectoryFinded += visit_DirectoryFinded;
            int objectCount = 0;

            foreach (var v in visit)
            {
                ++objectCount;
            }

            int expected = 10;
            Assert.AreEqual(expected, objectCount);
        }

        [TestMethod]
        public void IterationFilteredContains3ExcludeCoolTest()
        {
            FileSystemVisitor visit = new FileSystemVisitor(@"..\..\Test", x => x.Name.Contains("3"));
            visit.FilteredDirectoryFinded += visit_FilteredDirectoryFinded;
            int objectCount = 0;

            foreach (var v in visit)
            {
                ++objectCount;
            }

            int expected = 2;
            Assert.AreEqual(expected, objectCount);
        }

        [TestMethod]
        public void IterationStopSearchIfFileNameContains2()
        {
            FileSystemVisitor visit = new FileSystemVisitor(@"..\..\Test");
            visit.FilteredFileFinded += visit_FilteredFileFinded;
            int objectCount = 0;

            foreach (var v in visit)
            {
                ++objectCount;
            }

            int expected = 9;
            Assert.AreEqual(expected, objectCount);
        }

        void visit_FilteredFileFinded(object sender, IterationControlArgs e)
        {
            if (e.CurrentFile.Name.Contains("2"))
                e.TerminateSearch = true;
        }

        void visit_FilteredDirectoryFinded(object sender, IterationControlArgs e)
        {
            if (e.CurrentFile.Name.Contains("Cool"))
                e.Exclude = true;
        }

        void visit_DirectoryFinded(object sender, IterationControlArgs e)
        {
            if (e.CurrentFile.Name.Contains("4"))
                e.Exclude = true;
        }

        void visit_FileFinded(object sender, IterationControlArgs e)
        {
            if (e.CurrentFile.Name.Contains("1"))
                e.Exclude = true;
        }
    }
}
