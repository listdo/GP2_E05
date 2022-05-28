using System;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace FunctionGenerator
{
    public delegate double TerminalEvaluation(double[][] data, int sampleIndex, int variableIndex, double coefficient);
    public delegate double FunctionEvaluation(double[] parameters);

    public class FunctionalBasis
    {
        public static TerminalEvaluation CompileTerminal(string methodCode, out CompilationResults compilationResults)
        {
            var namespaceName = typeof(FunctionEvaluation).Namespace;
            var className = "Definitions";
            var methodName = nameof(FunctionEvaluation);
            var resultType = "double";
            
            // ---- Changed that line
            var argumentsCode = "double[][] data, int variableIndex, int sampleIndex, double coefficient";           
            
            var code = Auxiliary.CompilableCode(namespaceName, className, methodName, resultType, argumentsCode, methodCode);
            
            var cp = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                TreatWarningsAsErrors = true
            };
            
            var provider = new CSharpCodeProvider();
            var results = provider.CompileAssemblyFromSource(cp, code);
            
            TerminalEvaluation terminalEvaluation = null;
            
            if (!results.Errors.HasErrors)
            {
                var assembly = results.CompiledAssembly;
                var type = assembly.GetType($"{namespaceName}.{className}");
                var methodInfo = type.GetMethod(methodName);
            
                terminalEvaluation = (TerminalEvaluation) Delegate.CreateDelegate(typeof(TerminalEvaluation), methodInfo);
            }
            
            compilationResults = new CompilationResults(results, code);
            return terminalEvaluation;
        }

        public static FunctionEvaluation CompileFunction(string methodCode, out CompilationResults compilationResults)
        {
            var namespaceName = typeof(FunctionEvaluation).Namespace;
            var className = "Definitions";
            var methodName = nameof(FunctionEvaluation);
            var resultType = "double";
            var argumentsCode = "double[] args";


            var code = Auxiliary.CompilableCode(namespaceName, className, methodName, resultType, argumentsCode, methodCode);

            var cp = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                TreatWarningsAsErrors = true
            };

            var provider = new CSharpCodeProvider();
            var results = provider.CompileAssemblyFromSource(cp, code);

            FunctionEvaluation functionEvaluation = null;

            if (!results.Errors.HasErrors)
            {
                var assembly = results.CompiledAssembly;
                var type = assembly.GetType($"{namespaceName}.{className}");
                var methodInfo = type.GetMethod(methodName);

                functionEvaluation = (FunctionEvaluation) Delegate.CreateDelegate(typeof(FunctionEvaluation), methodInfo);
            }

            compilationResults = new CompilationResults(results, code);
            return functionEvaluation;
        }
    }
}
