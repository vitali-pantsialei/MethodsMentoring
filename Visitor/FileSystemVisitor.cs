using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visitor
{
    public class FileSystemVisitor : IEnumerable<FileSystemInfo>
    {
        private class ForTerm
        {
            public bool isTerm { get; set; }
        }

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private DirectoryInfo root;
        private Func<FileSystemInfo, bool> fileSystemFilter;
        private ForTerm terminateSearch = new ForTerm { isTerm = false };
        public event EventHandler<IterationControlArgs> Start;
        public event EventHandler<IterationControlArgs> Finish;
        public event EventHandler<IterationControlArgs> FileFinded;
        public event EventHandler<IterationControlArgs> DirectoryFinded;
        public event EventHandler<IterationControlArgs> FilteredFileFinded;
        public event EventHandler<IterationControlArgs> FilteredDirectoryFinded;

        public FileSystemVisitor(string rootPath)
        {
            this.root = new DirectoryInfo(rootPath);
            this.fileSystemFilter = x => true;
        }

        public FileSystemVisitor(string rootPath, Func<FileSystemInfo, bool> filter)
        {
            this.root = new DirectoryInfo(rootPath);
            this.fileSystemFilter = filter;
        }

        public IEnumerator<FileSystemInfo> GetEnumerator()
        {
            if (terminateSearch.isTerm)
            {
                logger.Info("Search has been terminated");
                yield break;
            }

            IterationControlArgs args = DefaultIterationControlArgs();
            args.CurrentFile = root;
            OnEvent(Start, args);
            if (args.TerminateSearch)
            {
                terminateSearch.isTerm = true;
                logger.Info("Search has been terminated");
                yield break;
            }

            if (!args.Exclude)
            {
                if (fileSystemFilter(root))
                    yield return root;
                foreach (DirectoryInfo d in root.GetDirectories())
                {
                    args = DefaultIterationControlArgs();
                    args.CurrentFile = d;
                    OnEvent(DirectoryFinded, args);
                    if (args.TerminateSearch)
                    {
                        terminateSearch.isTerm = true;
                        logger.Info("Search has been terminated");
                        yield break;
                    }
                    if (args.Exclude)
                    {
                        logger.Warn("Folder has been excluded");
                        continue;
                    }

                    if (fileSystemFilter(d))
                    {
                        args = DefaultIterationControlArgs();
                        args.CurrentFile = d;
                        OnEvent(FilteredDirectoryFinded, args);
                        if (args.TerminateSearch)
                        {
                            terminateSearch.isTerm = true;
                            logger.Info("Search has been terminated");
                            yield break;
                        }
                        if (args.Exclude)
                        {
                            logger.Warn("Folder has been excluded");
                            continue;
                        }
                    }
                    var newVisitor = new FileSystemVisitor(d.FullName, fileSystemFilter);
                    InheretAllEvents(newVisitor);
                    foreach (FileSystemInfo info in newVisitor)
                    {
                        if (fileSystemFilter(info))
                            yield return info;
                    }
                }
                foreach (FileInfo f in root.GetFiles())
                {
                    args = DefaultIterationControlArgs();
                    args.CurrentFile = f;
                    OnEvent(FileFinded, args);
                    if (args.TerminateSearch)
                    {
                        terminateSearch.isTerm = true;
                        logger.Info("Search has been terminated");
                        yield break;
                    }
                    if (args.Exclude)
                    {
                        logger.Warn("File has been excluded");
                        continue;
                    }

                    if (fileSystemFilter(f))
                    {
                        args = DefaultIterationControlArgs();
                        args.CurrentFile = f;
                        OnEvent(FilteredFileFinded, args);
                        if (args.TerminateSearch)
                        {
                            terminateSearch.isTerm = true;
                            logger.Info("Search has been terminated");
                            yield break;
                        }
                        if (args.Exclude)
                        {
                            logger.Warn("File has been excluded");
                            continue;
                        }

                        yield return f;
                    }
                }
            }
            args = DefaultIterationControlArgs();
            OnEvent(Finish, args);
            if (args.TerminateSearch)
            {
                terminateSearch.isTerm = true;
                logger.Info("Search has been terminated");
                yield break;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected virtual void OnEvent(EventHandler<IterationControlArgs> triggeredEvent, IterationControlArgs args)
        {
            var tmp = triggeredEvent;
            if (tmp != null)
                tmp(this, args);
        }

        private IterationControlArgs DefaultIterationControlArgs()
        {
            return new IterationControlArgs { CurrentFile = null, Exclude = false, TerminateSearch = false };
        }

        private void InheretAllEvents(FileSystemVisitor newVisitor)
        {
            newVisitor.terminateSearch = terminateSearch;
            newVisitor.FileFinded = this.FileFinded;
            newVisitor.FilteredFileFinded = this.FilteredFileFinded;
            newVisitor.DirectoryFinded = this.DirectoryFinded;
            newVisitor.FilteredDirectoryFinded = this.FilteredDirectoryFinded;
        }
    }
}
