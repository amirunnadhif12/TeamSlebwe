using System;
using System.Collections.Generic;
using System.Linq;

class FileSystem
{
    private Dictionary<string, object> files;
    private string currentPath;

    public FileSystem()
    {
        files = new Dictionary<string, object>();
        currentPath = "/";
    }

    private string[] ParsePath(string path)
    {
        if (path.StartsWith("/"))
            return path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
        return (currentPath + (currentPath.EndsWith("/") ? "" : "/") + path).Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
    }

    public void Touch(string filename)
    {
        var parsedPath = ParsePath(filename);
        var dirPath = string.Join("/", parsedPath.Take(parsedPath.Length - 1)) + "/";
        var fileName = parsedPath.Last();

        if (!files.ContainsKey(dirPath))
        {
            Console.WriteLine($"Directory '{dirPath}' does not exist.");
            return;
        }

        var dir = files[dirPath] as Dictionary<string, object>;
        if (dir.ContainsKey(fileName))
        {
            Console.WriteLine($"File '{fileName}' already exists.");
            return;
        }

        dir[fileName] = null;
        Console.WriteLine($"File '{fileName}' created.");
    }

    public void Mkdir(string dirname)
    {
        var parsedPath = ParsePath(dirname);
        var dirPath = string.Join("/", parsedPath.Take(parsedPath.Length - 1)) + "/";
        var newDirName = parsedPath.Last();

        if (!files.ContainsKey(dirPath))
        {
            Console.WriteLine($"Parent directory '{dirPath}' does not exist.");
            return;
        }

        var parentDir = files[dirPath] as Dictionary<string, object>;
        if (parentDir.ContainsKey(newDirName))
        {
            Console.WriteLine($"Directory '{newDirName}' already exists.");
            return;
        }

        parentDir[newDirName] = new Dictionary<string, object>();
        files[string.Join("/", parsedPath) + "/"] = parentDir[newDirName];
        Console.WriteLine($"Directory '{newDirName}' created.");
    }

    public void Ls(string path = null)
    {
        var parsedPath = path != null ? ParsePath(path) : ParsePath(currentPath);
        var dirPath = string.Join("/", parsedPath) + "/";

        if (!files.ContainsKey(dirPath))
        {
            Console.WriteLine($"Directory '{dirPath}' does not exist.");
            return;
        }

        var dir = files[dirPath] as Dictionary<string, object>;
        Console.WriteLine("Contents:");
        foreach (var item in dir.Keys)
        {
            Console.WriteLine(item);
        }
    }

    public void Cd(string path)
    {
        var parsedPath = ParsePath(path);
        var dirPath = string.Join("/", parsedPath) + "/";

        if (!files.ContainsKey(dirPath))
        {
            Console.WriteLine($"Directory '{path}' not found.");
            return;
        }

        currentPath = dirPath;
        Console.WriteLine($"Changed directory to {dirPath}");
    }

    public void Rm(string path)
    {
        var parsedPath = ParsePath(path);
        var dirPath = string.Join("/", parsedPath.Take(parsedPath.Length - 1)) + "/";
        var name = parsedPath.Last();

        if (!files.ContainsKey(dirPath))
        {
            Console.WriteLine($"Parent directory '{dirPath}' does not exist.");
            return;
        }

        var parentDir = files[dirPath] as Dictionary<string, object>;
        if (!parentDir.ContainsKey(name))
        {
            Console.WriteLine($"File or directory '{name}' not found.");
            return;
        }

        parentDir.Remove(name);
        Console.WriteLine($"Removed '{path}'.");
    }

    public void Mv(string sourcePath, string destPath)
    {
        var parsedSourcePath = ParsePath(sourcePath);
        var parsedDestPath = ParsePath(destPath);

        var sourceDirPath = string.Join("/", parsedSourcePath.Take(parsedSourcePath.Length - 1)) + "/";
        var destDirPath = string.Join("/", parsedDestPath.Take(parsedDestPath.Length - 1)) + "/";

        var sourceName = parsedSourcePath.Last();
        var destName = parsedDestPath.Last();

        if (!files.ContainsKey(sourceDirPath) || !files.ContainsKey(destDirPath))
        {
            Console.WriteLine("Source or destination directory does not exist.");
            return;
        }

        var sourceDir = files[sourceDirPath] as Dictionary<string, object>;
        var destDir = files[destDirPath] as Dictionary<string, object>;

        if (!sourceDir.ContainsKey(sourceName))
        {
            Console.WriteLine($"Source '{sourceName}' not found.");
            return;
        }

        destDir[destName] = sourceDir[sourceName];
        sourceDir.Remove(sourceName);
        Console.WriteLine($"Moved '{sourcePath}' to '{destPath}'.");
    }

    public void Cp(string sourcePath, string destPath)
    {
        var parsedSourcePath = ParsePath(sourcePath);
        var parsedDestPath = ParsePath(destPath);

        var sourceDirPath = string.Join("/", parsedSourcePath.Take(parsedSourcePath.Length - 1)) + "/";
        var destDirPath = string.Join("/", parsedDestPath.Take(parsedDestPath.Length - 1)) + "/";

        var sourceName = parsedSourcePath.Last();
        var destName = parsedDestPath.Last();

        if (!files.ContainsKey(sourceDirPath) || !files.ContainsKey(destDirPath))
        {
            Console.WriteLine("Source or destination directory does not exist.");
            return;
        }

        var sourceDir = files[sourceDirPath] as Dictionary<string, object>;
        var destDir = files[destDirPath] as Dictionary<string, object>;

        if (!sourceDir.ContainsKey(sourceName))
        {
            Console.WriteLine($"Source '{sourceName}' not found.");
            return;
        }

        destDir[destName] = sourceDir[sourceName];
        Console.WriteLine($"Copied '{sourcePath}' to '{destPath}'.");
    }

    public void Pwd()
    {
        Console.WriteLine(currentPath);
    }

    public void Locate(string startPath, string keyword)
    {
        var parsedPath = ParsePath(startPath);
        var dirPath = string.Join("/", parsedPath) + "/";

        if (!files.ContainsKey(dirPath))
        {
            Console.WriteLine($"Start directory '{startPath}' does not exist.");
            return;
        }

        Console.WriteLine("Matching files and directories:");
        foreach (var file in files.Keys)
        {
            if (file.Contains(keyword))
            {
                Console.WriteLine(file);
            }
        }
    }

    static void Main(string[] args)
    {
        FileSystem fs = new FileSystem();
        fs.Mkdir("dir1");
        fs.Touch("file1.txt");
        fs.Ls();
        fs.Cd("dir1");
        fs.Touch("file2.txt");
        fs.Ls();
        fs.Cd("/");
        fs.Rm("file1.txt");
        fs.Ls();
        fs.Mkdir("dir2");
        fs.Mv("dir1", "dir2/dir1");
        fs.Ls("dir2");
        fs.Cp("dir2/dir1", "dir2/dir1_copy");
        fs.Locate("/", "file");
        fs.Pwd();
    }
}
