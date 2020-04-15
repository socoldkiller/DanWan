using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyProject1.FileEntity
{
    public class FileEnity
    {
        //主目录
        private string path;
        private Dictionary<string, List<string>> directoryAndFiles;
        public FileEnity(string path)
        {
            this.path = path;
            directoryAndFiles = new Dictionary<string, List<string>>();
            bfs();
        }

        public string Path { get => path; set => path = value; }
        public Dictionary<string, List<string>> DirectoryAndFiles { get => directoryAndFiles; set => directoryAndFiles = value; }

        public Dictionary<string,List<string>>displayDirectoryFiles()
        {
            Dictionary<string, List<string>> show=new Dictionary<string, List<string>>();
            foreach(KeyValuePair<string, List<string>> x in directoryAndFiles)
            {
                string directory = System.IO.Path.GetFileNameWithoutExtension(x.Key);
                var files = x.Value.Select(tmp => tmp = System.IO.Path.GetFileNameWithoutExtension(tmp)).ToList();
                show.Add(directory, files);
            }
            return show;
        }

        //linq大法
        private void bfs()
        {
            Queue<string> q = new Queue<string>();
            q.Enqueue(path);
            while (q.Count != 0)
            {
                string path = q.Dequeue();
                List<string> files = Directory.GetFiles(path, "*.js").ToList();
                files.Sort();
                if (files.Count != 0)
                {
                    directoryAndFiles[path] = files;
                }

                string[] directorys = Directory.GetDirectories(path);
                foreach (string directroy in directorys)
                {
                    q.Enqueue(directroy);
                }

            }
        }
    }
}
