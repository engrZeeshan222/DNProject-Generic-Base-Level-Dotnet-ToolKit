using System.ComponentModel;

namespace GenericToolKit.Domain.Extensions
{
    public static class TypesValidator
    {
        // Checks if valid object
        public static bool IsValidObject(this object obj)
        {
            try
            {
                if(obj == null)
                {
                    throw new Exception("Obj is coming Null!");
                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        // Checks if valid dictionary
        public static bool IsValidDictionary(this Dictionary<string,object> dictionary)
        {
            try
            {
                if(dictionary == null || dictionary.Count == 0)
                {
                    throw new Exception("Dictionary is coming Null or Empty!");
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // To dictionary
        public static Dictionary<string,object> ToDictionary(this object obj)
        {
            try
            {
                if(!obj.IsValidObject())
                {
                    return new Dictionary<string, object>();
                }

                var dicToReturn = new Dictionary<string, object>();
                var allObjectProperties = TypeDescriptor.GetProperties(obj);
                for(int i = 0; i < allObjectProperties.Count; i++)
                {
                    dicToReturn.Add(allObjectProperties[i].Name, allObjectProperties[i]?.GetValue(obj));
                }
                return dicToReturn;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>();
            }
        }

        public static T Parse<T>(this Dictionary<string, object> dictionary, string keyToParse)
        {
            try
            {
                if (!dictionary.IsValidDictionary())
                {
                    return default;
                }

                if (dictionary.TryGetValue(keyToParse, out var result))
                {
                    if (result is null)
                        return default;

                    if (result is T typedResult)
                        return typedResult;

                    var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                    return (T)Convert.ChangeType(result, targetType);
                }

                return default;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public static bool IsValidList<T>(this List<T> list)
        {
            try
            {
                if (list == null || list.Count == 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsValidEnumerable<T>(this IEnumerable<T> enumerable)
        {
            try
            {
                if (enumerable == null || enumerable.Count() == 0)
                {
                    throw new Exception("List is coming Null or Empty!");
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Checks if valid string
        public static bool IsValidString(this string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    throw new Exception("List is coming Null or Empty!");
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Checks if valid date
        public static bool IsValidDate(this DateTime? date)
        {
            return (date is not null) && date != DateTime.MinValue && date != DateTime.MaxValue;
        }
    }
}

