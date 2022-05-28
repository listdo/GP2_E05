using System.Text;

namespace FunctionGenerator {
  public static class Auxiliary {
    public static readonly string LineBreak = "\n";
    public static readonly string Indent = "  ";

    #region CompilableCode
    public static string CompilableCode(string namespaceName, string className, string methodName,
                                        string resultType, string argumentsCode, string methodCode) {
      StringBuilder sb = new StringBuilder();
      sb.Append("using System;");
      sb.Append(LineBreak);
      sb.Append("namespace ");
      sb.Append(namespaceName);
      sb.Append("{");
      sb.Append(LineBreak);
      sb.Append(Indent + "public class ");
      sb.Append(className);
      sb.Append("{");
      sb.Append(LineBreak);

      string methodsCode = MethodsCode(methodName, resultType, argumentsCode, methodCode);
      sb.Append(methodsCode);

      sb.Append(Indent);
      sb.Append("}");
      sb.Append(LineBreak);
      sb.Append("}");
      return sb.ToString();
    }
    #endregion

    #region MethodsCode
    public static string MethodsCode(string methodName, string resultType, string argumentsCode, string methodCode) {
      StringBuilder sb = new StringBuilder();
      sb.Append(ComposedIndent(2));
      sb.Append("public static ");
      sb.Append(resultType);
      sb.Append(" ");
      sb.Append(methodName);
      sb.Append("(");
      sb.Append(argumentsCode);
      sb.Append("){");
      sb.Append(LineBreak);

      string[] methodCodeLines = methodCode.Split('\n');
      foreach (string line in methodCodeLines) {
        sb.Append(ComposedIndent(3));
        sb.Append(line);
        sb.Append(LineBreak);
      }

      sb.Append(ComposedIndent(2));
      sb.Append("}");
      sb.Append(LineBreak);
      return sb.ToString();
    }

    public static string ComposedIndent(int indents) {
      StringBuilder sb = new StringBuilder(Indent);
      for (int i = 0; i < indents - 1; i++)
        sb.Append(Indent);
      return sb.ToString();
    }
    #endregion
  }
}
