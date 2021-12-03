using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Homework5.TODOClass;

namespace Homework5
{
    public class ElevatorButton
    {
        public Elevator Elevator { get; }
        public ElevatorDoor ElevatorDoor { get; }
        public Floor Floor { get; }

        /// <summary>
        /// Creates an elevator button that would call the <paramref name="elevator"/>
        /// to go to the given floor the <paramref name="elevatorDoor"/> is on.
        /// </summary>
        /// <param name="elevator">The elevator the button is used for.</param>
        /// <param name="elevatorDoor">The elevator door on which the button is.</param>
        public ElevatorButton(Elevator elevator, ElevatorDoor elevatorDoor = null)
        {
            this.Elevator = elevator;
            this.ElevatorDoor = elevatorDoor;
            this.Floor = elevatorDoor.Floor;
        }

        /// <summary>
        /// Creates an elevator button that would call the <paramref name="elevator"/>
        /// to go to the given <paramref name="floor"/>. The button is created without an elevator door,
        /// meaning it is inside the elevator.
        /// </summary>
        /// <param name="elevator">The elevator the button is used for.</param>
        /// <param name="floor">The floor the button is used for.</param>
        public ElevatorButton(Elevator elevator, Floor floor)
        {
            this.Elevator = elevator;
            this.ElevatorDoor = null;
            this.Floor = floor;
        }

        public void CallElevator() // Press()?
        {
            this.Elevator.MoveToFloor(this.Floor, callingButton: this);
        }
    }
}
