using System;
using System.IO;
using Android.Content.Res;
using Birch.FileSystem;

namespace Birch.Android;

public class AndroidFileSystem : IFileSystem
{
    private AssetManager _assetManager;

    public AndroidFileSystem(AssetManager assetManager)
    {
        _assetManager = assetManager;
    }

    public bool Create(string file, string content = "")
    {
        try
        {
            using var fileStream = new FileStream(file, FileMode.Create);

            if (string.IsNullOrEmpty(content)) return true;

            using var writer = new StreamWriter(fileStream);
            writer.WriteLine(content);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return false;
    }

    public bool ExistFile(string path)
    {
        try
        {
            return File.Exists(path);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return false;
    }

    public bool ExistDirectory(string path)
    {
        try
        {
            return Directory.Exists(path);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return false;
    }

    public string[] GetFiles(string path)
    {
        try
        {
            return Directory.GetFiles(path);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return Array.Empty<string>();
    }

    public string[] GetDirectories(string path)
    {
        try
        {
            return Directory.GetDirectories(path);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return Array.Empty<string>();
    }

    public bool WriteTo(string path, string content)
    {
        if (!ExistFile(path)) Create(path, content);
        else
        {
            var currentContent = ReadFromFile(path);
            try
            {
                using var fileStream = new FileStream(path, FileMode.Open);
                using var writer = new StreamWriter(fileStream);
                writer.WriteLine($"{currentContent}{content}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        return false;
    }

    public byte[] ReadBytesFromFile(string path)
    {
        var bytes = Array.Empty<byte>();
        try
        {
            using var fileStream = new FileStream(path, FileMode.Open);
            using (var memory = new MemoryStream())
            {
                fileStream.CopyTo(memory);
                bytes = memory.ToArray();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return bytes;
    }

    public bool RewriteTo(string path, string content)
    {
        try
        {
            using var fileStream = new FileStream(path, FileMode.OpenOrCreate);
            using var writer = new StreamWriter(fileStream);
            writer.WriteLine(content);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return false;
    }

    public string ReadFromFile(string path)
    {
        try
        {
            using var fileStream = new FileStream(path, FileMode.Open);
            using var reader = new StreamReader(fileStream);
            return reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return string.Empty;
    }

    public bool DeleteFile(string path)
    {
        try
        {
            File.Delete(path);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return false;
    }

    public bool DeleteDirectory(string path)
    {
        try
        {
            Directory.Delete(path);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return false;
    }

    public string ReadStringFromAsset(string fileName)
    {
        var text = string.Empty;

        using (var input = _assetManager.Open(fileName))
        {
            using (var reader = new StreamReader(input))
            {
                text = reader.ReadToEnd();
            }
        }

        return text;
    }

    public byte[] ReadBytesFromAsset(string path)
    {
        var bytes = Array.Empty<byte>();
        try
        {
            using var fileStream = _assetManager.Open(path);
            using (var memory = new MemoryStream())
            {
                fileStream.CopyTo(memory);
                bytes = memory.ToArray();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return bytes;
    }

    public void RewriteToAsset(string path, string content)
    {
        try
        {
            using var fileStream = _assetManager.Open(path);
            using var writer = new StreamWriter(fileStream);
            writer.WriteLine(content);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
