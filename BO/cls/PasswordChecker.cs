using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO.CLS
{
    public class PasswordChecker
    {
        public BO.Result CheckPassword(string pass, int intMinLength,int intMaxLength,bool bolDigit,bool bolUpper,bool bolLower,bool bolSpecialChar)
        {            
            //min 6 chars, max 12 chars
            if (pass.Length < intMinLength || pass.Length > intMaxLength)
                return new BO.Result(true,string.Format("Minimální délka hesla je: {0} znaků, maximální délka: {1} znaků.", intMinLength,intMaxLength));

            //No white space
            if (pass.Contains(" "))
                return new BO.Result(true, "Heslo nesmí obsahovat mezeru.");

            //At least 1 upper case letter
            if (bolUpper && !pass.Any(char.IsUpper))
                return new BO.Result(true, "Heslo musí obsahovat minimálně jedno velké písmeno.");

            //At least 1 lower case letter
            if (bolLower && !pass.Any(char.IsLower))
                return new BO.Result(true, "Heslo musí obsahovat minimálně jedno malé písmeno.");

            if (bolDigit)
            {
                //At least 1 numer                
                char[] numbersArray = "0123456789".ToCharArray();
                bool b = false;
                foreach (char c in numbersArray)
                {
                    if (pass.Contains(c))
                        b = true;
                }
                if (b==false)
                {
                    return new BO.Result(true, "Heslo musí obsahovat minimálně jedno číslo.");
                }                    
            }

            ////No two similar chars consecutively
            //for (int i = 0; i < pass.Length - 1; i++)
            //{
            //    if (pass[i] == pass[i + 1])
            //        return false;
            //}

            if (bolSpecialChar)
            {
                //At least 1 special char
                string specialCharacters = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";
                char[] specialCharactersArray = specialCharacters.ToCharArray();
                bool b = false;
                foreach (char c in specialCharactersArray)
                {
                    if (pass.Contains(c))
                        b = true;
                }
                if (b == false)
                {
                    return new BO.Result(true, "Heslo musí obsahovat minimálně jeden non-alfanumerický znak.");
                }
            }
           

            return new BO.Result(false, "ok");
        }


        public string RandomPassword(int PasswordMinLength)
        {
            var x = new Random();
            string s= BO.BAS.GetGuid().Substring(0, 1).ToUpper() + BO.BAS.GetGuid().Substring(0, PasswordMinLength - 1) + "!@$%^&*()#".Substring(x.Next(0, 9), 1);
            if (!s.Any(char.IsUpper))
            {
                s += "A";
            }
            if (!s.Any(char.IsLower))
            {
                s += "a";
            }
            return s;

        }
    }
}
