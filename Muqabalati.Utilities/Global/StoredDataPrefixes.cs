using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muqabalati.Utilities.Global
{
    public class StoredDataPrefixes
    {
        // in Session Storage


        // in Cashe Storage
        public const string VerificationCodeKey = "VerificationCode_";
        public const int VerificationCodeKeyTime = 30;



        // Local Storage
        const string TIMER_KEY = "verification_timer"; 
    }
}
