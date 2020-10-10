using System;
using System.Collections.Generic;
using System.Linq;

namespace Elevator
{
    class Program
    {
        public class Passenger
        {
            public int FloorEnter { get; private set; }
            public int FloorExit { get; private set; }
            public bool IsUp { get; private set; }

            public Passenger(int floorEnter, int floorExit, bool isUp)
            {
                FloorEnter = floorEnter;
                FloorExit = floorExit;
                IsUp = isUp;
            }

            public void SetRouteFloor(int currentFloor, int floors)
            {
                FloorEnter = FloorExit;
                (IsUp, FloorExit) = RandomFloor(currentFloor, floors);
            }
        }

        static (bool, int) RandomFloor(int currentFloor, int floors)
        {
            bool isUp;
            Random random = new Random();

            if (currentFloor == 1)
                isUp = true;
            else if (currentFloor == floors)
                isUp = false;
            else
                isUp = random.Next(100) < 50;

            return (isUp, random.Next(isUp ? currentFloor + 1 : 1, isUp ? floors + 1 : currentFloor));
        }

        static void Main()
        {
            const int Capacity = 5;

            Random random = new Random();

            //Floors
            int n = random.Next(5, 21);

            List<Passenger> passengers = new List<Passenger>();
            List<Passenger> inLift = new List<Passenger>();
            List<Passenger> outLift = new List<Passenger>();

            int newFloor;
            bool isUp;

            //passengers initializing
            for (int floorEnter = 1; n >= floorEnter; floorEnter++)
            {
                int k = random.Next(0, 11);

                for (int i = 1; k >= i; i++)
                {
                    (isUp, newFloor) = RandomFloor(floorEnter, n);

                    passengers.Add(new Passenger(floorEnter, newFloor, isUp));
                }
            }

            int floor = 1;
            isUp = true;
            int freeCapacity;

            while (passengers.Count() > 0)
            {
                if (n == floor)
                    isUp = false;
                else if (floor == 1)
                    isUp = true;

                //check if passengers out
                if (inLift.Where(i => i.FloorExit == floor).Count() > 0)
                {
                    outLift = inLift.Where(i => i.FloorExit == floor).ToList();
                    inLift.RemoveAll(i => outLift.Any(k => k.GetHashCode() == i.GetHashCode()));
                }

                freeCapacity = Capacity - inLift.Count();
                //is capacity free for new passengers
                if (freeCapacity > 0)
                {
                    if (inLift.Count() == 0 && passengers.Where(i => i.FloorEnter == floor).Count() > 0
                        && passengers.Where(i => i.FloorEnter == floor && i.IsUp == !isUp).Count() > passengers.Where(i => i.FloorEnter == floor && i.IsUp == isUp).Count())
                        passengersToLift(true);
                    else
                        passengersToLift();
                }

                //set new floor for out passengers
                if (outLift.Count() > 0)
                {
                    List<Passenger> l = passengers.Where(i => outLift.Any(k => k.GetHashCode() == i.GetHashCode())).ToList();
                    l.ForEach(i => i.SetRouteFloor(floor, n));
                    outLift.Clear();
                }

                //route reverse
                if (n != floor && floor != 1 && inLift.Count() == 0)
                {
                    if (isUp && passengers.Where(i => i.FloorEnter > floor).Count() == 0)
                        isUp = false;
                    else if (!isUp && passengers.Where(i => i.FloorEnter < floor).Count() == 0)
                        isUp = true;
                }

                if (isUp)
                    floor++;
                else
                    floor--;
            }

            void passengersToLift(bool reverse = false)
            {
                if (reverse)
                    isUp = !isUp;

                inLift.AddRange(passengers.Where(i => i.FloorEnter == floor && i.IsUp == isUp).Take(freeCapacity).ToList());
            }
        }
    }
}

