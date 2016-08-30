using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkBoxFramework
{
    public class WBConfigStepFeedback
    {
        public String Name { get; private set; }
        public String UpdateType { get; private set; }
        public String Status { get; private set; }
        public List<String> Feedback { get; private set; }
        public String NextStepName { get; set; }

        public const String UPDATE_TYPE__CHECK = "Check";
        public const String UPDATE_TYPE__UPDATE = "Update";
        public const String UPDATE_TYPE__SETUP = "Setup";

        public const String STATUS__TO_DO = "To Do";
        public const String STATUS__SUCCESS = "Success";
        public const String STATUS__FAILED = "Failed";


        public WBConfigStepFeedback(String name)
        {
            Name = name;
            UpdateType = UPDATE_TYPE__SETUP;
            Status = STATUS__TO_DO;
            Feedback = new List<String>();
        }

        public void AddFeedback(String feedback)
        {
            feedback = feedback.WBxTrim();
            if (!String.IsNullOrEmpty(feedback)) Feedback.Add(feedback);
        }

        public void JustLog(String message)
        {
            WBLogging.Config.Monitorable(Name + " (" + UpdateType + " " + Status + "): Log: " + message);
        }

        public void JustLog(String message, Exception exception)
        {
            WBLogging.Config.Unexpected(Name + " (" + UpdateType + " " + Status + "): Log: " + message, exception);
        }

        public void LogFeedback(String feedback)
        {
            AddFeedback(feedback);
            WBLogging.Config.Monitorable(Name + " (" + UpdateType + " " + Status + "): Log: " + feedback);
        }

        public void LogFeedback(String feedback, Exception exception)
        {
            AddFeedback(feedback);
            WBLogging.Config.Unexpected(Name + " (" + UpdateType + " " + Status + "): Log: " + feedback, exception);
        }

        public void Checked(String feedback)
        {
            if (UpdateType != UPDATE_TYPE__UPDATE) UpdateType = UPDATE_TYPE__CHECK;
            Success(feedback);
        }

        public void Created(String feedback)
        {
            if (UpdateType == UPDATE_TYPE__CHECK) UpdateType = UPDATE_TYPE__UPDATE;
            else UpdateType = UPDATE_TYPE__SETUP;
            Success(feedback);
        }

        public void Updating(String feedback)
        {
            UpdateType = UPDATE_TYPE__UPDATE;
            AddFeedback(feedback);
        }

        public void Updated(String feedback)
        {
            UpdateType = UPDATE_TYPE__UPDATE;            
            Success(feedback);
        }


        public void Success()
        {
            Success("");
        }

        public void Success(String feedback)
        {
            if (Status != STATUS__FAILED) Status = STATUS__SUCCESS;
            AddFeedback(feedback);
            WBLogging.Config.Monitorable(Name + " (" + UpdateType + " " + Status + "): Success: " + feedback);
        }

        public void Failed()
        {
            Failed("", null);
        }

        public void Failed(String feedback)
        {
            Failed(feedback, null);
        }

        public void Failed(String feedback, Exception exception)
        {
            Status = STATUS__FAILED;
            AddFeedback(feedback);
            AddException(exception);

            WBLogging.Config.Unexpected(Name + " (" + UpdateType + " " + Status + "): Failed: " + feedback, exception);
        }

        public void AddException(Exception exception)
        {
            while (exception != null)
            {
                AddFeedback(exception.Message);
                AddFeedback(exception.StackTrace);

                exception = exception.InnerException;
                if (exception != null)
                {
                    AddFeedback("    ---- Inner Exception: ----");
                }
            }
        }

        public bool IsUpdate()
        {
            return (this.UpdateType == UPDATE_TYPE__UPDATE);
        }

        public bool IsCheck()
        {
            return (this.UpdateType == UPDATE_TYPE__CHECK);
        }

    }


}
