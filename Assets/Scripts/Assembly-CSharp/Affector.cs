public class Affector
{
	protected EffectNode Node;

	public Affector(EffectNode node)
	{
		Node = node;
	}

	public virtual void Update()
	{
	}

	public virtual void Reset()
	{
	}
}
