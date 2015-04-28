using System;
using System.Text.RegularExpressions;

namespace FCWebService
{
    // may be use  CreateVerificationSession method of the Engine object to run the verification process.
    public class PersonData
    {
        public char[] Number
        {
            get { return number; }
            set 
            {
                Regex numberRegex = new Regex(patternNumber);
                if (numberRegex.IsMatch(new string (value)))
                {
                    number = value; 
                }
                else
                {
                    try
                    {
                        if (!Char.IsUpper(value[0]) && value[0] == '0')
                        {
                            value[0] = 'O';;
                        }

                        if (!Char.IsUpper(value[1]) && value[1] == '0')
                        {
                            value[1] = 'O';
                        }

                        if (numberRegex.IsMatch(new string(value)))
                        {
                            number = value;
                        }

                        else
                        {
                            processInfo.error += "Recognition passport number does not pass validation. This text is wrong: " + new string(value);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                    }
                }               
            }
        }        

        public char[] Date
        {
            get { return date; }
            set 
            {
                Regex dateRegex = new Regex(patternDate);
                if (dateRegex.IsMatch(new string(value)))
                {                  
                    date = value;
                }
                else
                {
                   processInfo.error += "Recognition passport Expiry Date does not pass validation. This text is wrong: " + new string(value);
                }
            }
        }

        private char[] number = new char[] { 'e', 'r', 'r', 'o', 'r', ' ', ' ', ' ' };
        private char[] date = new char[] { 'e', 'r', 'r', 'o', 'r', ' ', ' ', ' ', ' ' };
        private const string patternNumber = @"^[A-Z][A-Z][0-9][0-9][0-9][0-9][0-9][0-9]$";
        private const string patternDate = @"^[0-9][0-9] \b(JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC) [0-9][0-9]$";
        
        public ProcessInfo processInfo = new ProcessInfo();
        
        public class ProcessInfo
        {
            public int processNumber;
            public string error;
            public int freeProcCount;
            public string RequestGUID;
        };
    }
}