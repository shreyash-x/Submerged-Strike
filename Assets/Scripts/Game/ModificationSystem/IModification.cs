namespace Game.ModificationSystem
{
    public interface IModification<in T>
    {
        void ApplyOn(T obj);
        void ResetOn(T obj);
    }

    public interface IAllowXModification
    {
        float XMultiplier { get; set; }
    }
    
    public interface IAllowYModification
    {
        float YMultiplier { get; set; }
    }
    
    public class InvertXModification : IModification<IAllowXModification>
    {
        public void ApplyOn(IAllowXModification obj)
        {
            obj.XMultiplier = -1;
        }

        public void ResetOn(IAllowXModification obj)
        {
            obj.XMultiplier = 1;
        }
    }
    
    public class InvertYModification : IModification<IAllowYModification>
    {
        public void ApplyOn(IAllowYModification obj)
        {
            obj.YMultiplier = -1;
        }

        public void ResetOn(IAllowYModification obj)
        {
            obj.YMultiplier = 1;
        }
    }
}