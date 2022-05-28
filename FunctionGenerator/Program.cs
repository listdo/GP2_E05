using System;
using System.Collections.Generic;

namespace FunctionGenerator {
  public class Program {
        public static void Main(string[] args) {
            Random random = new Random(42);
            int N = 1000;
            double[][] testData = new double[4][];
            for (int i = 0; i < 4; i++) {
                testData[i] = new double[N];
            }
            for (int i = 0; i < N; i++) {
                testData[0][i] = i + random.NextDouble() - 0.5;
                testData[1][i] = Math.Sin(i / 100 * 2 * Math.PI);
                testData[2][i] = testData[0][i] + testData[1][i];
                testData[3][i] = testData[2][i] * testData[1][i];
            }

            CompilationResults compilationResults;
            FunctionEvaluation addition = FunctionalBasis.CompileFunction("double result = args[0]; for(int i=1; i<args.Length; i++) {result += args[i];} return result;", out compilationResults);
            if (compilationResults != null && compilationResults.HasErrors) {
                throw new InvalidProgramException(compilationResults.ErrorsExplanation);
            }

            FunctionEvaluation multiplication = FunctionalBasis.CompileFunction("double result = args[0]; for(int i=1; i<args.Length; i++) {result *= args[i];} return result;", out compilationResults);
            if (compilationResults != null && compilationResults.HasErrors) {
              throw new InvalidProgramException(compilationResults.ErrorsExplanation);
            }

            TerminalEvaluation variable = FunctionalBasis.CompileTerminal("return data[variableIndex][sampleIndex]*coefficient;", out compilationResults);
            if (compilationResults != null && compilationResults.HasErrors) {
              throw new InvalidProgramException(compilationResults.ErrorsExplanation);
            }

            TerminalEvaluation constant = FunctionalBasis.CompileTerminal("return coefficient;", out compilationResults);
            if (compilationResults != null && compilationResults.HasErrors) {
              throw new InvalidProgramException(compilationResults.ErrorsExplanation);
            }

            Node variable0 = new Node(variable, 0, -2.9);
            Node variable1 = new Node(variable, 1, 1.2);
            Node variable2 = new Node(variable, 2, 0.3);

            Node multiplicationNode = new Node(multiplication, new List<Node>(new Node[] { variable0, variable2 }));
            Node additionNode = new Node(addition, new List<Node>(new Node[] { variable1, multiplicationNode }));
            
            double result = additionNode.Evaluate(testData, 10);

            Console.WriteLine(result);

            var arguments = new[] { 1.0, 2.0 };
            var mulArguments = new[] { 2.0, 2.0 };

            var variableIndex = 0;
            var sampleIndex = 1;
            var coefficient = 0.5F;

            var res = addition(arguments);
            var mulRes = multiplication(mulArguments);
            var variableRes = variable(testData, sampleIndex, variableIndex, coefficient);
            var constantRes = constant(testData, sampleIndex, variableIndex, coefficient);

            Console.WriteLine($"{string.Join(" + ", arguments)} = {res}");
            Console.WriteLine($"{string.Join(" * ", mulArguments)} = {mulRes}");
            Console.WriteLine($"{string.Join(" var ", testData, variableIndex, sampleIndex, coefficient)} = {variableRes}");
            Console.WriteLine($"{string.Join(" var ", testData, variableIndex, sampleIndex, coefficient)} = {constantRes}");
        }
  }
}
