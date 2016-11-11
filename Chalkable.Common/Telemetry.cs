using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Chalkable.Common {
    public enum Verbosity {
        Off = 0,
        Error = 1,
        Warning = 2,
        Info = 3,
        Verbose = 4
    }
    public static class Telemetry {
        private static TelemetryClient telemetrylient = new TelemetryClient();
        private static string SUCCESS_CODE = "200";
        private static string FAILURE_CODE = "500";        

        private static TelemetryClient TelemetryClient {
            get { return telemetrylient; }
        }

        public static void TrackException(Exception ex, string districtId = null, string taskId = null) {

            try {
                if (Chalkable.Common.Settings.Verbosity >= Verbosity.Error) { 
                    // Set up some properties:
                    Dictionary<string, string> properties = null;
                
                    if (!string.IsNullOrWhiteSpace(districtId) || !string.IsNullOrWhiteSpace(taskId)) { 
                        properties = new Dictionary<string, string>();

                        if (!string.IsNullOrWhiteSpace(districtId)) {
                            properties.Add("districtId", districtId);
                        }
                        if (!string.IsNullOrWhiteSpace(taskId)) {
                            properties.Add("taskId", taskId);
                        }                    
                    }

                    TelemetryClient.TrackException(ex, properties);
                }
            }
            catch (Exception) {                                
            }
        }

        public static void DispatchRequest(string name, string operation, DateTimeOffset startTime, TimeSpan duration, bool success, Verbosity verbosityThreshold, string districtId = null, string taskId = null, string details = null) {

            if (IsLoggingConfigured(verbosityThreshold))
            {
                var properties = new Dictionary<string, string>();

                if (!string.IsNullOrWhiteSpace(districtId))
                {
                    properties.Add("districtId", districtId);
                }
                if (!string.IsNullOrWhiteSpace(taskId))
                {
                    properties.Add("taskId", taskId);
                }
                if (!string.IsNullOrWhiteSpace(details))
                {
                    properties.Add("details", details);
                }

                DispatchRequest(name, operation, startTime, duration, success, verbosityThreshold, properties);
            }
        }

        public static void DispatchRequest(string name, string operation, DateTimeOffset startTime, TimeSpan duration, bool success, Verbosity verbosityThreshold, Dictionary<string, string> details)
        {

            if (IsLoggingConfigured(verbosityThreshold))
            {
                var request = new RequestTelemetry();
                //request.Context.InstrumentationKey = Settings.InstrumentationKey;
                request.Name = name;
                request.Timestamp = startTime;
                request.Duration = duration;
                request.Context.Operation.Name = operation;

                foreach (var pair in details)
                {
                    request.Context.Properties.Add(pair.Key, pair.Value);
                }                

                request.Success = success;
                request.ResponseCode = (success) ? SUCCESS_CODE : FAILURE_CODE;
                TelemetryClient.TrackRequest(request);
            }
        }

        private static bool IsLoggingConfigured(Verbosity verbosityThreshold) {

            var configured = Chalkable.Common.Settings.Verbosity >= verbosityThreshold;

            return configured;
        }        
    }
}