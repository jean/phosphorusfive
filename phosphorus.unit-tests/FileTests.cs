
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Reflection;
using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests
{
    [TestFixture]
    public class FileTests
    {
        [SetUp]
        public void SetUp ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
        }

        [ActiveEvent (Name = "pf.core.application-folder")]
        private static void GetRootFolder (ApplicationContext context, ActiveEventArgs e)
        {
            string asmPath = Assembly.GetExecutingAssembly ().Location;
            asmPath = asmPath.Substring (0, asmPath.LastIndexOf ("/") + 1);
            e.Args.Value = asmPath;
        }

        [Test]
        public void SaveAndLoadFile ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.remove:mumbo.txt
pf.file.save:mumbo.txt
  :""this is mumbo""
pf.file.load:mumbo.txt
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (1, tmp [1].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("this is mumbo", tmp [1] [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void OverwriteLongFileWithShort ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.remove:mumbo.txt
pf.file.save:mumbo.txt
  :""this is mumbo, and this is a long text""
pf.file.save:mumbo.txt
  :""this is mumbo, shorter""
pf.file.load:mumbo.txt
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (1, tmp [2].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("this is mumbo, shorter", tmp [2] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SaveAndRemoveFile ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.remove:mumbo.txt
pf.file.save:mumbo.txt
  :""this is mumbo""
pf.file.exists:mumbo.txt
pf.file.remove:mumbo.txt
pf.file.exists:mumbo.txt
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (true, tmp [2] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (false, tmp [4] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SaveToMultipleFiles ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.remove:mumbo1.txt
pf.file.remove:mumbo2.txt
_data
  :mumbo1.txt
  :mumbo2.txt
pf.file.save:@/-/*/?value
  :""this is mumbo""
pf.file.exists:mumbo1.txt
pf.file.exists:mumbo2.txt
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (true, tmp [4] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (true, tmp [5] [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void LoadMultipleFiles ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.remove:mumbo1.txt
pf.file.remove:mumbo2.txt
_data
  :mumbo1.txt
  :mumbo2.txt
pf.file.save:@/-/0/?value
  :""this is mumbo1""
pf.file.save:@/-2/1/?value
  :""this is mumbo2""
pf.file.load:@/-3/*/?value
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("this is mumbo1", tmp [5] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("mumbo1.txt", tmp [5] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("this is mumbo2", tmp [5] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("mumbo2.txt", tmp [5] [1].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void RemoveMultipleFiles ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.remove:mumbo1.txt
pf.file.remove:mumbo2.txt
_data
  :mumbo1.txt
  :mumbo2.txt
pf.file.save:@/-/0/?value
  :""this is mumbo1""
pf.file.save:@/-/1/?value
  :""this is mumbo2""
pf.file.remove:@/-3/*/?value
pf.file.exists:mumbo1.txt
pf.file.exists:mumbo2.txt
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (false, tmp [6] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (false, tmp [7] [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void FileExistsMultipleFiles ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.remove:mumbo1.txt
pf.file.remove:mumbo2.txt
_data
  :mumbo1.txt
  :mumbo2.txt
pf.file.save:@/-/0/?value
  :""this is mumbo1""
pf.file.save:@/-2/1/?value
  :""this is mumbo2""
pf.file.exists:@/-3/*/?value
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (true, tmp [5] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("mumbo1.txt", tmp [5] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (true, tmp [5] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("mumbo2.txt", tmp [5] [1].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void FileExistsMultipleFilesReturnsFalse ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.remove:mumbo1.txt
pf.file.remove:mumbo2.txt
pf.file.remove:mumbo3.txt
_data
  :mumbo1.txt
  :mumbo2.txt
  :mumbo3.txt
pf.file.save:@/-/0/?value
  :""this is mumbo1""
pf.file.save:@/-2/1/?value
  :""this is mumbo2""
pf.file.exists:@/-3/*/?value
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (true, tmp [6] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("mumbo1.txt", tmp [6] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (true, tmp [6] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("mumbo2.txt", tmp [6] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (false, tmp [6] [2].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("mumbo3.txt", tmp [6] [2].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void RemoveMultipleFilesReturnsFalse ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.remove:mumbo1.txt
pf.file.remove:mumbo2.txt
pf.file.remove:mumbo3.txt
_data
  :mumbo1.txt
  :mumbo2.txt
  :mumbo3.txt
pf.file.save:mumbo1.txt
  :""this is mumbo1""
pf.file.remove:@/-2/*/?value
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (true, tmp [5] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("mumbo1.txt", tmp [5] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (false, tmp [5] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("mumbo2.txt", tmp [5] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (false, tmp [5] [2].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("mumbo3.txt", tmp [5] [2].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void CreateAndRemoveFolder ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.remove-folder:mumbo
pf.file.create-folder:mumbo
pf.file.folder-exists:mumbo
pf.file.remove-folder:mumbo
pf.file.folder-exists:mumbo
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (true, tmp [2] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (false, tmp [4] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void CreateAndRemoveMultipleFolders ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.remove-folder:mumbo1
pf.file.remove-folder:mumbo2
_data
  :mumbo1
  :mumbo2
pf.file.create-folder:@/-/*/?value
pf.file.folder-exists:mumbo1
pf.file.folder-exists:mumbo2
pf.file.remove-folder:@/-4/*/?value
pf.file.folder-exists:mumbo1
pf.file.folder-exists:mumbo2
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (true, tmp [4] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (true, tmp [5] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (false, tmp [7] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (false, tmp [8] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void CreateAndRemoveMultipleFoldersReturnsFalse ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.remove-folder:mumbo1
pf.file.remove-folder:mumbo2
pf.file.remove-folder:mumbo3
_data
  :mumbo1
  :mumbo2
  :mumbo3
pf.file.create-folder:mumbo1
pf.file.folder-exists:@/-2/*/?value
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (true, tmp [5] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("mumbo1", tmp [5] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (false, tmp [5] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("mumbo2", tmp [5] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (false, tmp [5] [2].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("mumbo3", tmp [5] [2].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void RemoveNonExistingFolder ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.remove-folder:qwertyuiopXXXXX
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (false, tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("qwertyuiopXXXXX", tmp [0] [0].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void CheckNonExistingFolder ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.folder-exists:qwertyuiopXXXXX
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (false, tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("qwertyuiopXXXXX", tmp [0] [0].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void ListFiles ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.remove-folder:list-files
pf.file.create-folder:list-files
pf.file.save:list-files/mumbo1.txt
  :""mumbo1""
pf.file.save:list-files/mumbo2.txt
  :""mumbo2""
pf.file.save:list-files/mumbo3.txt
  :""mumbo3""
pf.file.list-files:list-files
pf.file.remove-folder:list-files
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (3, tmp [5].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("list-files/mumbo1.txt", tmp [5] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("list-files/mumbo2.txt", tmp [5] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("list-files/mumbo3.txt", tmp [5] [2].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void ListFolders ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.create-folder:list-folders
pf.file.create-folder:list-folders/mumbo1
pf.file.create-folder:list-folders/mumbo2
pf.file.create-folder:list-folders/mumbo3
pf.file.list-folders:list-folders
pf.file.remove-folder:list-folders
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (3, tmp [4].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("list-folders/mumbo1", tmp [4] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("list-folders/mumbo2", tmp [4] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("list-folders/mumbo3", tmp [4] [2].Value, "wrong value of node after executing lambda object");
        }
    }
}