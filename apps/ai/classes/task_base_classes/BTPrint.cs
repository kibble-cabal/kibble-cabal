using System.Collections.Generic;
using Godot;

namespace KibbleCabal.Apps.AI.Task
{
    /// <summary>
    /// Base class to print during a BTAction.
    /// </summary>
    [Tool]
    public abstract partial class BTPrint : BTDecorator
    {
        private string _text = "";
        private string[] _formatParams = [];

        [Export(PropertyHint.MultilineText)]
        public string Text
        {
            get => _text;
            set => this.Set(ref _text, value);
        }

        [Export]
        public string[] BlackboardFormatParams
        {
            get => _formatParams;
            set => this.Set(ref _formatParams, value);
        }

        protected bool HasPrinted = false;

        protected void Print()
        {
            if (!HasPrinted)
            {
                Dictionary<string, Variant> formatParams = [];
                foreach (var param in BlackboardFormatParams)
                    formatParams[param] = Blackboard.GetVar(param);
                GD.Print(string.Format(Text, formatParams));
                HasPrinted = true;
            }
        }
    }
}