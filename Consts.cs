using AssignmentsApi.Models.Vehicle;
using System;
using System.Collections.Generic;

namespace AssignmentsApi
{
    public static class Consts
    {
        public const string WorkingTimeDirective = "EC561";
        public const string DefaultDriverStateType = "ExtendedEC561";
        public const string DefaultTimeSpanString = "00:00:00";
        public const string ListItemSeparator = ",";
        public const string AxUnknownCountryCode = "X";
        public const string UnknownCountryZipCode = "XXEU";
        public const string UnknownCountryCode = "XX";
        public const string GebrudeClientId = "AT_GEBRUDE";
        public static readonly char[] CapabilitiesSeparators = { '|', '#' };
        public const string NoUkCapability = "NoK";
        public static readonly TimeSpan DriverRestTime = TimeSpan.FromHours(9);

        public static readonly IReadOnlyCollection<string> UnknownDestinationClients = new[] { "DE_AMAZONSB", "DE_PLUKON1", "IT_VIGNALI" };

        public static class StatusMessages
        {
            public const string InternalError = "Internal Error, contact IT.";
            public const string DataRetrieveError = "Axapta not reachable";
            public const string DataModelingError = "Failed while modeling data";
        }

        public static class Cabotage
        {
            public const string LoadingStatus = "ATV_I_PAKR";
            public const string UnloadingStatus = "ATV_I_ISKR";
        }

        public static class Vehicles
        {
            public const string UnassignedDriverId = "UNASSIGNED";
            public const string RestActivity = "Rest";
            public const string WorkActivity = "Work";
            public const string LngVehicleGroup = "SCANIA_LNG";
            public const string PreviousWeek = "PreviousWeek";
            public const string WeekBeforeThePreviousWeek = "WeekBeforeThePreviousWeek";
            public const string ThisWeek = "ThisWeek";
            public const bool OverrideExtendedDrives = true;
            public const int ExtendedDrives = 2;

            public static readonly IReadOnlyCollection<OperationalArea> LngOperationalAreas = new[]
            {
                new OperationalArea {CountryCode = "BE", ZipCodes = new List<string>()},
                new OperationalArea {CountryCode = "NL", ZipCodes = new List<string>()},
                new OperationalArea {CountryCode = "LU", ZipCodes = new List<string>()},
                new OperationalArea {CountryCode = "DE", ZipCodes = new List<string>{ "4*", "5*" }},
                new OperationalArea {CountryCode = "ES", ZipCodes = new List<string>()},
                new OperationalArea {CountryCode = "FR", ZipCodes = new List<string>{ "11*", "40*", "64*", "66*" }},
            };

            public static readonly IReadOnlyCollection<string> IgnorePlanningDirections =
                new [] { "ES", "NL_Driver", "PL_Driver", "DE_Driver", "IMP" };
        }

        public static class Tasks
        {
            public const string IntermodalTaskType = "Intermodal";
            public const string JustInTimeTaskType = "JIT";
            public const string ScheduledTaskType = "Schedule";
            public const string LoadingTaskType = "Loading";
            public const string UnloadingTaskType = "Unloading";
            public const string ControlTaskType = "KONTROLE";
            public const string ToDoStateType = "ToDo";
            public const string OtherTaskType = "Other";
            public const string LiveRow = "LiveRow";
            public const string CustomTask = "MUITINE";

            public static readonly IReadOnlyCollection<string> OtherTaskTypes =
                new[] { ControlTaskType, "PAL_ATIDAV", "PAL_PAEMIM", "MUITINE" };

            /// <summary>
            /// Countries in which loading/unloading may require a ferry transfer
            /// </summary>
            public static readonly IReadOnlyList<string> FerryCountryCodes = new[] { "GB", "GR" };

            public static readonly IReadOnlyList<(string Country, string[] Exceptions)> FerryIntermodalExceptions = new[]
                {
                    ("GR", new[] {"BG", "RO", "HU", "SK", "AT", "CZ", "PL", "LT", "LV", "EE", "FI"})
                };

            public static readonly IReadOnlyList<(string Country, string StartTaskType, string IntermodalId)> FerryIntermodalIds =
                new[]
                {
                    ("GB", LoadingTaskType, "DOVER-CALAIS"),
                    ("GB", UnloadingTaskType, "CALAIS-DOVER"),
                    ("GR", LoadingTaskType, "IGOUMENITSA-BRIND"),
                    ("GR", UnloadingTaskType, "BRINDISI-IGOU"),
                };
        }
    }
}