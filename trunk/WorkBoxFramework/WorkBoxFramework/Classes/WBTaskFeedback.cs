using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkBoxFramework
{
    public class WBTaskFeedback
    {
        public String Name { get; private set; }
        public String TaskType { get; private set; }
        public String Status { get; private set; }
        public List<String> Feedback { get; private set; }
        public String NextTaskName { get; set; }

        public const String TASK_TYPE__CHECK = "Check";
        public const String TASK_TYPE__UPDATE = "Update";
        public const String TASK_TYPE__SETUP = "Setup";
        public const String TASK_TYPE__PUBLISH = "Publish";

        public const String STATUS__TO_DO = "To Do";
        public const String STATUS__SUCCESS = "Success";
        public const String STATUS__FAILED = "Failed";


        private String _prettyName = null;
        public String PrettyName {
            get
            {
                if (String.IsNullOrEmpty(_prettyName)) return Name;
                return _prettyName;
            }
            set
            {
                _prettyName = value;
            }
        }

        public WBTaskFeedback(String type, String name)
        {
            Name = name;
            TaskType = type;
            Status = STATUS__TO_DO;
            Feedback = new List<String>();
        }

        public WBTaskFeedback(String name)
        {
            Name = name;
            TaskType = TASK_TYPE__SETUP;
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
            WBLogging.Config.Monitorable(Name + " (" + TaskType + " " + Status + "): Log: " + message);
        }

        public void JustLog(String message, Exception exception)
        {
            WBLogging.Config.Unexpected(Name + " (" + TaskType + " " + Status + "): Log: " + message, exception);
        }

        public void LogFeedback(String feedback)
        {
            AddFeedback(feedback);
            WBLogging.Config.Monitorable(Name + " (" + TaskType + " " + Status + "): Log: " + feedback);
        }

        public void LogFeedback(String feedback, Exception exception)
        {
            AddFeedback(feedback);
            WBLogging.Config.Unexpected(Name + " (" + TaskType + " " + Status + "): Log: " + feedback, exception);
        }

        public void Checked(String feedback)
        {
            if (TaskType != TASK_TYPE__UPDATE) TaskType = TASK_TYPE__CHECK;
            Success(feedback);
        }

        public void Created(String feedback)
        {
            if (TaskType == TASK_TYPE__CHECK) TaskType = TASK_TYPE__UPDATE;
            else TaskType = TASK_TYPE__SETUP;
            Success(feedback);
        }

        public void Updating(String feedback)
        {
            TaskType = TASK_TYPE__UPDATE;
            AddFeedback(feedback);
        }

        public void Updated(String feedback)
        {
            TaskType = TASK_TYPE__UPDATE;            
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
            WBLogging.Config.Monitorable(Name + " (" + TaskType + " " + Status + "): Success: " + feedback);
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

            WBLogging.Config.Unexpected(Name + " (" + TaskType + " " + Status + "): Failed: " + feedback, exception);
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
            return (this.TaskType == TASK_TYPE__UPDATE);
        }

        public bool IsCheck()
        {
            return (this.TaskType == TASK_TYPE__CHECK);
        }

    }


}
