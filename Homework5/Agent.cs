using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static Homework5.TODOClass;

namespace Homework5
{
    public class Agent
    {
        //private bool atWork = false;
        private Floor currentFloor;

        private enum AgentActions
        {
            DoWork, UseElevator, LeaveWork, TakeBreak
        }

        private Random random = new Random();

        public Building Workplace { get; }
        public string Name { get; set; }

        public bool AtWork => this.Workplace.WorkingAgents.Contains(this);
        public Floor CurrentFloor => currentFloor;

        public SecurityLevel SecurityLevel { get; set; }

        public Agent(Building workplace, string name, SecurityLevel securityLevel)
        {
            this.Workplace = workplace;
            this.Name = name;
            this.SecurityLevel = securityLevel;
        }

        private AgentActions GetRandomAgentAction()
        {
            var n = random.Next(100);

            if (n > 90) return AgentActions.LeaveWork;
            if (n > 80) return AgentActions.UseElevator; // 5
            if (n > 74) return AgentActions.TakeBreak; // 4
            return AgentActions.DoWork;
        }

        public void DoThingsAtWork() // change Name
        {
            this.GoToWork();

            while (this.AtWork)
            {
                var nextAction = GetRandomAgentAction();

                switch (nextAction)
                {
                    case AgentActions.DoWork:
                        this.Work();
                        break;
                    case AgentActions.UseElevator:
                        int desiredFloorNumber;

                        do
                        {
                            desiredFloorNumber = random.Next(0, this.Workplace.Floors.Length);
                        }
                        while (desiredFloorNumber == this.CurrentFloor.Position);
                        
                        this.UseElevator(desiredFloorNumber);
                        break;
                    case AgentActions.LeaveWork:
                        this.LeaveWork();
                        break;
                    case AgentActions.TakeBreak:
                        this.TakeBreak();
                        break;
                    default:
                        throw new NotImplementedException();
                }

                Thread.Sleep(100);
            }
        }

        public void UseElevator(int desiredFloorNumber)
        {
            var canExit = false;
            var newFloorNumber = desiredFloorNumber;
            var targetFloor = this.Workplace.Floors[newFloorNumber];

            Console.WriteLine($"{Name} decided to use the elevator to go to floor {targetFloor.Name} from floor {currentFloor.Name}.");

            this.currentFloor.ElevatorDoor.CallElevatorButton.CallElevator();
            lock (this.currentFloor)
            {
                this.currentFloor.ElevatorQueue.Enqueue(this);
            }

            var waitingForElevator = true;
            while (waitingForElevator)
            {
                //TODO("Wait for elevator to come...");
                Agent nextInQueue;
                lock (this.currentFloor)
                {
                    nextInQueue = this.currentFloor.ElevatorQueue.Peek();
                }
                while (nextInQueue != this ||
                        this.Workplace.Elevator.CurrentFloor.Position != currentFloor.Position ||
                       !this.Workplace.Elevator.IsDoorOpen)
                {
                    Thread.Sleep(100);
                }

                // Enter the elevator
                lock (this.currentFloor)
                {
                    if (!this.Workplace.Elevator.IsOccupied &&
                         this.Workplace.Elevator.IsDoorOpen &&
                         this.Workplace.Elevator.CurrentFloor.Position == currentFloor.Position)
                    {
                        this.Workplace.Elevator.Passenger = this;
                        waitingForElevator = false;
                    }

                    //this.Workplace.Elevator.Passenger = this;
                }
            }

            Console.WriteLine($"{Name} is in the elevator.");

            while (!canExit)
            {
                this.Workplace.Elevator.ElevatorButtons[newFloorNumber].CallElevator();

                //TODO("Wait for elevator to arrive...");
                while (this.Workplace.Elevator.CurrentFloor.Position != newFloorNumber/* ||
                      !this.Workplace.Elevator.IsDoorOpen*/)
                {
                    //if (this.Workplace.Elevator.)
                    Thread.Sleep(100);
                }
                Console.WriteLine($"{Name} reached the desired floor.");

                //TODO("Should wait for open door? Or maybe just wait (50ms?) after while");
                // Wait for the door to try to open
                Thread.Sleep(20);

                canExit = this.Workplace.Elevator.IsDoorOpen;

                if (!canExit)
                {
                    Console.WriteLine($"{Name} could not enter the desired floor because their security level is too low.");
                    
                    newFloorNumber = random.Next(0, this.Workplace.Floors.Length);
                    targetFloor = this.Workplace.Floors[newFloorNumber];

                    Console.WriteLine($"{Name} decided to go to floor {targetFloor.Name}.");
                }
            }


            lock (this.currentFloor)
            {
                this.Workplace.Elevator.Passenger = null;
                while (this.Workplace.Elevator.IsDoorOpen)
                {
                    Thread.Sleep(20);
                }
                this.currentFloor.ElevatorQueue.Dequeue();
            }

            this.currentFloor = this.Workplace.Floors[newFloorNumber];
            Console.WriteLine($"{Name} is now on floor {targetFloor.Name}.");
        }

        public void Work()
        {
            if (!this.AtWork)
            {
                throw new InvalidOperationException($"{Name} is not at work!");
            }

            Console.WriteLine($"{Name} is working...");
        }

        public void GoToWork()
        {
            if (this.AtWork)
            {
                throw new InvalidOperationException($"{Name} is already at work!");
            }

            this.Workplace.RegisterAgentStartedWorking(this);
            this.currentFloor = this.Workplace.Floors[0];

            Console.WriteLine($"{Name} ({SecurityLevel}) went to work.");
        }

        public void LeaveWork()
        {
            if (!this.AtWork)
            {
                throw new InvalidOperationException($"{Name} is not at work!");
            }

            Console.WriteLine($"{Name} finished work for the day.");
            
            if (this.currentFloor.Position != 0)
            {
                this.UseElevator(desiredFloorNumber: 0);
            }

            this.Workplace.RegisterAgentLeftWork(this);

            Console.WriteLine($"{Name} left work.");
        }

        private void TakeBreak()
        {
            if (!this.AtWork)
            {
                throw new InvalidOperationException($"{Name} is not at work!");
            }

            Console.WriteLine($"{Name} is taking a break.");
            Thread.Sleep(3000);
            Console.WriteLine($"{Name} finished their break. They are now refreshed!");
        }

    }
}
