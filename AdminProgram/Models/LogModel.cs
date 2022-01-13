using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace AdminProgram.Models
{
    public partial class LogModel : ObservableObject
    {
        //사용자 아이디
        private string userId;
        public string UserId
        {
            get => userId;
            set => SetProperty(ref userId, value);
        }

        //사용자 IP
        private string userIp;
        public string UserIp
        {
            get => userIp;
            set => SetProperty(ref userIp, value);
        }

        //접근 경로
        private string accessPath;
        public string AccessPath
        {
            get => accessPath;
            set => SetProperty(ref accessPath, value);
        }

        //로그 아이디
        private int logId;
        public int LogId
        {
            get => logId;
            set => SetProperty(ref logId, value);
        }

        //로그 레벨
        private string logLevel;
        public string LogLevel
        {
            get => logLevel;
            set => SetProperty(ref logLevel, value);
        }

        //로그 메시지
        private string logMessage;
        public string LogMessage
        {
            get => logMessage;
            set => SetProperty(ref logMessage, value);
        }

        //로그 날짜
        private DateTime logDate;
        public DateTime LogDate
        {
            get => logDate;
            set => SetProperty(ref logDate, value);
        }
    }
}
