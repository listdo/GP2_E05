using System;

namespace ProxyGenerator {
  public class LogInterception : IInterception {
    public void Before() {
        Console.WriteLine("=== BEFORE ===");
    }

    public void After() {
        Console.WriteLine("=== AFTER ===");
    }
  }
}
