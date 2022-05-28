using System;

namespace ProxyGenerator {
  public class Test : ITest {
    public void DoSomething(string s1, string s2) {
      Console.WriteLine(s1 + s2);
    }

    public int DoSomethingElse(int x, int y) {
      return x + y;
    }
  }
}
