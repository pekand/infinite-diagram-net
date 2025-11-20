using Diagram;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Globalization;
using System.Text.RegularExpressions;

#nullable disable

namespace Plugin
{
    public partial class CalcPlugin : IKeyPressPlugin
    {
        public string Name
        {
            get
            {
                return "Calc Plugin";
            }
        }

        public string Version
        {
            get
            {
                return "1.0";
            }
        }

        private string location = null;

        public void SetLocation(string location)
        {
            this.location = location;
        }

        private Log log = null;

        public void SetLog(Log log)
        {
            this.log = log;
        }

        public bool KeyPressAction(Diagram.Diagram diagram, DiagramView diagramView, Keys keyData)
        {

            if (KeyMap.ParseKey("CTRL+G", keyData))  // [KEY] [CTRL+G] Evaluate expresion or generate random value
            {
                if (diagramView.selectedNodes.Count == 0)
                {
                    this.Random(diagramView);
                }
                else
                {
                    this.EvaluateExpression(diagramView);
                }
            }

            return false;
        }

        // NODE random
        public void Random(DiagramView diagramView)
        {
            Node node = diagramView.CreateNode(diagramView.GetMousePosition(), true);
            node.SetName(Randomizer.GetRandomString());

            diagramView.diagram.Unsave("create", node, diagramView.shift, diagramView.currentLayer.id);
            diagramView.diagram.InvalidateDiagram();
        }

        public String Evaluate(String expression)
        {
            try
            {
                var result = CSharpScript.EvaluateAsync<int>(expression).Result; ;
                return result.ToString();
            }
            catch (Exception ex)
            {
                Program.log.Write("CalcPlugin evaluation error: " + ex.Message);
            }

            return "";
        }

        [GeneratedRegex(@"^\d+$")]
        private static partial Regex IntMatch();

        [GeneratedRegex(@"([-]{0,1}\d+[\.,]{0,1}\d*)", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex NumberMatch();

        // NODE evaluate masth expression
        public bool EvaluateExpression(DiagramView diagramView)
        {
            if (diagramView.selectedNodes.Count == 1) //evaluate as math expression
            {
                string expression = diagramView.selectedNodes[0].name;

                if (IntMatch().IsMatch(expression))
                {
                    expression += "+1";
                }

                string expressionResult = this.Evaluate(expression);

                if (expressionResult != "")
                {
                    Node newrec = diagramView.CreateNode(diagramView.GetMousePosition());
                    newrec.SetName(expressionResult);
                    newrec.color.Set("#8AC5FF");

                    diagramView.diagram.InvalidateDiagram();
                }

                return true;
            }
            else
            if (diagramView.selectedNodes.Count > 1)  // SUM sum nodes with numbers
            {
                float sum = 0;
                foreach (Node rec in diagramView.selectedNodes)
                {
                    Match match = NumberMatch().Match(rec.name);
                    if (match.Success)
                    {
                        sum += float.Parse(match.Groups[1].Value.Replace(",", "."), CultureInfo.InvariantCulture);
                    }
                }

                Node newrec = diagramView.CreateNode(diagramView.GetMousePosition());
                newrec.SetName(sum.ToString());
                newrec.color.Set("#8AC5FF");

                diagramView.diagram.InvalidateDiagram();
                return true;
            }

            return false;
        }
    }
}
