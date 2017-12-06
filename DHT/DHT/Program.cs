namespace DHT
{
    using Constellation.Package;
    using Constellation.PythonProxy;

    class Program
    {
        static void Main(string[] args)
        {
            PackageHost.Start<PythonPackage>(args);
        }
    }
}