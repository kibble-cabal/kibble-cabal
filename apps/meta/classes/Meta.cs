using System.Linq;
using Godot;

public partial class Meta : Node
{
	[Signal]
	public delegate void SubSystemReadyEventHandler(Node system);

	[Signal]
	public delegate void AllSubSystemsReadyEventHandler();

	private Node[] SubSystems = [
		SaveSubSystem.Instance,
		LocationSubSystem.Instance,
		DateTimeSubSystem.Instance,
		ExpansionPackSubSystem.Instance,
		GameModeSubSystem.Instance,
		ModSubSystem.Instance,
		MusicSubSystem.Instance
	];

	public override void _EnterTree()
	{
		foreach (var system in SubSystems)
		{
			system.Ready += () => OnSubSystemReady(system);
			AddChild(system);
		}
	}

	public override void _Ready() { }

	private void OnSubSystemReady(Node system)
	{
		EmitSignal(SignalName.SubSystemReady, [system]);
		if (SubSystems.All(node => node.IsNodeReady()))
			EmitSignal(SignalName.AllSubSystemsReady);
	}
}
