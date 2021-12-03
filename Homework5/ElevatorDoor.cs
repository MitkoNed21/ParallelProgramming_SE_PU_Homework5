using static Homework5.TODOClass;

namespace Homework5
{
    public class ElevatorDoor
    {
        private bool isOpen = false;

        public Elevator Elevator { get; }
        public Floor Floor { get; }
        public ElevatorButton CallElevatorButton { get; }

        public bool IsOpen => isOpen;

        public ElevatorDoor(Elevator elevator, Floor floor)
        {
            this.Elevator = elevator;
            this.Floor = floor;

            this.CallElevatorButton = new ElevatorButton(elevator, elevatorDoor: this);
        }

        public bool Open()
        {
            if (!this.Elevator.IsOccupied)
            {
                this.isOpen = true;
            }
            else if (this.Elevator.Passenger.SecurityLevel >= this.Floor.SecurityLevelRequirement)
            {
                this.isOpen = true;
            }
            else
            {
                this.isOpen = false;
            }

            return this.isOpen;
        }

        public bool Close()
        {
            this.isOpen = false;
            return true;
        }
    }
}