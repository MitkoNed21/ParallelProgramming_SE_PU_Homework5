using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework5
{
    public class Floor
    {
        public string Name { get; set; }
        public int Position { get; }
        public SecurityLevel SecurityLevelRequirement { get; }
        public Building Building { get; }
        public ElevatorDoor ElevatorDoor { get; }
        public Queue<Agent> ElevatorQueue { get; } = new Queue<Agent>();

        public Floor(int position, string name, SecurityLevel securityLevelRequirement, Building building)
        {
            this.Position = position;
            this.Name = name;
            this.SecurityLevelRequirement = securityLevelRequirement;

            this.Building = building;

            this.ElevatorDoor = new ElevatorDoor(building.Elevator, floor: this);
        }
    }
}
