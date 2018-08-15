namespace SogetiViewAutomation
{
    using System;
    using System.Threading;
    using System.Windows.Automation;


    // All possible patterns for UIAutomation.
    public enum Pattern { Window, Value, Toggle, DoubleToggle, Invoke, SelectionItem, Expand, Collapse };

    public static class ViewTree
    {
        private const int DELAY = 100; // Miliseconds.


        /// <summary>
        /// Finds the first occurrence of a child node based on two conditions (second one may be null) and use a pattern if provided.
        /// </summary>
        /// <param name="node">The automation element.</param>
        /// <param name="conditions">Array of condition.</param>
        /// <param name="pattern">The pattern to be used.</param>
        /// <param name="value">In case of ValuePattern, the value to be set.</param>
        /// <returns>The pattern retrieved from the node.</returns>
        public static BasePattern RetrieveChildNodePatternByCondition(ref AutomationElement node, Condition[] conditions, bool usePattern = false, Pattern pattern = Pattern.Value, string value = "")
        {
            Thread.Sleep(DELAY);
            node = conditions.Length > 1 ? node.FindFirst(TreeScope.Children, new AndCondition(conditions)) : node.FindFirst(TreeScope.Children, conditions[0]);

            if (node == null)
            {
                throw new Exception("Child node not found.");
            }

            Thread.Sleep(DELAY);
            return usePattern ? (BasePattern)UsePattern(node, pattern, value) : null;
        }

        /// <summary>
        /// Finds the child node at index "n" and use a pattern if provided.
        /// </summary>
        /// <param name="node">The automation element.</param>
        /// <param name="index">The index.</param>
        /// <param name="usePattern">Whether a pattern will be provided and used.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="value">In case of ValuePattern, the value to be set.</param>
        /// <returns>The pattern retrieved from the node.</returns>
        public static BasePattern RetrieveChildNodePatternByIndex(ref AutomationElement node, int index, bool usePattern = false, Pattern pattern = Pattern.Value, string value = "")
        {
            Thread.Sleep(DELAY);
            TreeWalker walker = new TreeWalker(Condition.TrueCondition);
            node = walker.GetFirstChild(node);

            for (int i = 1; i < index; i++)
            {
                node = walker.GetNextSibling(node);
            }

            if (node == null)
            {
                throw new Exception("Child node not found.");
            }

            Thread.Sleep(DELAY);
            return usePattern ? (BasePattern)UsePattern(node, pattern, value) : null;
        }

        /// <summary>
        /// Gets the next sibling node and use a pattern if provided.
        /// </summary>
        /// <param name="node">The automation element.</param>
        /// <param name="usePattern">Whether a pattern will be provided and used.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="value">In case of ValuePattern, the value to be set.</param>
        /// <returns>The pattern retrieved from the node.</returns>
        public static BasePattern RetrieveNextSiblingNode(ref AutomationElement node, bool usePattern = false, Pattern pattern = Pattern.Value, string value = "")
        {
            Thread.Sleep(DELAY);
            TreeWalker walker = new TreeWalker(Condition.TrueCondition);
            node = walker.GetNextSibling(node);

            if (node == null)
            {
                throw new Exception("Sibling node not found.");
            }

            Thread.Sleep(DELAY);
            return usePattern ? (BasePattern)UsePattern(node, pattern, value) : null;
        }

        /// <summary>
        /// Finds all occurrence of child nodes based on a list of conditions.
        /// </summary>
        /// <param name="node">Automation element.</param>
        /// <param name="conditions">Array of conditions.</param>
        /// <returns></returns>
        public static AutomationElementCollection RetrieveAllChildNodes(AutomationElement node, Condition[] conditions)
        {
            AutomationElementCollection nodes = null;
            nodes = conditions.Length > 1 ? node.FindAll(TreeScope.Children, new AndCondition(conditions)) : node.FindAll(TreeScope.Children, conditions[0]);

            if (nodes == null)
            {
                throw new Exception("Child nodes not found.");
            }

            return nodes;
        }

        /// <summary>
        /// Set focus on an automation element.
        /// </summary>
        /// <param name="node">The automation element.</param>
        public static void Focus(AutomationElement node)
        {
            node.SetFocus();
            Thread.Sleep(DELAY);
        }

        /// <summary>
        /// Use a pattern on an automation element.
        /// </summary>
        /// <param name="node">The automation element.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="value">In case of ValuePattern, the value to be set.</param>
        /// <returns></returns>
        public static object UsePattern(AutomationElement node, Pattern pattern, string value = "")
        {
            object objectPattern = null;
            switch (pattern)
            {
                case Pattern.Window:
                    objectPattern = node.GetCurrentPattern(WindowPattern.Pattern);
                    break;
                case Pattern.Value:
                    objectPattern = node.GetCurrentPattern(ValuePattern.Pattern);
                    ((ValuePattern)objectPattern).SetValue(value);
                    break;
                case Pattern.Toggle:
                    objectPattern = node.GetCurrentPattern(TogglePattern.Pattern);
                    ((TogglePattern)objectPattern).Toggle();
                    break;
                case Pattern.DoubleToggle:
                    objectPattern = node.GetCurrentPattern(TogglePattern.Pattern);
                    ((TogglePattern)objectPattern).Toggle();
                    Thread.Sleep(DELAY);
                    ((TogglePattern)objectPattern).Toggle();
                    break;
                case Pattern.Invoke:
                    objectPattern = node.GetCurrentPattern(InvokePattern.Pattern);
                    ((InvokePattern)objectPattern).Invoke();
                    break;
                case Pattern.SelectionItem:
                    objectPattern = node.GetCurrentPattern(SelectionItemPattern.Pattern);
                    ((SelectionItemPattern)objectPattern).Select();
                    break;
                case Pattern.Expand:
                    objectPattern = node.GetCurrentPattern(ExpandCollapsePattern.Pattern);
                    ((ExpandCollapsePattern)objectPattern).Expand();
                    break;
                case Pattern.Collapse:
                    objectPattern = node.GetCurrentPattern(ExpandCollapsePattern.Pattern);
                    ((ExpandCollapsePattern)objectPattern).Collapse();
                    break;
            }

            Thread.Sleep(DELAY);
            return objectPattern;
        }
    }
}