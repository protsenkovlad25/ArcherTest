namespace ArcherTest.Core.Interfaces
{
    public interface IPoolable
    {
        delegate void Disabled(IPoolable poolable);

        event Disabled OnDisabled;

        void OnActivate();
        void OnDeactivate();
        void Disable();
    }
}
