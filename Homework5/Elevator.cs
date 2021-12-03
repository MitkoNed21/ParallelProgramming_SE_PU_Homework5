using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static Homework5.TODOClass;

namespace Homework5
{
    public class Elevator
    {
        private Queue<int> callHistory = new Queue<int>();
        private int transportingFloorPosition = -1;
        private Floor currentFloor;
        private bool isMoving = false;
        private bool isSetup = false;

        public Building Building { get; }
        public ElevatorButton[] ElevatorButtons { get; }
        public Floor CurrentFloor => currentFloor; // throw if moving?

        //public bool IsMoving => isMoving;
        public bool IsDoorOpen => this.currentFloor.ElevatorDoor.IsOpen;

        public bool IsOccupied => Passenger is not null;

        public Agent Passenger { get; set; }

        public Elevator(Building building)
        {
            this.Building = building;

            this.ElevatorButtons = new ElevatorButton[this.Building.Floors.Length];
        }

        private void Setup()
        {
            for (int i = 0; i < this.Building.Floors.Length; i++)
            {
                this.ElevatorButtons[i] = new ElevatorButton(elevator: this, floor: Building.Floors[i]);
            }

            this.currentFloor = this.ElevatorButtons[0].Floor;
            isSetup = true;
            Console.WriteLine("ELEVATOR IS NOW SET UP AND READY FOR USE!");
        }

        public void MoveToFloor(Floor floor, ElevatorButton callingButton)
        {
            MoveToFloor(floor.Position, callingButton);

            //if (floor.Position == this.currentFloor.Position) return;

            //callHistory.Enqueue(floor.Position);
        }
        public void MoveToFloor(int floorPosition, ElevatorButton callingButton)
        {
            // wait for elevator to be setup
            while (!this.isSetup) ;

            var hasPassenger = false;
            lock (this.currentFloor)
            {
                hasPassenger = Passenger is not null;
            }
            // If there is a passenger and someone calls the elevator from outside,
            // then add the call to the call history. Otherwise, the passenger pressed
            // a button inside the elevator.
            if (!hasPassenger || callingButton.ElevatorDoor is not null) 
            {
                lock (this)
                {
                    callHistory.Enqueue(floorPosition);
                }
            }
            else
            {
                transportingFloorPosition = floorPosition;
            }

            /**
             *   if (this.Passenger is null)
             *   { 
             *       lock (this)
             *       {
             *           callHistory.Enqueue(floorPosition);
             *       }
             *   }
             *   else if (callingButton.ElevatorDoor is not null)
             *   { 
             *       lock (this)
             *       {
             *           callHistory.Enqueue(floorPosition);
             *       }
             *   }
             *   else
             *   {
             *       transportingFloorPosition = floorPosition;
             *   }
             **/
        }

        public void StartWorking()
        {
            if (!isSetup) Setup();

            while (this.Building.WorkingAgents.Count != 0)
            {
                Move();
                Thread.Sleep(20);
            }
        }

        private void Move() //TODO: Move moves elevator, MoveToFloor adds floor to callList. How to trigger move?
        {
            //Console.WriteLine("DEBUG: ELEVATOR MOVE()");
            //if (this.isMoving) return;
            //TODO("When elevator is moving set isMoving to true");

            int callHistoryCount;
            lock (this)
            {
                callHistoryCount = this.callHistory.Count;
            }

            //Console.WriteLine($"DEBUG: CALLHISTORY: {callHistoryCount}");
            if (callHistoryCount == 0) return;

            int nextFloorPosition;
            while (callHistoryCount > 0)
            {
                lock (this)
                {
                    nextFloorPosition = callHistory.Dequeue();
                }

                while (true)
                {
                    if (nextFloorPosition != this.currentFloor.Position)
                    {
                        var floorOffset = 1; // going up
                        if (nextFloorPosition < this.currentFloor.Position)
                        {
                            floorOffset = -1; // going down
                        }

                        while (this.currentFloor.Position != nextFloorPosition)
                        {
                            Thread.Sleep(1000);

                            this.currentFloor = this.Building.Floors[this.currentFloor.Position + floorOffset];

                            var elevatorResponse = $"Elevator is now at floor {this.currentFloor.Name}";
                            if (this.IsOccupied)
                            {
                                elevatorResponse += $" with {this.Passenger.Name}";
                            }

                            elevatorResponse += ".";

                            Console.WriteLine(elevatorResponse);

                        }
                    }

                    this.currentFloor.ElevatorDoor.Open();

                    if (this.IsDoorOpen)
                    {
                        Console.WriteLine("DEBUG: ELEVATOR OPENED DOORS");

                        if (this.IsOccupied)
                        {
                            //TODO("Wait for agent to leave"); // or choose a floor...
                            while (Passenger is not null)
                            {
                                Thread.Sleep(20);
                            }

                            this.currentFloor.ElevatorDoor.Close();
                            Console.WriteLine("DEBUG: ELEVATOR CLOSED DOORS");
                            break;
                        }
                        else
                        {
                            //TODO("Wait for agent to enter");
                            var waitTimeMs = 20;
                            var totalTimeWaited = 0;
                            while (Passenger is null)
                            {
                                Thread.Sleep(waitTimeMs);
                                totalTimeWaited += waitTimeMs;

                                //TODO("Wait some time and if no one comes in, close the door (break; loop)");
                                if (totalTimeWaited > waitTimeMs * 20)
                                {
                                    break;
                                }
                            }

                            this.currentFloor.ElevatorDoor.Close();

                            if (totalTimeWaited > waitTimeMs * 20)
                            {
                                Console.WriteLine("DEBUG: NO ONE GOT IT, ELEVATOR CLOSED DOORS");
                                
                                // after door closes keep on with call history
                                break;
                            }

                            Console.WriteLine($"DEBUG: ELEVATOR CLOSED DOORS. {Passenger.Name} IS INSIDE");

                            //TODO("Wait for agent to choose floor...");
                            while (transportingFloorPosition == -1) // no floor 
                            {
                                Thread.Sleep(20);
                            }

                            //TODO("Set nextFloorPosition");
                            nextFloorPosition = transportingFloorPosition;
                            transportingFloorPosition = -1;
                        }
                    }
                    else
                    {
                        //TODO("Wait for agent to select another floor...");
                        while (transportingFloorPosition == -1) // no floor
                        {
                            Thread.Sleep(20);
                        }

                        //TODO("Set nextFloorPosition");
                        nextFloorPosition = transportingFloorPosition;
                        transportingFloorPosition = -1;
                    }
                }

                lock (this)
                {
                    callHistoryCount = this.callHistory.Count;
                }
            }
        }
    }
}
