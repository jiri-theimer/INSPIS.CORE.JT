using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace BO
{
    public static class Reflexe
    {
        private static string ErrorMessage;


        public static object GetPropertyValue(object obj, string PropName)
        {
            Type objType = obj.GetType();
            object PropValue = null;
            try
            {
                System.Reflection.PropertyInfo pInfo = objType.GetProperty(PropName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);

                PropValue = pInfo.GetValue(obj, System.Reflection.BindingFlags.GetProperty, null, null, null);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return PropValue;
        }



        public static void SetPropertyValue(object obj, string PropName, object objNewValue)
        {
            Type objType = obj.GetType();
            System.Reflection.PropertyInfo pInfo = objType.GetProperty(PropName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);



            if (pInfo.PropertyType.Name == "Int32")
            {
                if (objNewValue == null)
                {
                    objNewValue = 0;
                }
                else
                {
                    objNewValue = Convert.ToInt32(objNewValue);
                }

            }
            if (pInfo.PropertyType.Name == "Double")
            {
                if (objNewValue == null) { objNewValue = 0; } else { objNewValue = Convert.ToDouble(objNewValue); }
            };
            if (pInfo.PropertyType.Name == "Decimal")
            {
                if (objNewValue == null) { objNewValue = 0; } else { objNewValue = Convert.ToDecimal(objNewValue); }
            };
            //if (pInfo.PropertyType.Name == "Nullable`1")
            //{
            //    if (objNewValue == null) { objNewValue = null; } else { objNewValue = Convert.ToDecimal(objNewValue); }
            //};

            
            pInfo.SetValue(obj, objNewValue, null);

        }

       

        
    }


}
