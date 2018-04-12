using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ViewModels
{
    public class ParticipantViewModel
    {
        private readonly AccountType _accountType;

        public ParticipantViewModel(AccountType accountType)
        {
            _accountType = accountType;
        }
        public int AccountId { get; set; }
        public string Names { get; set; }
        public string Photo { get; set; }
        public string Role
        {
            get
            {
                if(_accountType == AccountType.Lecturer)
                {
                    return "Moderator";
                }
                else if(_accountType == AccountType.Student)
                {
                    return "Student";
                }
                else
                {
                    return "Unknown";
                }
            }
        }
    }
}
