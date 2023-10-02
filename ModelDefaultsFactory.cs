using AssignmentsApi.Extensions;
using AssignmentsApi.Models.Common;
using AssignmentsApi.Models.Vehicle;
using System;
using System.Collections.Generic;
using Base = AssignmentsApi.Models.Base.Base;
using Driver = AssignmentsApi.Models.Vehicle.Driver;

namespace AssignmentsApi
{
    public static class ModelDefaultsFactory
    {
        private const string FreezUnit = "FreezUnit";
        private static readonly IReadOnlyDictionary<string, string> TrailerTypes = new Dictionary<string, string>
        {
            {"EU_LT_TEN", "Tent"},
            {"EU_LT_TEN_2", "Tent"},
            {"EU_LT_MMG", "MiniMega"},
            {"EU_DE_MMG", "MiniMega"},
            {"EU_PL_MEG", "MiniMega"},
            {"EU_LT", FreezUnit},
            {"EU_LT_2", FreezUnit},
            {"EU_LT_NYD", FreezUnit},
            {"EU_PL", FreezUnit},
            {"EU_PL_2", FreezUnit},
            {"EU_LT_GBR", FreezUnit},
            {"EU_LT_GBR_2", FreezUnit},
            {"EU_LT_INT", FreezUnit},
            {"EU_PL_GBR", FreezUnit},
            {"EU_PL_GBR_2", FreezUnit},
        };

        public static Base GetDefaultBase() =>
            new Base
            {
                Id = "UNKNOWN",
                Position = Position.Create(54.63883M, 25.27039M, "LT", "02190"),
                IsTrailerDispositionAllowed = false
            };

        public static VehicleProperties WithDefaultTrailerProperties(this VehicleProperties properties, string parkGroup)
        {
            properties.TrailerPlate = "NO_TRAILER";
            properties.TrailerType = !string.IsNullOrWhiteSpace(parkGroup) && TrailerTypes.ContainsKey(parkGroup)
                ? TrailerTypes[parkGroup]
                : FreezUnit;
            properties.WPP = true;
            properties.HasPanicButton = false;
            properties.HasMapping = false;
            properties.FCL = true;
            properties.HasThermogram = properties.TrailerType == FreezUnit;
            properties.EmissionStandard = "Euro 5";
            properties.TrailerTapaLevel = 0;
            properties.TrailerTA60 = 60;
            properties.TrailerGroup = "Unknown";
            properties.TrailerTransportGroup = "Unknown";

            return properties;
        }

        public static Driver GetDefaultDriver(DateTime time, DriverState driverState)
        {
            return new Driver
            {
                Id = Consts.Vehicles.UnassignedDriverId,
                State = driverState,
                RotationBaseIds = new List<string> { "UNKNOWN" },
                RotationDate = time.AddDays(90),
                Properties = new Driver1Properties
                {
                    DriverHasGDP = false,
                    DriverHasADR = false,
                    DriverHasUKVisa = false,
                    DriverNationality = "LT",
                    DriverExperienceLevel = 1
                }
            };
        }

        public static DriverState GetDefaultDriverState(DateTime time)
        {
            return new DriverState
            {
                Type = "ExtendedEC561",
                StartOfOperationalDay = time,
                StartOfOperationalWeek = time,
                LastRegularWeeklyRestSettlementWeek = Consts.Vehicles.ThisWeek,
                LastEvent = new LastEvent(Consts.Vehicles.RestActivity, time.AddHours(-48)),
                Totals = DriverStateExtensions.GetDefaultTotals()
            };
        }
    }
}