using System; 

namespace Delen.Core.Entities
{
    public abstract class Entity
    {
        

        protected Entity()
        {
            CreationDate = DateTime.Now;
            Activate();
        }

        public bool Active { get; private set; }

        public void Deactivate()
        {
            this.Active = false;
        }

        public int Id { get; set; }

        protected bool Equals(Entity other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Entity) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public void Activate()
        {
            Active = true;
        }

        public DateTime? CreationDate { get; protected set; }
    }
}