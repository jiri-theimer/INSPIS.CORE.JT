using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BL.bas
{
    public class PasswordChecker
    {
        const string _LOWER_CASE = "abcdefghijklmnopqursuvwxyz";
        const string _UPPER_CAES = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string _NUMBERS = "123456789";
        const string _SPECIALS = @"!@£$%^&*()#€";        
        private bool RequireDigit { get; set; }
        private bool RequireLowercase { get; set; }
        private bool RequireUppercase { get; set; }
        private bool RequireNonAlphanumeric { get; set; }
        private int MinLength { get; set; }
        private int MaxLength { get; set; }

        public PasswordChecker()
        {
            var strFolder = System.IO.Directory.GetCurrentDirectory();
            var config = new ConfigurationBuilder().AddJsonFile(strFolder + "\\appsettings.json", true).Build();
            this.MinLength = Convert.ToInt32(config.GetSection("PasswordChecker")["MinLength"]);
            this.MaxLength = Convert.ToInt32(config.GetSection("PasswordChecker")["MaxLength"]);
            this.RequireDigit = Convert.ToBoolean(config.GetSection("PasswordChecker")["RequireDigit"]);
            this.RequireLowercase = Convert.ToBoolean(config.GetSection("PasswordChecker")["RequireLowercase"]);
            this.RequireUppercase = Convert.ToBoolean(config.GetSection("PasswordChecker")["RequireUppercase"]);
            this.RequireNonAlphanumeric = Convert.ToBoolean(config.GetSection("PasswordChecker")["RequireNonAlphanumeric"]);
        }

        public BO.Result CheckPassword(string pass)
        {
            //min 6 chars, max 12 chars
            if (pass.Length < this.MinLength || pass.Length > this.MaxLength)
                return new BO.Result(true, string.Format("Minimální délka hesla je: {0} znaků, maximální délka: {1} znaků.", this.MinLength, this.MaxLength));

            //No white space
            if (pass.Contains(" "))
                return new BO.Result(true, "Heslo nesmí obsahovat mezeru.");

            //At least 1 upper case letter
            if (this.RequireUppercase && !pass.Any(char.IsUpper))
                return new BO.Result(true, "Heslo musí obsahovat minimálně jedno velké písmeno.");

            //At least 1 lower case letter
            if (this.RequireLowercase && !pass.Any(char.IsLower))
                return new BO.Result(true, "Heslo musí obsahovat minimálně jedno malé písmeno.");

            if (this.RequireDigit)
            {
                //At least 1 numer                
                char[] numbersArray = _NUMBERS.ToCharArray();
                bool b = false;
                foreach (char c in numbersArray)
                {
                    if (pass.Contains(c))
                        b = true;
                }
                if (!b)
                {
                    return new BO.Result(true, "Heslo musí obsahovat minimálně jedno číslo.");
                }
            }



            if (this.RequireNonAlphanumeric)
            {
                //At least 1 special char                
                char[] specialCharactersArray = _SPECIALS.ToCharArray();
                bool b = false;
                foreach (char c in specialCharactersArray)
                {
                    if (pass.Contains(c))
                        b = true;
                }
                if (!b)
                {
                    return new BO.Result(true, "Heslo musí obsahovat minimálně jeden non-alfanumerický znak.");
                }
            }


            return new BO.Result(false, "ok");
        }



        public string GetRandomPassword()
        {

            System.Random rnd = new Random();
            string ret = "";

            // Build up the character set to choose from
            if (this.RequireLowercase) ret += _LOWER_CASE[rnd.Next(_LOWER_CASE.Length - 1)];
            if (this.RequireUppercase) ret += _UPPER_CAES[rnd.Next(_UPPER_CAES.Length - 1)];
            if (this.RequireDigit) ret += _NUMBERS[rnd.Next(_NUMBERS.Length - 1)];
            if (this.RequireNonAlphanumeric) ret += _SPECIALS[rnd.Next(_SPECIALS.Length - 1)];
            ret += BO.BAS.GetGuid();

            return ret.Substring(0, this.MinLength);


        }
    }
}
