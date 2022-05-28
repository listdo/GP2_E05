using System;

namespace ProxyGenerator {
  public class Program {
    public static void Main(string[] args) {
      ITest test = new Test();

      IInterception interception = new LogInterception();
      ITest proxy = ProxyGenerator.Create(test, interception);

      test.DoSomething("Hello ", "World");
      Console.WriteLine(test.DoSomethingElse(13, 29));

      Console.WriteLine("---------");

      proxy.DoSomething("Hello ", "World");
      Console.WriteLine(proxy.DoSomethingElse(13, 29));
    }
  }
}
