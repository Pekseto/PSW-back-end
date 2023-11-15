﻿using System.ComponentModel.DataAnnotations.Schema;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Tours.Core.Domain.Tours;
using Explorer.Tours.Core.Domain.Utilities;
using Newtonsoft.Json;
using static Explorer.Tours.API.Enums.TourEnums;

namespace Explorer.Tours.Core.Domain.TourExecutions
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TourExecution: JsonEntity
    {
        [NotMapped]
        private const int PointProximity = 100;
        [NotMapped][JsonProperty]
        public TourExecutionStatus Status { get; set; }
        [NotMapped][JsonProperty]
        public Position? Position { get; set; }
        [NotMapped][JsonProperty]
        public List<PointTask>? Tasks { get; set; } = new List<PointTask>();
        [NotMapped][JsonProperty]
        public int TourId { get; set; }
        
        public TourExecution(){}

        [JsonConstructor]
        public TourExecution(TourExecutionStatus status, Position position, List<PointTask> tasks)
        {
            Status = status;
            Position = position;
            Tasks = tasks;
        }
        public TourExecution(List<Point> points, int tourId)
        {
            Validate(points);
            PrepareInitialPosition(points);
            PrepareTasks(points);
            Status = TourExecutionStatus.Active;
            TourId=tourId;
        }
        public void UpdatePosition(Position currentPosition)
        {
            if (Status != TourExecutionStatus.Active) return;
            if (!Position.IsChanged(currentPosition)) return;
            Position = currentPosition;
            var currentPoint = Tasks.FirstOrDefault(x => !x.Done) ?? throw new Exception("Exception! All Tour Points passed!");
            var inProximity = DistanceCalculator.CalculateDistance(currentPoint, currentPosition) * 1000 <= PointProximity;
            if (inProximity)
            {
                currentPoint.Done = true;
                currentPoint.DoneOn = currentPosition.LastActivity;
            }
            Status = Tasks.TrueForAll(x => x.Done) ? TourExecutionStatus.Completed : Status;
        }
        public void QuitTourExecution()
        {
            Status = TourExecutionStatus.Abandoned;
        }
        private static void Validate(IReadOnlyCollection<Point> points)
        {
            if (points.Count < 0) throw new ArgumentException("Exception! No points found!");
        }
        private void PrepareTasks(IReadOnlyCollection<Point> points)
        {
            Tasks = new(
                points.Select(x => new PointTask(x, TaskType.Point, DateTime.MinValue, false)));
        }
        private void PrepareInitialPosition(IReadOnlyCollection<Point> points)
        {
            var firsPoint = points.FirstOrDefault() ?? throw new ArgumentException("Exception! No points found!");
            Position = new Position(firsPoint.Latitude, firsPoint.Longitude, DateTime.MinValue);
        }


        public double PercentageOfDone(TourExecution tourExecution)
        {
            if (tourExecution.Tasks == null || tourExecution.Tasks.Count == 0)
            {
                return 0.0; 
            }

            int totalTasks = tourExecution.Tasks.Count;
            int completedTasks = tourExecution.Tasks.Count(task => task.Done);

            double percentage = (double)completedTasks / totalTasks * 100.0;

            return percentage;
        }


        public bool IsLastActivityWithinWeek(TourExecution tourExecution)
        {
            
            if (tourExecution.Position == null)
            {
                return true;
            }

            
            if (tourExecution.Position.LastActivity == null)
            {
                return true; 
            }

            TimeSpan timeDifference = DateTime.Now - tourExecution.Position.LastActivity;

            
            return timeDifference.TotalDays > 7.0;
        }


        public override void ToJson()
        {
            JsonObject = JsonConvert.SerializeObject(this, Formatting.Indented) ??
                         throw new JsonSerializationException("Exception! Could not serialize object!");
        }

        public override void FromJson()
        {
            var tourExecution = JsonConvert.DeserializeObject<TourExecution>(JsonObject ??
                                                                             throw new NullReferenceException(
                                                                                 "Exception! No object to deserialize!")) ??
                                throw new NullReferenceException("Exception! TourExecution is null!");
            Status = tourExecution.Status;
            Position = tourExecution.Position;
            Tasks = tourExecution.Tasks;
            TourId= tourExecution.TourId;
        }
    }
}
