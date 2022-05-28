using System.Collections.Generic;

namespace FunctionGenerator {
  public class Node {
    public TerminalEvaluation TerminalEvaluation { get; private set; } = null;
    public FunctionEvaluation FunctionEvaluation { get; private set; } = null;
    public int VariableIndex { get; private set; } = 0;
    public double Coefficient { get; private set; } = 0;
    public List<Node> Children { get; private set; }

    public Node(TerminalEvaluation terminalEvaluation, int variableIndex, double coefficient) {
        TerminalEvaluation = terminalEvaluation;
        VariableIndex = variableIndex;
        Coefficient = coefficient;
    }

    public Node(FunctionEvaluation functionEvaluation, List<Node> children) {
        FunctionEvaluation = functionEvaluation;
        Children = children;
    }

    public double Evaluate(double[][] data, int sampleIndex) {
        if(TerminalEvaluation != null)
            return TerminalEvaluation.Invoke(data, VariableIndex, sampleIndex, Coefficient);

        if(FunctionEvaluation != null)
        {
            var resultList = new List<double>();

            foreach (var item in Children)
                resultList.Add(item.Evaluate(data, sampleIndex));

            return FunctionEvaluation.Invoke(resultList.ToArray());
        }

        return 0.0F;
    }
  }
}
