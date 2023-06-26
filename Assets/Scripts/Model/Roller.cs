namespace PickMaster.Model
{
    public class Roller
    {
        public RollerModel Model { get; }
        public bool IsPositive { get; set; }
        
        public Roller(RollerModel model, bool isPositive)
        {
            Model = model;
            IsPositive = isPositive;
        }
    }
}