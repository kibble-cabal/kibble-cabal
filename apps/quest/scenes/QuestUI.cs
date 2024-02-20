using Godot;

namespace KibbleCabal.Apps.Quest.UI
{
    public interface IRenderableQuest
    {
        void Render(RQuest quest);
    }

    public partial class QuestUI : HBoxContainer
    {
        public static readonly PackedScene Scene = GD.Load<PackedScene>("res://apps/quest/scenes/quest_ui.tscn");

        [Export]
        public RQuest? Quest;

        [ExportGroup("Nodes")]

        [Export]
        public Label? NameLabel;

        [Export]
        public Label? DescriptionLabel;

        private Control? UI;

        public override void _Ready() => Render();

        public void Render()
        {
            if (Quest is null) return;
            var isComplete = Quest.IsComplete();
            var completeString = isComplete ? "complete" : "incomplete";
            if (NameLabel is not null)
                NameLabel.Text = $"{Quest.DisplayName} ({completeString})";
            if (isComplete)
                Modulate = new Color(Modulate, 0.5f);
            UI?.QueueFree();
            if (Quest.UIScene is not null)
            {
                UI = Quest.UIScene.Instantiate<Control>();
                AddChild(UI);
                if (UI is IRenderableQuest ui)
                    ui.Render(Quest);
            }
        }

        public static QuestUI Instantiate(RQuest quest)
        {
            var scene = Scene.Instantiate<QuestUI>();
            scene.Quest = quest;
            return scene;
        }

    }
}
