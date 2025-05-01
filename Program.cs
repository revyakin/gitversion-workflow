using System.Reflection;

var version = Assembly.GetExecutingAssembly()
                      .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                      .InformationalVersion;

Console.WriteLine($"Hello, World! App version: {version}");
