using System.Linq;
using Godot;

namespace KibbleCabal.Apps.Quest.UI
{
    public partial class QuestListUI : ScrollContainer
    {
        [Export]
        public VBoxContainer? Container;

        public override void _Ready() => Render();

        public void Render()
        {
            this.QueueFreeChildren();
            QuestDB.Resources
                .Where(quest => quest.IsAvailable())
                .ForEach(quest => Container?.AddChild(QuestUI.Instantiate(quest)));
        }
    }
}