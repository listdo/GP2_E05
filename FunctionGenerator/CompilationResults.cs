using System.Text;
using System.CodeDom.Compiler;

namespace FunctionGenerator {
  public class CompilationResults {
    private CompilerResults compilerResults = null;
    private string code = null;

    public bool HasErrors { get; private set; }
    public string ErrorsExplanation { get; private set; } = null;

    public CompilationResults(CompilerResults compilerResults, string code) {
      this.compilerResults = compilerResults;
      this.code = code;

      if (compilerResults.Errors.HasErrors) {
        string[] lines = code.Split('\n');

        StringBuilder header = new StringBuilder();
        header.Append("An error occurred while compiling functions and / or terminal definitions:");

        StringBuilder debugCode = new StringBuilder();
        for (int i = 0; i < lines.Length; i++) { // add line numbers
          debugCode.Append((i + 1) + " |" + lines[i] + "\n");
        }

        StringBuilder errorMessage = new StringBuilder();
        foreach (CompilerError error in compilerResults.Errors) {
          errorMessage.Append("line " + error.Line.ToString());
          errorMessage.Append(" column " + error.Column.ToString());
          errorMessage.Append(": " + error.ErrorText + "\r\n");
        }
        ErrorsExplanation = header.ToString() + "\n\n" + errorMessage.ToString() + "\n" + debugCode.ToString();
        HasErrors = true;
      } else {
        HasErrors = false;
        ErrorsExplanation = null;
      }
    }
  }
}
