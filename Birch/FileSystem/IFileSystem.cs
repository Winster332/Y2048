namespace Birch.FileSystem;

public interface IFileSystem
{
    public bool Create(string file, string content = "");
    public bool ExistFile(string path);
    public bool ExistDirectory(string path);
    public string[] GetFiles(string path);
    public string[] GetDirectories(string path);
    public bool WriteTo(string path, string content);
    public string ReadFromFile(string path);
    public byte[] ReadBytesFromFile(string path);
    public bool RewriteTo(string path, string content);
    public bool DeleteFile(string path);
    public bool DeleteDirectory(string path);
    public byte[] ReadBytesFromAsset(string path);
    public void RewriteToAsset(string path, string content);
    public string ReadStringFromAsset(string fileName);
}
